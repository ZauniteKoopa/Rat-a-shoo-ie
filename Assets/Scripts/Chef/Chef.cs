﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions;

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
    PLACE_INGREDIENT_BACK
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

    // Serialized variables for attacking and chasing the rat
    [Header("Aggression")]
    [SerializeField]
    private GameObject chefHitbox = null;
    [SerializeField]
    private float attackingRange = 5f;
    [SerializeField]
    private float chaseMovementSpeed = 6f;
    [SerializeField]
    private float anticipationTime = 0.65f;
    [SerializeField]
    private float attackingTime = 0.3f;
    private bool aggressive = false;
    private Vector3 lastSeenTarget = Vector3.zero;

    // Variables for issue management
    [Header("IssueHandling")]
    [SerializeField]
    private LevelInfo levelInfo = null;
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
    private IssueObject highestPriorityIssue = null;

    // Recipe handling
    private Recipe targetRecipe = null;
    private int currentRecipeStep = 0;
    private RecipeStage currentIngredientStage = RecipeStage.GO_TO_INGREDIENT;
    [SerializeField]
    private Transform cookingStation = null;

    // Variables for getting solutions
    private bool sensingSolutions = false;
    private SolutionObject targetedSolution = null;
    private SolutionType targetedSolutionType = SolutionType.FIRE_EXTINGUISHER;
    private Vector3 targetedSolutionPosition = Vector3.zero;
    private SolutionPosComparer solutionPosComparer;

    // Variables for animation
    [Header("Animation")]
    [SerializeField]
    private Animator animator = null;

    // Testing variables
    private Color normalColor;
    [Header("Testing colors")]
    [SerializeField]
    private Color anticipationColor = Color.magenta;
    [SerializeField]
    private Color attackColor = Color.red;

    // Audio variables
    private ChefAudioManager audioManager;


    // Start is called before the first frame update
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        normalColor = meshRenderer.material.color;
        navMeshAgent = GetComponent<NavMeshAgent>();
        audioManager = GetComponent<ChefAudioManager>();
        solutionPosComparer = new SolutionPosComparer(transform);

        thoughtBubble.mainLevel = levelInfo;
        targetRecipe = levelInfo.pickRandomMenuItem();

        chefSensing.issueEnterEvent.AddListener(onIssueSpotted);
        solutionSensor.solutionSensedEvent.AddListener(onSolutionSpotted);
        StartCoroutine(mainIntelligenceLoop());
    }

    private void Update()
    {
        animator.SetFloat("movementspeed", navMeshAgent.velocity.magnitude/navMeshAgent.speed);
        animator.SetBool("aggro", aggressive);

        int lookDirection = WorldSprite.getSpriteLookDirectionTest(transform.forward);
        animator.SetFloat("direction", lookDirection);
    }

    // Main intelligence loop
    private IEnumerator mainIntelligenceLoop() {
        bool interrupted = false;

        // If you were interrupted by a higher priority issue and you were holding onto a solutionObject for a hazard, put the solution object back
        if (targetedSolution != null && highestPriorityIssue != null) {
            navMeshAgent.enabled = false;
            faceHighPriorityIssue();
            audioManager.playChefAlert();
            yield return new WaitForSeconds(surprisedAtIssueDuration);
            navMeshAgent.enabled = true;

            navMeshAgent.speed = chaseMovementSpeed;
            yield return goToPosition(targetedSolution.getInitialLocation());
            dropSolutionObject(targetedSolution);
            targetedSolution = null;

            interrupted = true;
        }

        // Main loop
        while (true) {

            if (highestPriorityIssue != null) {
                yield return solveIssue(interrupted);
                interrupted = false;
            }
            else if (!aggressive)
            {
                yield return finishRecipe();
            }
            else
            {
                yield return doAggressiveAction();
            }
        }
    }

    // Main sequence to do passive recipe gathering
    private IEnumerator finishRecipe() {
        // Force chef to move at passive speed
        navMeshAgent.speed = passiveMovementSpeed;

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
                yield return goToPosition(cookingStation.position);

                navMeshAgent.enabled = false;
                yield return new WaitForSeconds(targetedSolution.getDuration());
                navMeshAgent.enabled = true;

                currentIngredientStage = RecipeStage.PLACE_INGREDIENT_BACK;
            }

            // Place ingredient back if chef still has ingredient
            if (currentIngredientStage == RecipeStage.PLACE_INGREDIENT_BACK) {
                if (targetedSolution != null) {
                    yield return goToPosition(targetedSolution.getInitialLocation());
                    dropSolutionObject(targetedSolution);
                    targetedSolution = null;
                }

                currentIngredientStage = RecipeStage.GO_TO_INGREDIENT;
            }

            currentRecipeStep++;
        }

        // Get the newest recipe
        targetRecipe = levelInfo.pickRandomMenuItem();
        currentRecipeStep = 0;
    }

    // Main IEnumerator to do aggressive action
    private IEnumerator doAggressiveAction() {
        WaitForEndOfFrame waitFrame = new WaitForEndOfFrame();
        navMeshAgent.destination = (chefSensing.currentRatTarget != null) ? chefSensing.currentRatTarget.position : lastSeenTarget;
        navMeshAgent.speed = chaseMovementSpeed;

        // Make sure the path has been fully processed before running
        while (navMeshAgent.pathPending) {
            yield return waitFrame;
        }

        // Chase the player: if player out of sight, go to the last position chef saw the player
        while (navMeshAgent.remainingDistance > attackingRange) {
            if (chefSensing.currentRatTarget != null) {
                navMeshAgent.destination = chefSensing.currentRatTarget.position;
            }

            // process path
            while (navMeshAgent.pathPending) {
                yield return waitFrame;
            }

            yield return waitFrame;
        }

        // Attack the player if in range, once in this mode, locked in this mode
        if (chefSensing.currentRatTarget != null && navMeshAgent.remainingDistance <= attackingRange) {
            yield return attackRat();
        }

        // If AI can't see user anymore, act confused
        if (chefSensing.currentRatTarget == null) {
            audioManager.playChefLost();
            aggressive = false;
            levelInfo.onChefChaseEnd();
            yield return actConfused();
        }
    }

    // Main IEnumerator to attackRat
    private IEnumerator attackRat() {
        navMeshAgent.enabled = false;
        Transform lockedTarget = chefSensing.currentRatTarget;

        // Face target
        Vector3 flattenTarget = new Vector3(lockedTarget.position.x, 0, lockedTarget.position.z);
        Vector3 flattenPosition = new Vector3(transform.position.x, 0, transform.position.z);
        transform.forward = (flattenTarget - flattenPosition).normalized;

        meshRenderer.material.color = anticipationColor;
        yield return new WaitForSeconds(anticipationTime);

        // Activate hit box and attack
        audioManager.playChefAttack();
        chefHitbox.SetActive(true);
        meshRenderer.material.color = attackColor;

        yield return new WaitForSeconds(attackingTime);

        // Disable attack and go back to normal
        chefHitbox.SetActive(false);
        meshRenderer.material.color = normalColor;
        navMeshAgent.enabled = true;
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

        yield return mainIntelligenceLoop();
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
            yield return new WaitForSeconds(targetedSolution.getDuration());
            navMeshAgent.enabled = true;

            if (i == highestPriorityIssue.getTotalSequenceSteps() - 1) {
                Object.Destroy(highestPriorityIssue.gameObject);
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
            // get the current targeted solution
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
    private IEnumerator goToPosition(Vector3 position) {
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
        if (highestPriorityIssue == null || newIssue.getPriority() > highestPriorityIssue.getPriority()) {
            newIssue.isBeingDealtWith = true;

            if (highestPriorityIssue != null) {
                highestPriorityIssue.isBeingDealtWith = false;
            }

            if (aggressive) {
                aggressive = false;
                levelInfo.onChefChaseEnd();
            }

            highestPriorityIssue = newIssue;

            Debug.Log("New high priority issue in mind: " +  newIssue);
            StopAllCoroutines();
            chefHitbox.SetActive(false);
            thoughtBubble.clearUpThoughts();
            StartCoroutine(mainIntelligenceLoop());
        }
    }

    // Main event handler when rat has been spotted
    //  Only interrupt if there is no highestPriorityIssue in mind
    public void onRatSpotted() {
        if (highestPriorityIssue == null) {
            StopAllCoroutines();
            StartCoroutine(spotRat());
        }
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
    }

    // Private helper method to drop a solution object to its initial position
    private void dropSolutionObject(SolutionObject solution) {
        solution.transform.parent = null;
        solution.transform.position = targetedSolution.getInitialLocation();
        solution.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
    }

}
