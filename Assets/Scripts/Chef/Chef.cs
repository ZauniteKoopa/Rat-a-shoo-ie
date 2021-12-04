using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions;
using UnityEngine.Events;

// Main comparer method for getting positions
public class SolutionPosComparer : IComparer<Vector3> {
    private Transform transform;

    // main constructor
    public SolutionPosComparer (Transform t) {
        transform = t;
    }

    // Main compare method, value vector3 positions based on how close they are to current transform
    //  If x < y, return something less than 0
    //  If x == y, return 0
    //  If x > y, return something more than 0
    public int Compare(Vector3 x, Vector3 y) {
        Vector3 curPosition = transform.position;
        return (int)(Vector3.Distance(x, curPosition) - Vector3.Distance(y, curPosition));
    }
}

// Main enum to indicate recipe stages (The recipes are all in appropriate order from top to bottom)
//  This is done for each ingredient
public enum RecipeStage {
    GO_TO_INGREDIENT,
    GO_TO_ACTION,
    PLACE_INGREDIENT_BACK,
    GET_FOOD,
    BRING_FOOD_TO_CUSTOMER,
}



// Main AI behavior for the chef
public class Chef : MonoBehaviour
{
    // Variables for navigation
    private NavMeshAgent navMeshAgent;
    private MeshRenderer meshRenderer;

    // Serialized variables for navigation and speed
    [Header("Navigation")]
    [SerializeField]
    private ChefSight chefSensing = null;
    [SerializeField]
    private ChefSolutionSensor solutionSensor = null;
    [SerializeField]
    private float passiveMovementSpeed = 3.5f;
    [SerializeField]
    private float confusionDuration = 0.75f;
    [SerializeField]
    private float navDistance = 1f;
    [SerializeField]
    private Transform[] angryPatrolPoints = null;
    private int angerPatrolIndex = 0;

    // Trap sensing
    [SerializeField]
    private RatCage[] initialTraps = null;
    private Transform sensedTrap = null;

    // Serialized variables for attacking and chasing the rat
    [Header("Aggression")]
    [SerializeField]
    private float chaseMovementSpeed = 6f;
    [SerializeField]
    private float angryMovementSpeed = 12f;
    [SerializeField]
    private Color angryColor = Color.red;
    private bool aggressive = false;
    private Vector3 lastSeenTarget = Vector3.zero;
    private bool canSpotRat = true;                 //Boolean variable for when rat gets hit and the chef resets to passive points
    private bool didHitRat = false;                 //Boolean on whether or not the rat was hit by a chef. Needed for when resetting
    private AbstractAggressiveChefAction aggressiveAction = null;

    // Variables for issue management
    [Header("IssueHandling")]
    [SerializeField]
    private float surprisedAtIssueDuration = 2.0f;
    [SerializeField]
    private float noSolutionFoundDuration = 2.0f;
    [SerializeField]
    private Transform chefSprite = null;
    [SerializeField]
    private Vector3 solutionHook = Vector3.zero;
    [SerializeField]
    private ThoughtBubble thoughtBubble = null;
    [SerializeField]
    private ChefCloset closet = null;
    private IssueObject highestPriorityIssue = null;
    private LevelInfo levelInfo = null;

    // Recipe handling
    [Header("Recipe / Customer Handling")]
    [SerializeField]
    private OrderWindow orderWindow = null;
    private Recipe targetRecipe = null;
    private int currentRecipeStep = 0;
    private RecipeStage currentIngredientStage = RecipeStage.GO_TO_INGREDIENT;
    [SerializeField]
    private StockPot cookingStation = null;
    [SerializeField]
    private GameObject mealBox = null;
    [SerializeField]
    private SpriteRenderer mealSprite = null;
    private FoodInstance servedFood = null;
    private bool mealPoisoned = false;

    // Anger handling
    private bool angered = false;
    private bool inStartAngerSequence = false;

    // Variables for getting solutions
    private bool sensingSolutions = false;
    private SolutionObject targetedSolution = null;
    private SolutionType targetedSolutionType = SolutionType.FIRE_EXTINGUISHER;
    private Vector3 targetedSolutionPosition = Vector3.zero;
    private SolutionPosComparer solutionPosComparer;

