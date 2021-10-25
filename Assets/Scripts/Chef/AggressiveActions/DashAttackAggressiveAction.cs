using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DashAttackAggressiveAction : AbstractAggressiveChefAction
{
    [SerializeField]
    private float dashAnticipation = 2f;
    [SerializeField]
    private float angryDashAnticipation = 0.4f;
    [SerializeField]
    private float recoilTime = 1.0f;
    [SerializeField]
    private float dashSpeed = 30f;
    [SerializeField]
    private float angryDashSpeed = 35f;
    [SerializeField]
    private float chaseDistance = 1.5f;
    [SerializeField]
    private float dashRange = 15f;
    [SerializeField]
    private float dashThroughDistance = 5f;
    [SerializeField]
    private float angerAcceleration = 10f;
    [SerializeField]
    private GameObject dashHitbox = null;
    [SerializeField]
    private GameObject slashHitbox = null;
    [SerializeField]
    private float slashTime = 0.25f;

    private Vector3 lockedTarget = Vector3.zero;
    private bool locked = false;
    private bool angered = false;

    // Reference variables: DashLineRender - goes from position 1 to position 0
    private Collider chefCollider;
    private LineRenderer dashLineRender;

    // Main method to do initial setup
    protected override void initialSetup() {
        chefCollider = GetComponent<Collider>();
        dashLineRender = GetComponent<LineRenderer>();
        dashLineRender.enabled = false;
    }

    // Main method to do aggressive action
    public override IEnumerator doAggressiveAction(ChefSight chefSensing, Vector3 lastSeenTarget, float chaseMovementSpeed) {
        WaitForEndOfFrame waitFrame = new WaitForEndOfFrame();
        navMeshAgent.destination = (chefSensing.currentRatTarget != null) ? chefSensing.currentRatTarget.position : lastSeenTarget;
        navMeshAgent.speed = chaseMovementSpeed;

        // Make sure the path has been fully processed before running
        while (navMeshAgent.pathPending) {
            yield return waitFrame;
        }

        // Chase rat until chef can't see rat
        bool canDash = isWithinDashRange(chefSensing);

        while (navMeshAgent.remainingDistance < chaseDistance && !canDash) {
            // Update destination if not null
            if (chefSensing.currentRatTarget != null) {
                navMeshAgent.destination = chefSensing.currentRatTarget.position;
            }

            // process path
            while (navMeshAgent.pathPending) {
                yield return waitFrame;
            }

            yield return waitFrame;
            canDash = isWithinDashRange(chefSensing);
        }

        // If dash is within range, dash
        if (canDash) {
            yield return executeDash(chefSensing);
        }
    }

    // Main IEnumerator to execute the dash
    private IEnumerator executeDash(ChefSight chefSensing) {
        // Have some time of anticipation. Target is not locked yet
        navMeshAgent.enabled = false;
        WaitForEndOfFrame waitFrame = new WaitForEndOfFrame();

        Transform ratTarget = chefSensing.currentRatTarget;
        lockedTarget = new Vector3(ratTarget.position.x, transform.position.y, ratTarget.position.z);
        lockedTarget = lockedTarget + (dashThroughDistance * (lockedTarget - transform.position).normalized);
        lockedTarget = getNearestValidNavMeshPosition(lockedTarget);

        // Face target
        transform.forward = (lockedTarget - transform.position).normalized;
        float currentAnticipation = (angered) ? angryDashAnticipation : dashAnticipation;
        animator.SetBool("anticipating", true);

        yield return new WaitForSeconds(currentAnticipation);

        // Once anticipation is done calculate the time
        locked = true;
        float maxDashTime = calculateDashTime();
        float timer = 0.0f;
        Vector3 originalPos = transform.position;
        chefCollider.enabled = false;

        // Set up animations / audio / dash lines
        dashLineRender.SetPosition(1, transform.position);
        dashLineRender.SetPosition(0, transform.position);
        dashLineRender.enabled = true;

        audioManager.playChefAttack();
        animator.SetBool("anticipating", false);
        animator.SetBool("attacking", true);
        dashHitbox.SetActive(true);

        // Dash sequence
        while (timer <= maxDashTime) {
            yield return waitFrame;
            timer += Time.deltaTime;
            float progress = timer / maxDashTime;

            transform.position = Vector3.Lerp(originalPos, lockedTarget, progress);
            dashLineRender.SetPosition(0, transform.position);
        }

        dashHitbox.SetActive(false);
        transform.position = lockedTarget;
        dashLineRender.enabled = false;

        // Slash sequence
        slashHitbox.SetActive(true);
        yield return new WaitForSeconds(slashTime);
        slashHitbox.SetActive(false);

        locked = false;
        chefCollider.enabled = true;
        animator.SetBool("attacking", false);

        // Do small recoil time before enabling movement
        yield return new WaitForSeconds(recoilTime);
        faceRat(ratTarget);
        navMeshAgent.enabled = true;
    }

    // Private IEnumerator to finish dash if the chef was locked into a dash initially
    private IEnumerator finishDash() {
        WaitForEndOfFrame waitFrame = new WaitForEndOfFrame();

        float maxDashTime = calculateDashTime();
        float timer = 0.0f;
        Vector3 originalPos = transform.position;
        dashHitbox.SetActive(true);

        // Dash sequence
        while (timer <= maxDashTime) {
            yield return waitFrame;
            timer += Time.deltaTime;
            float progress = timer / maxDashTime;

            transform.position = Vector3.Lerp(originalPos, lockedTarget, progress);
            dashLineRender.SetPosition(0, transform.position);
        }

        dashHitbox.SetActive(false);
        transform.position = lockedTarget;
        dashLineRender.enabled = false;

        // Slash sequence
        slashHitbox.SetActive(true);
        yield return new WaitForSeconds(slashTime);
        slashHitbox.SetActive(false);

        locked = false;
        chefCollider.enabled = true;
    }

    // Private helper method to check if the chef can dash at the player now
    private bool isWithinDashRange(ChefSight chefSensing) {
        if (chefSensing.currentRatTarget == null) {
            return false;
        }

        Vector3 flatRatPosition = new Vector3(chefSensing.currentRatTarget.position.x, 0f, chefSensing.currentRatTarget.position.z);
        Vector3 flatChefPosition = new Vector3(transform.position.x, 0f, transform.position.z);

        return Vector3.Distance(flatRatPosition, flatChefPosition) <= dashRange;
    }

    // Private helper method to face transform
    private void faceRat(Transform ratTarget) {
        Vector3 flatRatTarget = new Vector3(ratTarget.position.x, transform.position.y, ratTarget.position.z);
        transform.forward = (flatRatTarget - transform.position).normalized;
    }

    // Private helper method to calculate the time it takes to dash from chef position to lockedTarget
    private float calculateDashTime() {
        Vector3 flatRatPosition = new Vector3(lockedTarget.x, 0f, lockedTarget.z);
        Vector3 flatChefPosition = new Vector3(transform.position.x, 0f, transform.position.z);
        float distance = Vector3.Distance(flatRatPosition, flatChefPosition);
        float currentDashSpeed = (angered) ? angryDashSpeed : dashSpeed;

        return distance / currentDashSpeed;
    }

    // Private helper method to get the nearest valid position in the navmesh
    private Vector3 getNearestValidNavMeshPosition(Vector3 position) {
        NavMeshHit hit;

        if (NavMesh.SamplePosition(position, out hit, 50.0f, NavMesh.AllAreas)) {
            // Get the sample position and flatten it to match the height of the chef
            Vector3 navMeshPosition = hit.position;
            return new Vector3(navMeshPosition.x, transform.position.y, navMeshPosition.z);

        } else {
            // If no sample position found, abort attack
            return transform.position;
        }
    }


    // Main method to cancel the aggressive action and remove any side effects
    public override void cancelAggressiveAction() {

        // If cancelled when locked in a dash, make sure the dash finishes before going further
        if (locked) {
            StartCoroutine(finishDash());
        } else {
            // Else, make sure chefCollider is enabled
            chefCollider.enabled = true;
        }
    }

    // Main method to make the action more aggressive
    public override void makeAngry() {
        angered = true;
        navMeshAgent.acceleration = angerAcceleration;
    }
}
