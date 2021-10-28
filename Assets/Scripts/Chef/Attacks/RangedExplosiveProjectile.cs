using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedExplosiveProjectile : RangedProjectile
{
    // Boolean flags
    private bool bombTimerActivated = false;
    private bool hookEnlarged = false;

    // Serialized fields
    [SerializeField]
    private GameObject anticipationZone = null;
    [SerializeField]
    private GameObject explosionZone = null;
    [SerializeField]
    private float explosiveAnticipationTime = 1.5f;
    [SerializeField]
    private float explosiveAttackTime = 0.3f;
    [SerializeField]
    private Color startAnticipationColor = Color.black;
    [SerializeField]
    private Color endAnticipationColor = Color.black;
    public LayerMask hookLayer;
    private float curDistance = 0.0f;
    private float hookedDistance = 0.0f;


    // Main method to move the projectile
    protected override void moveProjectile() {
        if (!bombTimerActivated) {
            Vector3 translateVector = Time.fixedDeltaTime * projectileSpeed * projectileDirection;
            transform.Translate(translateVector);

            // At a certain distance, hook to the ground below
            if (!hookEnlarged) {
                curDistance += translateVector.magnitude;

                if (curDistance >= hookedDistance) {
                    hookEnlarged = true;
                }
            } else {
                RaycastHit hit;

                if (Physics.Raycast(transform.position, Vector3.down, out hit, 3f, hookLayer)) {
                    executeAfterEffects(false);
                }
            }
        }
    }

    // Main method to execute the after effects
    protected override void executeAfterEffects(bool hitPlayer) {
        if (hitPlayer) {
            Object.Destroy(gameObject);
        } else  {
            bombTimerActivated = true;
            GetComponent<Collider>().enabled = false;
            StartCoroutine(bombSequence());
        }
    }

    // IEnumerator to start bomb sequence
    private IEnumerator bombSequence() {
        // Set anticipation portion
        anticipationZone.SetActive(true);
        MeshRenderer anticipationRender = anticipationZone.GetComponent<MeshRenderer>();
        anticipationRender.material.color = startAnticipationColor;

        // Anticipation timer
        float timer = 0.0f;
        WaitForEndOfFrame waitFrame = new WaitForEndOfFrame();

        while (timer <= explosiveAnticipationTime) {
            yield return waitFrame;
            timer += Time.deltaTime;

            anticipationRender.material.color = Color.Lerp(startAnticipationColor, endAnticipationColor, timer / explosiveAnticipationTime);
        }

        // Set explosive and then destroy object
        spriteTransform.GetComponent<SpriteRenderer>().enabled = false;
        anticipationRender.enabled = false;
        explosionZone.SetActive(true);

        yield return new WaitForSeconds(explosiveAttackTime);

        Object.Destroy(gameObject);
    }

    // Main method to set the hook distance
    public void setHookDistance(float hDist) {
        hookedDistance = hDist;
    }
    
}