    // Variables for cage management
    private ChefTrapManager trapManager = null;

    // Variables for animation
    [Header("Animation")]
    [SerializeField]
    private Animator animator = null;
    bool facingRight = false;
    [SerializeField]
    private SpriteRenderer characterSprite = null;
    [SerializeField]
    private ChefHitboxRotator hitboxRotator = null;

    // Unity Events
    [Header("ToDo List Events")]
    public UnityEvent chefClosetEvent;

    // Audio variables
    private ChefAudioManager audioManager;

    // Activation flag handling
    private bool willDeactivate = false;


    // Start is called before the first frame update
    void Start()
    {
        levelInfo = FindObjectOfType<LevelInfo>();

        meshRenderer = GetComponent<MeshRenderer>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        audioManager = GetComponent<ChefAudioManager>();
        aggressiveAction = GetComponent<AbstractAggressiveChefAction>();
        trapManager = GetComponent<ChefTrapManager>();
        solutionPosComparer = new SolutionPosComparer(transform);

        thoughtBubble.mainLevel = levelInfo;
        targetRecipe = orderWindow.getCurrentRecipe();
        mealSprite.sprite = targetRecipe.foodSprite;

        // Set up traps
        if (initialTraps != null) {
            foreach(RatCage trap in initialTraps) {
                trap.trapTriggerEvent.AddListener(onRatCageSetOff);
            }
        }

        // Connect to events
        chefSensing.issueEnterEvent.AddListener(onIssueSpotted);
        solutionSensor.solutionSensedEvent.AddListener(onSolutionSpotted);
        cookingStation.taintedMealEvent.AddListener(onPoisonedMeal);
        levelInfo.restaurantDestroyedEvent.AddListener(lockChefInAnger);
        levelInfo.destroyChefsEvent.AddListener(destroyChef);
    }

    // On update, update animator
    private void Update()
    {

        animator.SetFloat("movementspeed", navMeshAgent.velocity.magnitude/navMeshAgent.speed);
        animator.SetBool("aggro", aggressive);

        float lookDirection = WorldSprite.getSpriteLookDirection(transform.forward);
        animator.SetFloat("direction", lookDirection);

        if (lookDirection == 0.5f)
        {
            if (transform.forward.x > 0 && !facingRight)
                Flip();
            else if (transform.forward.x < 0 && facingRight)
                Flip();
        }

        hitboxRotator.rotateHitbox(lookDirection, facingRight);
    }

    // Main intelligence loop
    private IEnumerator mainIntelligenceLoop() {
        bool interrupted = false;

        // Main loop
        while (true) {
            // Main method to process unrelated info
            bool holdsUnrelatedSolution = targetedSolution != null && !aggressive;

            if (holdsUnrelatedSolution &&  highestPriorityIssue != null) {
                holdsUnrelatedSolution = true;
            } else if (holdsUnrelatedSolution && currentRecipeStep < targetRecipe.getNumSteps()) {
                holdsUnrelatedSolution = targetedSolution.solutionType != targetRecipe.getSolutionStep(currentRecipeStep);
            }

            // If you were interrupted by a higher priority issue and you were holding onto a solutionObject for a hazard, put the solution object back
            if (holdsUnrelatedSolution) {
                if (holdsUnrelatedSolution &&  highestPriorityIssue != null) {
                    navMeshAgent.enabled = false;
                    faceHighPriorityIssue();
                    audioManager.playChefAlert();
                    yield return new WaitForSeconds(surprisedAtIssueDuration);
                    navMeshAgent.enabled = true;
                    navMeshAgent.speed = chaseMovementSpeed;

                    thoughtBubble.thinkOfSolution(highestPriorityIssue.getNthStep(0));
                }
                
                yield return goToPosition(targetedSolution.getInitialLocation());
                dropSolutionObject(targetedSolution);
                targetedSolution = null;

                interrupted = true;
            }


            // MAIN TREE
            if (highestPriorityIssue != null) {                         // Branch for when chef is dealing with issue
                yield return solveIssue(interrupted);
                interrupted = false;
                aggressive = chefSensing.currentRatTarget != null;

                if (willDeactivate && highestPriorityIssue == null && !aggressive && !mealPoisoned) {
                    yield return deactivateChefFromActiveBranch();
                }

                if (aggressive)
                    yield return spotRat();
            }
            else if (aggressive)                                        // Branch for when chef is chasing rat
            {
                yield return doAggressiveAction(chaseMovementSpeed);
                
                if (willDeactivate && !aggressive && !mealPoisoned) {
                    yield return deactivateChefFromActiveBranch();
                }
                
            }
            else if (mealPoisoned) {                                    // Branch for when meal is poisoned, serve immediately
                // Drop targeted solution
                thoughtBubble.clearUpThoughts();
                if (targetedSolution != null) {
                    yield return goToPosition(targetedSolution.getInitialLocation());
                    dropSolutionObject(targetedSolution);
                    targetedSolution = null;
                }

                // Set up finish recipe variables
                currentRecipeStep = targetRecipe.getNumSteps();
                if (currentIngredientStage != RecipeStage.GET_FOOD && currentIngredientStage != RecipeStage.BRING_FOOD_TO_CUSTOMER) {
                    currentIngredientStage = RecipeStage.GET_FOOD;
                }

                //Bring recipe to customer
                yield return finishRecipe();

                if (willDeactivate) {
                    yield return deactivateChefFromActiveBranch();
                }
            }
            else                                                        // Passive, default branch
            {
                yield return passiveAction();
            }
        }
    }

