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
            StartCoroutine(checkColliderStatus(collider));
            DestroyableObject destroyableObject = collider.GetComponent<DestroyableObject>();

            // Get destroyable parent
            DestroyableObject destroyableParent = null;
            if (collider.transform.parent != null) {
                destroyableParent = collider.transform.parent.GetComponent<DestroyableObject>();
            }

            // Listen to event if object has been destroyed
            if (destroyableObject != null) {
                destroyableObject.destroyObjectEvent.AddListener(onColliderDestroyed);
            } else if(destroyableParent != null) {
                destroyableParent.destroyObjectEvent.AddListener(onColliderDestroyed);
            }

            checkBreakablePillarOnEnter(collider);
        }
    }

    // If something goes out of trigger
    private void OnTriggerExit(Collider collider) {
        if (collider.tag == "Platform") {
            numCollidingObjects--;
            DestroyableObject destroyableObject = collider.GetComponent<DestroyableObject>();

            // Get destroyable parent
            DestroyableObject destroyableParent = null;
            if (collider.transform.parent != null) {
                destroyableParent = collider.transform.parent.GetComponent<DestroyableObject>();
            }

            // Connect variables
            if (destroyableObject != null) {
                destroyableObject.destroyObjectEvent.RemoveListener(onColliderDestroyed);
            } else if (destroyableParent != null) {
                destroyableParent.destroyObjectEvent.RemoveListener(onColliderDestroyed);
            }

            checkBreakablePillarOnExit(collider);
        }
    }

    // Private helper method to check BreakablePillar on enter
    private void checkBreakablePillarOnEnter(Collider collider) {
        RestaurantPillar pillar = collider.GetComponent<RestaurantPillar>();
        RestaurantPillar parentPillar = null;

        if (collider.transform.parent != null) {
            parentPillar = collider.transform.parent.GetComponent<RestaurantPillar>();
        }

        // Connect variables if possible
        if (pillar != null) {
            pillar.pillarDestroyedEvent.AddListener(onColliderDestroyed);
        } else if (parentPillar != null) {
            parentPillar.pillarActivatedEvent.AddListener(onColliderDestroyed);
        }
    }

    // Private helper method to check BreakablePillar on exit
    private void checkBreakablePillarOnExit(Collider collider) {
        RestaurantPillar pillar = collider.GetComponent<RestaurantPillar>();
        RestaurantPillar parentPillar = null;

        if (collider.transform.parent != null) {
            parentPillar = collider.transform.parent.GetComponent<RestaurantPillar>();
        }

        // Connect variables if possible
        if (pillar != null) {
            pillar.pillarDestroyedEvent.RemoveListener(onColliderDestroyed);
        } else if (parentPillar != null) {
            parentPillar.pillarActivatedEvent.RemoveListener(onColliderDestroyed);
        }
    }

    // Check collider status
    //  If collider has been destroyed after 2 framee and numCollidingObject didn't update, update it
    private IEnumerator checkColliderStatus(Collider collider) {
        int startingNumObjects = numCollidingObjects;

        for (int i = 0; i < 2; i++) {
            yield return null;
        }

        if (collider == null && startingNumObjects >= numCollidingObjects) {
            onColliderDestroyed();
        }
    }

    // Event handler for when object is destroyed
    private void onColliderDestroyed() {
        if (numCollidingObjects > 0) {
            numCollidingObjects--;
        }
    }

    // Public method to check if the sensor is blocked
    public bool isBlocked() {
        return numCollidingObjects > 0;
    }
}
