using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedProjectile : MonoBehaviour
{
    private Vector3 projectileDirection = Vector3.zero;
    [SerializeField]
    private float projectileSpeed = 6.0f;
    [SerializeField]
    private float rotationSpeed = 50.0f;
    [SerializeField]
    private Transform spriteTransform = null;

    // Main method to move the object
    private void FixedUpdate() {

        Vector3 translateVector = Time.fixedDeltaTime * projectileSpeed * projectileDirection;
        transform.Translate(translateVector);

        Vector3 newEulerAngles = spriteTransform.eulerAngles + (Vector3.forward * rotationSpeed * Time.fixedDeltaTime);
        spriteTransform.eulerAngles = newEulerAngles;
    }

    // Method for player collision
    private void OnTriggerEnter(Collider collider) {
        if (collider.tag == "Platform") {
            DestroyableObject destroyObject = collider.GetComponent<DestroyableObject>();

            if (destroyObject != null) {
                destroyObject.destroyObject();
            }

            Object.Destroy(gameObject);
        } else if (collider.GetComponent<RatController3D>() != null) {
            collider.GetComponent<RatController3D>().takeDamage();
            Object.Destroy(gameObject);
        }
    }
    
    // Method to set projectile direction
    public void setDirection(Vector3 dir) {
        projectileDirection = dir.normalized;
    }
}
