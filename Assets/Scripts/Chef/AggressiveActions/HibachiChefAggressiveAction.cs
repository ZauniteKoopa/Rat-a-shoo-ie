using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HibachiChefAggressiveAction : AbstractAggressiveChefAction
{
    // Melee attack
    [Header("Melee Attack")]
    [SerializeField]
    private GameObject anticipationBox = null;
    [SerializeField]
    private GameObject meleeHitbox = null;
    [SerializeField]
    private float meleeAttackingRange = 1f;
    [SerializeField]
    private float meleeAnticipationTime = 0.65f;
    [SerializeField]
    private float angryMeleeAnticipationTime = 0.4f;
    [SerializeField]
    private float meleeAttackingTime = 0.3f;
    [SerializeField]
    public GameObject explosion;

    // Ranged attack
    [Header("Ranged explosive attack")]
    public LayerMask aimCollisionLayerMask;
    [SerializeField]
    private float rangedAttackingRange = 4.0f;
    [SerializeField]
    private float rangedAnticipationTime = 0.5f;
    [SerializeField]
    private float rangedAttackTime = 0.2f;
    [SerializeField]
    private Transform chefEye = null;
    [SerializeField]
    private Transform projectilePrefab = null;
    [SerializeField]
    private float rangedAttackCooldown = 2.0f;
    [SerializeField]
    private float rangedAngryAttackCooldown = 1.5f;
    private float rangedAttackTimer = 0.0f;
    private float MAX_LOCAL_HEIGHT = 0.45f;
    private float MIN_LOCAL_HEIGHT = -0.32f;

    // General movement
    [Header("General movement")]
    [SerializeField]
    private float angerAcceleration = 10f;
    private bool angered = false;


    // Main override method to do aggressive action
    public override IEnumerator doAggressiveAction(ChefSight chefSensing, Vector3 lastSeenTarget, float chaseMovementSpeed) {
        WaitForEndOfFrame waitFrame = new WaitForEndOfFrame();
        navMeshAgent.destination = (chefSensing.currentRatTarget != null) ? chefSensing.currentRatTarget.position : lastSeenTarget;
        navMeshAgent.speed = chaseMovementSpeed;
        float currentRangedCooldown = (angered) ? rangedAngryAttackCooldown : rangedAttackCooldown;

        // Make sure the path has been fully processed before running
        while (navMeshAgent.pathPending) {
            yield return waitFrame;
        }

        // Chase the player: if player out of sight, go to the last position chef saw the player
        while (navMeshAgent.remainingDistance > meleeAttackingRange) {
            yield return waitFrame;
            rangedAttackTimer += Time.deltaTime;

            // Check if you can fire. If you can, fire a projectile
            bool sensingRat = chefSensing.currentRatTarget != null && !chefSensing.currentRatTarget.GetComponent<RatController3D>().respawning;
            bool canMelee = sensingRat && navMeshAgent.remainingDistance <= meleeAttackingRange;
            bool canFire = sensingRat && rangedAttackTimer > currentRangedCooldown && navMeshAgent.remainingDistance <= rangedAttackingRange;
            if (canFire && !canMelee) {
                yield return fireProjectile(chefSensing.currentRatTarget.position);
                rangedAttackTimer = 0.0f;
            }

            // Process path again and check distance
            if (chefSensing.currentRatTarget != null) {
                navMeshAgent.destination = chefSensing.currentRatTarget.position;
            }

            while (navMeshAgent.pathPending) {
                yield return waitFrame;
                rangedAttackTimer += Time.deltaTime;
            }
        }

        // Attack the player if in range, once in this mode, locked in this mode
        bool senseRat = chefSensing.currentRatTarget != null && !chefSensing.currentRatTarget.GetComponent<RatController3D>().respawning;
        if (senseRat && navMeshAgent.remainingDistance <= meleeAttackingRange) {
            yield return attackRat(chefSensing);
        }
    }


    // Main method to fire projectile
    private IEnumerator fireProjectile(Vector3 ratTarget) {
        navMeshAgent.enabled = false;

        animator.SetBool("ranged", true);
        animator.SetBool("anticipating", true);
        yield return new WaitForSeconds(rangedAnticipationTime);

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
        curProjectile.GetComponent<RangedExplosiveProjectile>().setHookDistance(Vector3.Distance(projectileSpawnPos, localRatPos) + 2f);

        animator.SetBool("anticipating", false);
        animator.SetBool("attacking", true);
        yield return new WaitForSeconds(rangedAttackTime);

        animator.SetBool("attacking", false);
        navMeshAgent.enabled = true;
    }


    // Main method to attack the rat
    private IEnumerator attackRat(ChefSight chefSensing) {
        navMeshAgent.enabled = false;
        Transform lockedTarget = chefSensing.currentRatTarget;

        // Face target
        Vector3 flattenTarget = new Vector3(lockedTarget.position.x, 0, lockedTarget.position.z);
        Vector3 flattenPosition = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 flattenDir = (flattenTarget - flattenPosition).normalized;
        if (flattenDir != Vector3.zero) {
            transform.forward = flattenDir;
        }

        animator.SetBool("ranged", false);
        animator.SetBool("anticipating", true);
        anticipationBox.SetActive(true);
        float currentmeleeAnticipationTime = (angered) ? angryMeleeAnticipationTime : meleeAnticipationTime;
        yield return new WaitForSeconds(currentmeleeAnticipationTime);

        // Activate hit box and attack
        audioManager.playChefAttack();
        anticipationBox.SetActive(false);
        meleeHitbox.SetActive(true);
        animator.SetBool("anticipating", false);
        animator.SetBool("attacking", true);
        audioManager.playWeaponAttack();


        // TODO Get the splash to show up, this is jank
        //var expl = Instantiate(explosion, new Vector3(transform.position.x, 0, transform.position.z + 10), Quaternion.identity);
        //Destroy(expl, 3); 

        yield return new WaitForSeconds(meleeAttackingTime);

        // Disable attack and go back to normal
        meleeHitbox.SetActive(false);
        navMeshAgent.enabled = true;
        animator.SetBool("attacking", false);
    }

    // Main method to cancel aggressive action based off of the behavior tree
    public override void cancelAggressiveAction() {
        anticipationBox.SetActive(false);
        meleeHitbox.SetActive(false);
    }

    // Main method to make action more scarier
    public override void makeAngry() {
        navMeshAgent.acceleration = angerAcceleration;
        angered = true;
    }
}