    // Main anger loop when chef is finally angered
    private IEnumerator mainAngerLoop() {
        while (true) {
            if (aggressive) {
                yield return doAggressiveAction(angryMovementSpeed);

                if (willDeactivate && !aggressive && sensedTrap == null) {
                    yield return deactivateChefFromActiveBranch();
                }

            } else if (sensedTrap != null) {
                // Face the direction of the trap
                Transform lockedTarget = sensedTrap;
                Vector3 flattenTarget = new Vector3(lockedTarget.position.x, 0, lockedTarget.position.z);
                Vector3 flattenPosition = new Vector3(transform.position.x, 0, transform.position.z);
                transform.forward = (flattenTarget - flattenPosition).normalized;

                audioManager.playChefAlert();
                navMeshAgent.enabled = false;
                yield return new WaitForSeconds(0.5f);
                navMeshAgent.enabled = true;

                // Go to position and wait
                yield return goToPosition(sensedTrap.position);

                navMeshAgent.enabled = false;
                yield return new WaitForSeconds(0.5f);
                navMeshAgent.enabled = true;

                sensedTrap = null;

                if (willDeactivate) {
                    yield return deactivateChefFromActiveBranch();
                }
            } else {
                navMeshAgent.speed = angryMovementSpeed;
                Vector3 currentPatrolPoint = angryPatrolPoints[angerPatrolIndex].position;
                yield return goToPosition(currentPatrolPoint);
                yield return new WaitForSeconds(1.5f);
                angerPatrolIndex = (angerPatrolIndex + 1) % angryPatrolPoints.Length;
            }
        }
    }

    // Main ienumerator introducing to anger
    private IEnumerator startAngerSequence() {
        inStartAngerSequence = true;
        canSpotRat = false;
        navMeshAgent.enabled = false;

        // Face order window for a few seconds before getting angry
        Transform lockedTarget = orderWindow.transform;
        Vector3 flattenTarget = new Vector3(lockedTarget.position.x, 0, lockedTarget.position.z);
        Vector3 flattenPosition = new Vector3(transform.position.x, 0, transform.position.z);
        transform.forward = (flattenTarget - flattenPosition).normalized;

        audioManager.playChefEnrage();

        yield return new WaitForSeconds(0.5f);

        animator.SetBool("angry", true);
        float angerTimer = 0.0f;
        WaitForEndOfFrame waitFrame = new WaitForEndOfFrame();

        while (angerTimer < 1.5f) {
            yield return waitFrame;
            angerTimer += Time.deltaTime;
            animator.GetComponent<SpriteRenderer>().color = Color.Lerp(Color.white, angryColor, angerTimer / 1.5f);
        }
        
        animator.GetComponent<SpriteRenderer>().color = angryColor;

        navMeshAgent.enabled = true;
        canSpotRat = true;
        aggressiveAction.makeAngry();
        inStartAngerSequence = false;

        if (willDeactivate) {
            yield return deactivateChefFromActiveBranch();
        }

        if (chefSensing.currentRatTarget != null) {
            aggressive = true;
            yield return spotRat();
        } else {
            yield return mainAngerLoop();
        }
    }

