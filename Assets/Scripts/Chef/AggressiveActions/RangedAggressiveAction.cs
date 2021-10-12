using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedAggressiveAction : AbstractAggressiveChefAction
{
    // Collision mask layer
    public LayerMask aimCollisionLayerMask;
    private float attackTimer = 2.0f;
    [SerializeField]
    private float attackingRange = 3.0f;
    [SerializeField]
    private float anticipationTime = 0.5f;
    [SerializeField]
    private float attackTime = 0.2f;

    // Main sequence to do aggressive actions
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
                attackTimer += Time.deltaTime;
            }

            yield return waitFrame;
            attackTimer += Time.deltaTime;


            if (attackTimer >= 2.0f && chefSensing.currentRatTarget != null) {

                RaycastHit hit;
                Vector3 rayCastDir = chefSensing.currentRatTarget.transform.position - transform.position;

                if (!Physics.Raycast(transform.position, rayCastDir.normalized, out hit, rayCastDir.magnitude, aimCollisionLayerMask)) {
                    yield return throwProjectile(chefSensing.currentRatTarget.transform.position);
                    attackTimer = 0.0f;
                }
            }
        }
    }

    // Main method to throw a projectile
    public IEnumerator throwProjectile(Vector3 ratTarget) {
        yield return new WaitForSeconds(anticipationTime);
        Debug.Log("throw projectile");
        yield return new WaitForSeconds(attackTime);
    }

    // Main method to get rid of any lingering side effects
    public override void cancelAggressiveAction() {}
}
