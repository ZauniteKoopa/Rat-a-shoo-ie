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
    [SerializeField]
    private Transform chefEye = null;
    [SerializeField]
    private Transform projectilePrefab = null;
    [SerializeField]
    private float attackCooldown = 2.0f;
    [SerializeField]
    private float angryAttackCooldown = 1.5f;
    [SerializeField]
    private float angerAcceleration = 10f;
    private float MAX_LOCAL_HEIGHT = 0.45f;
    private float MIN_LOCAL_HEIGHT = -0.32f;
    private float randomVariance = 0.2f;
    private bool angered = false;

    // Main sequence to do aggressive actions
    public override IEnumerator doAggressiveAction(ChefSight chefSensing, Vector3 lastSeenTarget, float chaseMovementSpeed) {
        WaitForEndOfFrame waitFrame = new WaitForEndOfFrame();
        navMeshAgent.destination = (chefSensing.currentRatTarget != null) ? chefSensing.currentRatTarget.position : lastSeenTarget;
        navMeshAgent.speed = chaseMovementSpeed;

        // Make sure the path has been fully processed before running
        while (navMeshAgent.pathPending) {
            yield return waitFrame;
        }

        // Set up current cooldown
        float currentCooldown = (angered) ? angryAttackCooldown : attackCooldown;
        currentCooldown = Random.Range(currentCooldown - randomVariance, currentCooldown + randomVariance);

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

            if (attackTimer >= currentCooldown && chefSensing.currentRatTarget != null) {

                RaycastHit hit;
                Vector3 rayCastDir = chefSensing.currentRatTarget.transform.position - transform.position;

                if (!Physics.Raycast(chefEye.position, rayCastDir.normalized, out hit, rayCastDir.magnitude, aimCollisionLayerMask)) {
                    yield return throwProjectile(chefSensing.currentRatTarget.transform.position);
                    attackTimer = 0.0f;

                    // Update current cooldown
                    currentCooldown = (angered) ? angryAttackCooldown : attackCooldown;
                    currentCooldown = Random.Range(currentCooldown - randomVariance, currentCooldown + randomVariance);
                }
            }
        }
    }

    // Main method to throw a projectile
    private IEnumerator throwProjectile(Vector3 ratTarget) {
        navMeshAgent.enabled = false;

        animator.SetBool("anticipating", true);
        yield return new WaitForSeconds(anticipationTime);

        // Get rat specific height to throw shoe at
        Vector3 localRatPos = transform.InverseTransformPoint(ratTarget);
        float clampedHeight = Mathf.Clamp(localRatPos.y, MIN_LOCAL_HEIGHT, MAX_LOCAL_HEIGHT);
        localRatPos = new Vector3(localRatPos.x, clampedHeight, localRatPos.z);
        localRatPos = transform.TransformPoint(localRatPos);

        // Decide the spawn position
        Vector3 projectileSpawnPos = new Vector3(chefEye.position.x, localRatPos.y, chefEye.position.z);
        Transform curProjectile = Object.Instantiate(projectilePrefab, projectileSpawnPos, Quaternion.identity);
        audioManager.playChefAttack();

        // Decide the projectile direction by flatterning the y axis
        Vector3 projDir = new Vector3(ratTarget.x - chefEye.position.x, 0f, ratTarget.z - chefEye.position.z);
        curProjectile.GetComponent<RangedProjectile>().setDirection(projDir);

        animator.SetBool("anticipating", false);
        animator.SetBool("attacking", true);
        yield return new WaitForSeconds(attackTime);

        animator.SetBool("attacking", false);
        navMeshAgent.enabled = true;
    }

    // Main method to get rid of any lingering side effects
    public override void cancelAggressiveAction() {}

    // Main method to make the action more angry
    public override void makeAngry() {
        navMeshAgent.acceleration = angerAcceleration;
        angered = true;
    }
}
