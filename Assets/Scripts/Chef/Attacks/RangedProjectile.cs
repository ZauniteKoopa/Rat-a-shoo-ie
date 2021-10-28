using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedProjectile : MonoBehaviour
{
    protected Vector3 projectileDirection = Vector3.zero;
    [SerializeField]
    protected float projectileSpeed = 6.0f;
    [SerializeField]
    protected float rotationSpeed = 50.0f;
    [SerializeField]
    protected Transform spriteTransform = null;

    // Main method to move the object
    private void FixedUpdate() {
        moveProjectile();
    }

    // Method to move the projectile in the air
    protected virtual void moveProjectile() {
        Vector3 translateVector = Time.fixedDeltaTime * projectileSpeed * projectileDirection;
        transform.Translate(translateVector);

        Vector3 newEulerAngles = spriteTransform.eulerAngles + (Vector3.forward * rotationSpeed * Time.fixedDeltaTime);
        spriteTransform.eulerAngles = newEulerAngles;
    }

    // Method for player collision
    private void OnTriggerEnter(Collider collider) {
        if (collider.tag == "Platform") {
            executeAfterEffects(false);
        } else if (collider.GetComponent<RatController3D>() != null) {
            collider.GetComponent<RatController3D>().takeDamage();
            executeAfterEffects(true);
        }
    }

    // Method to execute after effects when object finally lands on the ground
    protected virtual void executeAfterEffects(bool hitPlayer) {
        Object.Destroy(gameObject);
    }
    
    // Method to set projectile direction
    public void setDirection(Vector3 dir) {
        projectileDirection = dir.normalized;
    }
}