    // Method to do passive action when there is no hazard in mind or rat to chase
    //  can be overriden (TODO: make this general AI class an abstract class)
    protected virtual IEnumerator passiveAction() {
        yield return finishRecipe();
    }

    // Main sequence to do passive recipe gathering
    private IEnumerator finishRecipe() {
        // Force chef to move at passive speed
        navMeshAgent.speed = passiveMovementSpeed;
        //Debug.Log(currentRecipeStep + " " + targetRecipe.getNumSteps());

        // Go through each step in the recipe, keeping track of the state of the recipe as you go
        for (int i = currentRecipeStep; i < targetRecipe.getNumSteps(); i++) {
            SolutionType currentIngredient = targetRecipe.getSolutionStep(i);

            // Go to ingredient if haven't done so already
            if (targetedSolution == null && currentIngredientStage != RecipeStage.PLACE_INGREDIENT_BACK) {
                yield return goToSolutionObject(currentIngredient);
                currentIngredientStage = RecipeStage.GO_TO_ACTION;
            }

            // Go to action area and do action if you haven't done it
            if (currentIngredientStage == RecipeStage.GO_TO_ACTION) {
                yield return goToPosition(cookingStation.transform.position);

                navMeshAgent.enabled = false;
                targetedSolution.playUsageSound();
                yield return new WaitForSeconds(targetedSolution.getDuration());
                navMeshAgent.enabled = true;

                cookingStation.addProperIngredient();

                currentIngredientStage = RecipeStage.PLACE_INGREDIENT_BACK;
            }

            // Place ingredient back if chef still has ingredient
            if (currentIngredientStage == RecipeStage.PLACE_INGREDIENT_BACK) {
                if (targetedSolution != null) {
                    if (i + 1 < targetRecipe.getNumSteps()) {
                        thoughtBubble.thinkOfSolution(targetRecipe.getSolutionStep(i + 1));
                    }

                    yield return goToPosition(targetedSolution.getInitialLocation());
                    dropSolutionObject(targetedSolution);
                    targetedSolution = null;
                }

                currentIngredientStage = (currentRecipeStep < targetRecipe.getNumSteps() - 1) ? RecipeStage.GO_TO_INGREDIENT : RecipeStage.GET_FOOD;
            }

            currentRecipeStep++;
        }

        // After recipe has been all made, go to get the completed food
        if (currentIngredientStage == RecipeStage.GET_FOOD) {
            yield return goToPosition(cookingStation.transform.position);
            servedFood = cookingStation.takeCompletedMeal();
            mealBox.SetActive(true);
            currentIngredientStage = RecipeStage.BRING_FOOD_TO_CUSTOMER;
        }

        // Take food to order window and get the newest recipe from the order window
        if (currentIngredientStage == RecipeStage.BRING_FOOD_TO_CUSTOMER) {
            yield return goToPosition(orderWindow.transform.position);
            orderWindow.serveFood(servedFood);
            mealSprite.sprite = targetRecipe.foodSprite;
            mealBox.SetActive(false);
        }

        // Reset recipe steps
        targetRecipe = orderWindow.getCurrentRecipe();
        currentRecipeStep = 0;
        currentIngredientStage = RecipeStage.GO_TO_INGREDIENT;
    }

    // Main IEnumerator to do aggressive action
    protected virtual IEnumerator doAggressiveAction(float aggressiveMoveSpeed) {
        yield return aggressiveAction.doAggressiveAction(chefSensing, lastSeenTarget, aggressiveMoveSpeed);

        // If AI can't see user anymore, act confused
        if (didHitRat) {
            aggressive = false;
            didHitRat = false;
            levelInfo.onChefChaseEnd();
        }
        else if (chefSensing.currentRatTarget == null)
        {
            audioManager.playChefLost();
            aggressive = false;
            levelInfo.onChefChaseEnd();
            yield return actConfused();
        }
    }

