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

    // Serialized variables
    [Header("Navigation")]
    [SerializeField]
    private List<Transform> waypoints = null;
    [SerializeField]
    private ChefSight chefSensing = null;
    [SerializeField]
    private float passiveMovementSpeed = 3.5f;
    [SerializeField]
    private float confusionDuration = 0.75f;

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

    private ChefAudioManager audioManager;


    // Start is called before the first frame update
    void Start()
    {
        Assert.IsTrue(waypoints != null && waypoints.Count > 0);
        meshRenderer = GetComponent<MeshRenderer>();
        normalColor = meshRenderer.material.color;
        navMeshAgent = GetComponent<NavMeshAgent>();
        audioManager = GetComponent<ChefAudioManager>();

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

            if (!aggressive) {
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
        while (Vector3.Distance(flatTargetPosition, flatCurrentPosition) > navDistance && chefSensing.currentTarget == null) {
            yield return waitFrame;
            flatCurrentPosition = new Vector3(transform.position.x, 0, transform.position.z);
        }

        // Do alert if spotted rat
        if (chefSensing.currentTarget != null) {
            yield return spotRat();
        }
    }

    // Main IEnumerator to do passive action (Probably stirring soup or something, but for now, just standing)
    private IEnumerator doPassiveAction() {

        float timer = 0f;
        WaitForEndOfFrame waitFrame = new WaitForEndOfFrame();
        
        while (timer < 3f && chefSensing.currentTarget == null) {
            yield return waitFrame;
            timer += Time.deltaTime;
        }

        // Do alert if spotted rat
        if (chefSensing.currentTarget != null) {
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
            if (chefSensing.currentTarget != null) {
                navMeshAgent.destination = chefSensing.currentTarget.position;
            }

            yield return waitFrame;
        }

        // Attack the player if in range, once in this mode, locked in this mode
        if (chefSensing.currentTarget != null && navMeshAgent.remainingDistance <= attackingRange) {
            yield return attackRat();
        }

        // If AI can't see user anymore, act confused
        if (chefSensing.currentTarget == null) {
            audioManager.playChefLost();
            aggressive = false;
            yield return actConfused();
        }
    }

    // Main IEnumerator to attackRat
    private IEnumerator attackRat() {
        navMeshAgent.enabled = false;
        Transform lockedTarget = chefSensing.currentTarget;

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

        while (confusionTimer < confusionDuration && chefSensing.currentTarget == null) {
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
        Transform lockedTarget = chefSensing.currentTarget;
        lastSeenTarget = lockedTarget.position;
        Vector3 flattenTarget = new Vector3(lockedTarget.position.x, 0, lockedTarget.position.z);
        Vector3 flattenPosition = new Vector3(transform.position.x, 0, transform.position.z);
        transform.forward = (flattenTarget - flattenPosition).normalized;

        yield return new WaitForSeconds(0.3f);
        navMeshAgent.enabled = true;
        audioManager.playChefAlert();
    }
}
