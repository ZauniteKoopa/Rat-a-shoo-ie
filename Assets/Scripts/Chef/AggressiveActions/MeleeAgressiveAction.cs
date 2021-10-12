using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MeleeAgressiveAction : AbstractAggressiveChefAction
{
    [SerializeField]
    private GameObject chefHitbox = null;
    [SerializeField]
    private float attackingRange = 5f;
    [SerializeField]
    private float anticipationTime = 0.65f;
    [SerializeField]
    private float attackingTime = 0.3f;

    // Main override method to do aggressive action
    public override IEnumerator doAggressiveAction(ChefSight chefSensing, Vector3 lastSeenTarget, float chaseMovementSpeed) {
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
            yield return attackRat(chefSensing);
        }
    }

    // Main method to attack the rat
    private IEnumerator attackRat(ChefSight chefSensing) {
        navMeshAgent.enabled = false;
        Transform lockedTarget = chefSensing.currentRatTarget;

        // Face target
        Vector3 flattenTarget = new Vector3(lockedTarget.position.x, 0, lockedTarget.position.z);
        Vector3 flattenPosition = new Vector3(transform.position.x, 0, transform.position.z);
        transform.forward = (flattenTarget - flattenPosition).normalized;

        animator.SetBool("anticipating", true);
        yield return new WaitForSeconds(anticipationTime);

        // Activate hit box and attack
        audioManager.playChefAttack();
        chefHitbox.SetActive(true);
        animator.SetBool("anticipating", false);
        animator.SetBool("attacking", true);

        yield return new WaitForSeconds(attackingTime);

        // Disable attack and go back to normal
        chefHitbox.SetActive(false);
        navMeshAgent.enabled = true;
        animator.SetBool("attacking", false);
    }

    // Main method to cancel aggressive action based off of the behavior tree
    public override void cancelAggressiveAction() {
        chefHitbox.SetActive(false);
    }

}
