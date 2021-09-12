using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions;

public class Chef : MonoBehaviour
{
    // Variables for navigation
    private NavMeshAgent navMeshAgent;
    private MeshRenderer meshRenderer;
    private int waypointIndex = 0;

    // Serialized variables for navigation and speed
    [Header("Navigation")]
    [SerializeField]
    private List<Transform> waypoints = null;
    [SerializeField]
    private ChefSight chefSensing = null;
    [SerializeField]
    private ChefSolutionSensor solutionSensor = null;
    [SerializeField]
    private float passiveMovementSpeed = 3.5f;
    [SerializeField]
    private float confusionDuration = 0.75f;

    // Serialized variables for attacking and chasing the rat
    [Header("Aggression")]
    [SerializeField]
    private GameObject chefHitbox = null;
    [SerializeField]
    private float navDistance = 1f;
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
    private IssueObject highestPriorityIssue = null;

    // Variables for getting solutions
    private bool sensingSolutions = false;
    private SolutionObject targetedSolution = null;
    private SolutionType targetedSolutionType = SolutionType.FIRE_EXTINGUISHER;
    private Vector3 targetedSolutionPosition = Vector3.zero;

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
        Assert.IsTrue(waypoints != null && waypoints.Count > 0);
        meshRenderer = GetComponent<MeshRenderer>();
        normalColor = meshRenderer.material.color;
        navMeshAgent = GetComponent<NavMeshAgent>();
        audioManager = GetComponent<ChefAudioManager>();

        chefSensing.issueEnterEvent.AddListener(onIssueSpotted);
        solutionSensor.solutionSensedEvent.AddListener(onSolutionSpotted);
        StartCoroutine(mainIntelligenceLoop());
    }

    private void Update()
    {
        animator.SetFloat("movementspeed", navMeshAgent.velocity.magnitude/navMeshAgent.speed);
    }

    // Main intelligence loop
    private IEnumerator mainIntelligenceLoop() {
        while (true) {
            animator.SetBool("aggro", aggressive);

            if (highestPriorityIssue != null) {
                yield return solveIssue();
            }
            else if (!aggressive)
            {
                yield return moveToNextWaypoint();
                yield return doPassiveAction();
            }
            else
            {
                yield return doAggressiveAction();
            }
        }
    }

    // Main IEnumerator to move to waypoint
    private IEnumerator moveToNextWaypoint() {
        // Set up and update variables
        navMeshAgent.speed = passiveMovementSpeed;
        waypointIndex = (waypointIndex + 1) % waypoints.Count;
        Vector3 targetPosition = waypoints[waypointIndex].position;
        WaitForEndOfFrame waitFrame = new WaitForEndOfFrame();

        navMeshAgent.destination = targetPosition;

        // Modify vector so that its flatten in the x, z plane
        Vector3 flatTargetPosition = new Vector3(targetPosition.x, 0, targetPosition.z);
        Vector3 flatCurrentPosition = new Vector3(transform.position.x, 0, transform.position.z);

        // Main wait loop until agent gets to destination point
        while (Vector3.Distance(flatTargetPosition, flatCurrentPosition) > navDistance && chefSensing.currentRatTarget == null) {
            yield return waitFrame;
            flatCurrentPosition = new Vector3(transform.position.x, 0, transform.position.z);
        }

        // Do alert if spotted rat
        if (chefSensing.currentRatTarget != null) {
            yield return spotRat();
        }
    }

    // Main IEnumerator to do passive action (Probably stirring soup or something, but for now, just standing)
    private IEnumerator doPassiveAction() {
        //Debug.Log("wait on patrol point");
        float timer = 0f;
        WaitForEndOfFrame waitFrame = new WaitForEndOfFrame();
        
        while (timer < 1f && chefSensing.currentRatTarget == null) {
            yield return waitFrame;
            timer += Time.deltaTime;
        }

        // Do alert if spotted rat
        if (chefSensing.currentRatTarget != null) {
            yield return spotRat();
        }
    }

    // Main IEnumerator to do aggressive action
    private IEnumerator doAggressiveAction() {
        WaitForEndOfFrame waitFrame = new WaitForEndOfFrame();
        navMeshAgent.destination = lastSeenTarget;
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

        yield return new WaitForSeconds(0.3f);
        navMeshAgent.enabled = true;
        audioManager.playChefAlert();
    }

    // Main Sequence when chef is trying to solve an issue, as seen in highestPriorityIssue
    //  Pre: highestPriorityIssue != null
    private IEnumerator solveIssue() {
        Debug.Assert(highestPriorityIssue != null);
        navMeshAgent.enabled = false;
        yield return new WaitForSeconds(1.0f);
        navMeshAgent.enabled = true;

        for (int i = 0; i < highestPriorityIssue.getTotalSequenceSteps(); i++) {
            SolutionType currentStep = highestPriorityIssue.getNthStep(i);

            yield return goToSolutionObject(currentStep);
            yield return goToPosition(highestPriorityIssue.transform.position);

            navMeshAgent.enabled = false;
            yield return new WaitForSeconds(targetedSolution.getDuration());
            navMeshAgent.enabled = true;

            if (i == highestPriorityIssue.getTotalSequenceSteps() - 1) {
                Object.Destroy(highestPriorityIssue.gameObject);
            }

            yield return goToPosition(targetedSolutionPosition);

            targetedSolution.gameObject.SetActive(true);
            targetedSolution = null;
        }

        // Once issue has been fully solved, destroy object and get the next highest priority issue sensed (if there is any)
        highestPriorityIssue = chefSensing.getHighestPriorityIssue();
        Debug.Log("I solved an issue");

        // For each solution in the issueObject
        //      go to nearest solution location (interruptable)
        //          if nearest solution is not there, go to next one (assume that there is always one that is reachable)
        //      go to issue location    (interruptable)
        //      interact (duration in the solution)
    }

    // Main sequence for going to a targeted solution object
    private IEnumerator goToSolutionObject(SolutionType solutionType) {
        WaitForEndOfFrame waitFrame = new WaitForEndOfFrame();
        List<Vector3> initialSolutionPositions = levelInfo.getPositions(solutionType);
        Vector3 closestPoint = initialSolutionPositions[0]; //TO BE CHANGED
        targetedSolutionPosition = closestPoint;

        navMeshAgent.destination = closestPoint;
        navMeshAgent.speed = chaseMovementSpeed;
        sensingSolutions = true;

        // Make sure the path has been fully processed before running
        while (navMeshAgent.pathPending) {
            yield return waitFrame;
        }

        // Keep going until targetedSolution != null    ASSUMPTION: TARGET-SOLUTION POSITIONS ARE STATIC
        while (targetedSolution == null) {
            yield return waitFrame;
        }

        sensingSolutions = false;
        targetedSolution.gameObject.SetActive(false); // another assumption to changed
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
        while(navMeshAgent.remainingDistance > 1.0f) {
            yield return waitFrame;
        }

    }

    
    // Main event handler method when an issue object has been spotted by the sensor
    public void onIssueSpotted(IssueObject newIssue) {
        if (highestPriorityIssue == null || newIssue.getPriority() > highestPriorityIssue.getPriority()) {

            highestPriorityIssue = newIssue;
            Debug.Log("New high priority issue in mind: " +  newIssue);
            StopAllCoroutines();
            StartCoroutine(mainIntelligenceLoop());
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

}