    // Main IEnumerator sequence to act confused until either the confusion timer runs out or chef sees the player again
    private IEnumerator actConfused() {
        float confusionTimer = 0.0f;
        WaitForEndOfFrame waitFrame = new WaitForEndOfFrame();
        navMeshAgent.enabled = false;

        while (confusionTimer < confusionDuration && chefSensing.currentRatTarget == null) {
            yield return waitFrame;
            confusionTimer += Time.deltaTime;
        }

        navMeshAgent.enabled = true;

        // Instantiate cage if angered
        if (angered) {
            trapManager.spawnTrap(onRatCageSetOff);
        }
    }

    // Main sequence when chef has spot rat
    private IEnumerator spotRat() {
        navMeshAgent.enabled = false;
        aggressive = true;

        // Face target
        Transform lockedTarget = chefSensing.currentRatTarget;
        lastSeenTarget = lockedTarget.position;
        Vector3 flattenTarget = new Vector3(lockedTarget.position.x, 0, lockedTarget.position.z);
        Vector3 flattenPosition = new Vector3(transform.position.x, 0, transform.position.z);
        transform.forward = (flattenTarget - flattenPosition).normalized;

        // Clear up thought bubble
        thoughtBubble.clearUpThoughts();

        // Indicate event start
        levelInfo.onChefChaseStart();

        yield return new WaitForSeconds(0.3f);
        navMeshAgent.enabled = true;
        audioManager.playChefAlert();
        sensedTrap = null;

        if (angered) {
            yield return mainAngerLoop();
        } else {
            yield return mainIntelligenceLoop();
        }
    }


    // Main Sequence when chef is trying to solve an issue, as seen in highestPriorityIssue
    //  Pre: highestPriorityIssue != null
    private IEnumerator solveIssue(bool interrupted) {
        Debug.Assert(highestPriorityIssue != null);

        if (!interrupted) {
            navMeshAgent.enabled = false;
            faceHighPriorityIssue();
            audioManager.playChefAlert();
            yield return new WaitForSeconds(surprisedAtIssueDuration);
            navMeshAgent.enabled = true;
        }

        navMeshAgent.speed = chaseMovementSpeed;

        for (int i = 0; i < highestPriorityIssue.getTotalSequenceSteps(); i++) {
            SolutionType currentStep = highestPriorityIssue.getNthStep(i);
            yield return goToSolutionObject(currentStep);
            yield return goToPosition(highestPriorityIssue.transform.position);

            navMeshAgent.enabled = false;
            targetedSolution.playUsageSound();
            yield return new WaitForSeconds(targetedSolution.getDuration());
            navMeshAgent.enabled = true;

            if (i == highestPriorityIssue.getTotalSequenceSteps() - 1) {
                highestPriorityIssue.removeIssue();
                navMeshAgent.speed = passiveMovementSpeed;
            }

            yield return goToPosition(targetedSolution.getInitialLocation());

            dropSolutionObject(targetedSolution);
            targetedSolution = null;
        }

        // Once issue has been fully solved, destroy object and get the next highest priority issue sensed (if there is any)
        highestPriorityIssue = chefSensing.getHighestPriorityIssue();
    }


    // Private helper method to get forward to highestPriorityIssue
    private void faceHighPriorityIssue() {
        Vector3 flattenedIssuePos = new Vector3(highestPriorityIssue.transform.position.x, transform.position.y, highestPriorityIssue.transform.position.z);
        transform.forward = (flattenedIssuePos - transform.position).normalized;
    }


