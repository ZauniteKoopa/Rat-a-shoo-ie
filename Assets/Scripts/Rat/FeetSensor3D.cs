﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FeetSensor3D : MonoBehaviour
{
    // Event handling
    private int numTouchingObjects = 0;
    public UnityEvent landingEvent;
    public UnityEvent leavingEvent;

    // On trigger enter 2D, check if object is a platform. If so, alert landing event
    void OnTriggerEnter(Collider collider) {
        if (collider.tag == "Platform") {
            if (numTouchingObjects == 0) {
                landingEvent.Invoke();
            }

            numTouchingObjects++;
            StartCoroutine(checkColliderStatus(collider));

            // Check if platform or its parent is destroyable
            DestroyableObject destroyable = collider.GetComponent<DestroyableObject>();

            DestroyableObject destroyableParent = null;
            if (collider.transform.parent != null) {
                destroyableParent = collider.transform.parent.GetComponent<DestroyableObject>();
            }

            if (destroyable != null) {
                destroyable.destroyObjectEvent.AddListener(onPlatformDestroyed);
            } else if (destroyableParent != null) {
                destroyableParent.destroyObjectEvent.AddListener(onPlatformDestroyed);
            }

            CageSensor cageSensor = collider.GetComponent<CageSensor>();
            if (cageSensor != null) {
                cageSensor.cagePlatformResetEvent.AddListener(onPlatformDestroyed);
            }

            checkBreakablePillarOnEnter(collider);
        }
    }

    // On trigger exit 2D, check if object is a platform, If so, alert falling event
    void OnTriggerExit(Collider collider) {
        if (collider.tag == "Platform") {
            numTouchingObjects--;

            if (numTouchingObjects == 0) {
                leavingEvent.Invoke();
            }

            // Check if platform or its parent is destroyable
            DestroyableObject destroyable = collider.GetComponent<DestroyableObject>();

            DestroyableObject destroyableParent = null;
            if (collider.transform.parent != null) {
                destroyableParent = collider.transform.parent.GetComponent<DestroyableObject>();
            }

            if (destroyable != null) {
                destroyable.destroyObjectEvent.RemoveListener(onPlatformDestroyed);
            } else if (destroyableParent != null) {
                destroyableParent.destroyObjectEvent.RemoveListener(onPlatformDestroyed);
            }

            CageSensor cageSensor = collider.GetComponent<CageSensor>();
            if (cageSensor != null) {
                cageSensor.cagePlatformResetEvent.RemoveListener(onPlatformDestroyed);
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
            pillar.pillarDestroyedEvent.AddListener(onPlatformDestroyed);
        } else if (parentPillar != null) {
            parentPillar.pillarActivatedEvent.AddListener(onPlatformDestroyed);
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
            pillar.pillarDestroyedEvent.RemoveListener(onPlatformDestroyed);
        } else if (parentPillar != null) {
            parentPillar.pillarActivatedEvent.RemoveListener(onPlatformDestroyed);
        }
    }

    // Check collider status
    //  If collider has been destroyed after 2 framee and numCollidingObject didn't update, update it
    private IEnumerator checkColliderStatus(Collider collider) {
        int startingNumObjects = numTouchingObjects;

        for (int i = 0; i < 2; i++) {
            yield return null;
        }

        if (collider == null && startingNumObjects >= numTouchingObjects) {
            onPlatformDestroyed();
        }
    }

    // Public event handler method for when object you are standing on is destroyed
    public void onPlatformDestroyed() {
        numTouchingObjects--;

        if (numTouchingObjects == 0) {
            leavingEvent.Invoke();
        }
    }
}
