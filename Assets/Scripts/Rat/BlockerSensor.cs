using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Main class to check if blocked
public class BlockerSensor : MonoBehaviour
{
    private int numCollidingObjects = 0;

    // If something goes in trigger
    private void OnTriggerEnter(Collider collider) {
        if (collider.tag == "Platform") {
            numCollidingObjects++;
            DestroyableObject destroyableObject = collider.GetComponent<DestroyableObject>();

            if (destroyableObject != null) {
                destroyableObject.destroyObjectEvent.AddListener(onColliderDestroyed);
            }
        }
    }

    // If something goes out of trigger
    private void OnTriggerExit(Collider collider) {
        if (collider.tag == "Platform") {
            numCollidingObjects--;

            DestroyableObject destroyableObject = collider.GetComponent<DestroyableObject>();

            if (destroyableObject != null) {
                destroyableObject.destroyObjectEvent.RemoveListener(onColliderDestroyed);
            }
        }
    }

    // Event handler for when object is destroyed
    private void onColliderDestroyed() {
        numCollidingObjects--;
    }

    // Public method to check if the sensor is blocked
    public bool isBlocked() {
        return numCollidingObjects > 0;
    }
}