    // Main sequence for going to a targeted solution object
    private IEnumerator goToSolutionObject(SolutionType solutionType) {
        WaitForEndOfFrame waitFrame = new WaitForEndOfFrame();
        List<Vector3> initialSolutionPositions = getSortedSolutionPositions(levelInfo.getPositions(solutionType));

        int s = 0;

        targetedSolution = solutionSensor.getSolutionInRange(solutionType);
        targetedSolutionType = solutionType;
        sensingSolutions = true;
        thoughtBubble.thinkOfSolution(solutionType);

        // Main loop for trying to get a solution
        while (targetedSolution == null) {
            // if S is still in range get the current targeted solution
            if (s < initialSolutionPositions.Count) {
                targetedSolutionPosition = initialSolutionPositions[s];
                navMeshAgent.destination = targetedSolutionPosition;

                // make sure path has been fully processed before running
                while (navMeshAgent.pathPending) {
                    yield return waitFrame;
                }

                // Keep going on path until navMeshAgent.remainingDistance is less than a threshold
                while (targetedSolution == null && navMeshAgent.remainingDistance > navDistance) {
                    yield return waitFrame;
                }

                // Act confused until either the confused timer went out or the targetSolution was found
                float timer = 0f;
                while (targetedSolution == null && timer < noSolutionFoundDuration) {
                    yield return waitFrame;
                    timer += Time.deltaTime;
                }

            // Else, go rummage through the closet, GUARANTEED TO GET A SOLUTION OBJECT
            } else {
                targetedSolutionPosition = closet.transform.position;
                yield return goToPosition(targetedSolutionPosition);

                chefClosetEvent.Invoke();
                yield return new WaitForSeconds(3.0f);
                closet.spawnSolutionObject(solutionType);

                while (targetedSolution == null)
                    yield return waitFrame;
            }

            // set up for geting the next targeted solution
            s++;
        }

        thoughtBubble.clearUpThoughts();
        grabSolutionObject(targetedSolution);
        sensingSolutions = false;
        
    }


    // Private helper method to get a new list and sort them based on vector3 position
    //  Pre: the solution position is not null
    //  Post: returns a copy of the list with the positions sorted based on who's closest to this AI
    private List<Vector3> getSortedSolutionPositions(List<Vector3> solutionPositions) {
        // Make a new list and copy contents of position to the new list
        List<Vector3> sortedPositions = new List<Vector3>();

        for(int i = 0; i < solutionPositions.Count; i++) {
            sortedPositions.Add(solutionPositions[i]);
        }

        // Sort the list using the comparater, uses quicksort O(NlogN)
        sortedPositions.Sort(solutionPosComparer);
        return sortedPositions;
    }


    // Main sequence to go to issue location (hazards will not change in location)
    protected IEnumerator goToPosition(Vector3 position) {
        navMeshAgent.enabled = true;
        WaitForEndOfFrame waitFrame = new WaitForEndOfFrame();
        yield return waitFrame;
        navMeshAgent.destination = position;

        // Make sure the path has been fully processed before running
        while (navMeshAgent.pathPending) {
            yield return waitFrame;
        }

        // Go towards the highest priority issue
        while(navMeshAgent.remainingDistance > navDistance) {
            yield return waitFrame;
        }

    }

    
    // Main event handler method when an issue object has been spotted by the sensor
    public void onIssueSpotted(IssueObject newIssue) {
        if ((highestPriorityIssue == null || newIssue.getPriority() > highestPriorityIssue.getPriority()) && !angered) {
            newIssue.isBeingDealtWith = true;

            if (highestPriorityIssue != null) {
                highestPriorityIssue.isBeingDealtWith = false;
            }

            if (aggressive) {
                aggressive = false;
                levelInfo.onChefChaseEnd();
            }

            highestPriorityIssue = newIssue;

            StopAllCoroutines();

            // Reset animations and state
            aggressiveAction.cancelAggressiveAction();
            thoughtBubble.clearUpThoughts();
            animator.SetBool("anticipating", false);
            animator.SetBool("attacking", false);

            StartCoroutine(mainIntelligenceLoop());
        }
    }

    // Main event handler when rat has been spotted
    //  Only interrupt if there is no highestPriorityIssue in mind
    public void onRatSpotted() {
        if (highestPriorityIssue == null && canSpotRat && !aggressive) {

            if (aggressive) {
                aggressiveAction.cancelAggressiveAction();
                levelInfo.onChefChaseEnd();
            }
            sensingSolutions = false;
            StopAllCoroutines();

            animator.SetBool("anticipating", false);
            animator.SetBool("attacking", false);
            
            StartCoroutine(spotRat());
        }
    }

    // Main event handler when targeted rat gets damaged
    //  When rat gets damaged, ignore the rat sequence and set a boolean flag to break the aggression sequence
    public void onRatDamaged() {
        canSpotRat = false;

        if (aggressive) {
            didHitRat = true;
        }

        levelInfo.StartCoroutine(ignoreRatSequence());

        if (willDeactivate) {
            // Disable aggressive behaviors manually
            aggressiveAction.cancelAggressiveAction();
            animator.SetBool("anticipating", false);
            animator.SetBool("attacking", false);
            aggressive = false;
            levelInfo.onChefChaseEnd();

            StopAllCoroutines();
            StartCoroutine(deactivateChefFromActiveBranch());
        }
    }

    // Main sequence handler for dealing with not sensing the rat when the rat is in the process of dying
    private IEnumerator ignoreRatSequence() {
        canSpotRat = false;
        yield return new WaitForSecondsRealtime(0.2f);
        canSpotRat = true;
    }

    // Main event handler method when a solution object is in range:
    //  Only take not of what solutions sensed 
    public void onSolutionSpotted(SolutionObject solutionObject) {
        if (sensingSolutions) {
            if (targetedSolutionType == solutionObject.solutionType && targetedSolution == null) {
                targetedSolution = solutionObject;
            }
        }
    }


    // Private helper method to get solution object
    private void grabSolutionObject(SolutionObject solution) {
        solution.transform.parent = chefSprite;
        solution.transform.localPosition = solutionHook;
        solution.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        solution.canChefGrab = false;

        GeneralInteractable solutionInteractable = solution.GetComponent<GeneralInteractable>();
        SpotShadow shadow = solution.GetComponentInChildren<SpotShadow>();

        if (solution.GetComponent<Ingredient>() != null) {
            solution.GetComponent<Ingredient>().isHeldByChef = true;
        }

        if (shadow != null) {
            shadow.disable();
        }

        if (solutionInteractable != null) {
            solutionInteractable.canBePickedUp = false;
        }
    }

    // Private helper method to drop a solution object to its initial position
    private void dropSolutionObject(SolutionObject solution) {
        solution.transform.parent = null;
        solution.canChefGrab = true;
        solution.transform.position = targetedSolution.getInitialLocation();
        solution.transform.rotation = Quaternion.identity;
        solution.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;

        GeneralInteractable solutionInteractable = solution.GetComponent<GeneralInteractable>();
        SpotShadow shadow = solution.GetComponentInChildren<SpotShadow>();

        if (solution.GetComponent<Ingredient>() != null) {
            solution.GetComponent<Ingredient>().isHeldByChef = false;
        }

        if (shadow != null) {
            shadow.enable();
        }

        if (solutionInteractable != null) {
            solutionInteractable.canBePickedUp = true;
        }
    }

    // Private helper method to drop the solution object in place
    private void dropSolutionObjectInPlace(SolutionObject solution) {
        solution.transform.parent = null;
        solution.canChefGrab = true;
        solution.transform.position = transform.position;
        solution.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;

        GeneralInteractable solutionInteractable = solution.GetComponent<GeneralInteractable>();
        SpotShadow shadow = solution.GetComponentInChildren<SpotShadow>();

        if (solution.GetComponent<Ingredient>() != null) {
            solution.GetComponent<Ingredient>().isHeldByChef = false;
        }

        if (shadow != null) {
            shadow.enable();
        }

        if (solutionInteractable != null) {
            solutionInteractable.canBePickedUp = true;
        }
    }

    // Main event handler method for when to-do list is done
    //  when to-do list is done, make the chef super angry
    public void onToDoListDone() {
        if (!angered) {
            // Set angered flag to true, will now ignore hazards
            angered = true;

            // If aggressive, disable that
            if (aggressive) {
                aggressiveAction.cancelAggressiveAction();
                levelInfo.onChefChaseEnd();
                animator.SetBool("anticipating", false);
                animator.SetBool("attacking", false);
            }

            // Stop all coroutine and their side effects
            aggressive = false;
            sensingSolutions = false;
            thoughtBubble.clearUpThoughts();
            highestPriorityIssue = null;

            if (targetedSolution != null) {
                dropSolutionObjectInPlace(targetedSolution);
            }

            mealBox.SetActive(false);
            StopAllCoroutines();
            inStartAngerSequence = true;

            if (gameObject.activeInHierarchy) {
                StartCoroutine(startAngerSequence());
            }
        }
    }

    // Public event handler method for when a rat trap has been set off
    public void onRatCageSetOff(Transform ratTrap) {
        if (angered && !aggressive) {
            sensedTrap = ratTrap;
            StopAllCoroutines();
            StartCoroutine(mainAngerLoop());
        }
    }

    // Main event handler method for poisoned meals
    public void onPoisonedMeal() {
        if (!angered) {
            mealPoisoned = true;

            if (!aggressive && highestPriorityIssue == null) {
                StopAllCoroutines();
                StartCoroutine(mainIntelligenceLoop());
            }
        }

    }

    // Main method to flip the sprite
    void Flip()
    {
        facingRight = !facingRight;
        characterSprite.flipX = !characterSprite.flipX;
    }

    // Main method to activate the chef
    public void activateChef() {
        gameObject.SetActive(true);
        willDeactivate = false;

        // Check if chef is literally in any state that's not passive first
        bool isNormallyPassive = highestPriorityIssue == null && !aggressive && !mealPoisoned;
        bool isAngrilyPassive = sensedTrap == null && !aggressive && !inStartAngerSequence;
        navMeshAgent.enabled = true;

        if (inStartAngerSequence) {
            StopAllCoroutines();
            
            // Method to set chef to angry mode without going through start sequence
            animator.GetComponent<SpriteRenderer>().color = angryColor;

            canSpotRat = true;
            aggressiveAction.makeAngry();
            inStartAngerSequence = false;

            if (chefSensing.currentRatTarget != null) {
                aggressive = true;
                StartCoroutine(spotRat());
            } else {
                StartCoroutine(mainAngerLoop());
            }

        } else if (angered && isAngrilyPassive) {
            didHitRat = false;

            StopAllCoroutines();
            StartCoroutine(mainAngerLoop());
        } else if (!angered && isNormallyPassive) {
            didHitRat = false;

            StopAllCoroutines();
            StartCoroutine(mainIntelligenceLoop());
        }
    }

    // Main method to deactivat ethe chef
    public void deactivateChef() {
        
        // Check if chef can just easily deactivate (In the most passive state)
        bool isNormallyPassive = !angered && highestPriorityIssue == null && !aggressive && !mealPoisoned;
        bool isAngrilyPassive = angered && sensedTrap == null && !aggressive && !inStartAngerSequence;

        if (isNormallyPassive || isAngrilyPassive) {
            StopAllCoroutines();
            gameObject.SetActive(false);
        } else {
            // If not, chef must easily deactivate near stock pot
            willDeactivate = true;
        }
    }

    // Private IEnumerator sequence to deactivate the chef from a nonpassive branch
    private IEnumerator deactivateChefFromActiveBranch() {
        navMeshAgent.enabled = true;

        // Go to specified position 
        WaitForEndOfFrame waitFrame = new WaitForEndOfFrame();
        yield return waitFrame;
        navMeshAgent.destination = cookingStation.transform.position;

        // Make sure the path has been fully processed before running OR deactivation sequence was cancelled
        while (willDeactivate && navMeshAgent.pathPending) {
            yield return waitFrame;
        }

        // Go towards the highest priority issue or deactivation sequence was cancelled
        while(willDeactivate && navMeshAgent.remainingDistance > navDistance) {
            yield return waitFrame;
        }
        
        if (willDeactivate) {
            StopAllCoroutines();
            gameObject.SetActive(false);
            willDeactivate = false;
        }

        didHitRat = false;
    }

    // Main method to lock the chef in place, doing angry animation
    public void lockChefInAnger() {
        StopAllCoroutines();

        navMeshAgent.enabled = false;
        animator.SetBool("anticipating", false);
        animator.SetBool("attacking", false);
        aggressiveAction.cancelAggressiveAction();
        animator.SetBool("angry", true);
    }

    // Main method to destroy the chef
    public void destroyChef() {
        Object.Destroy(gameObject);
    }

}
