using System.Collections;
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

            // Check if platform is destroyable
            DestroyableObject destroyable = collider.GetComponent<DestroyableObject>();
            if (destroyable != null) {
                destroyable.destroyObjectEvent.AddListener(onPlatformDestroyed);
            }

            CageSensor cageSensor = collider.GetComponent<CageSensor>();
            if (cageSensor != null) {
                cageSensor.cagePlatformResetEvent.AddListener(onPlatformDestroyed);
            }
        }
    }

    // On trigger exit 2D, check if object is a platform, If so, alert falling event
    void OnTriggerExit(Collider collider) {
        if (collider.tag == "Platform") {
            numTouchingObjects--;

            if (numTouchingObjects == 0) {
                leavingEvent.Invoke();
            }

            // Check if platform is destroyable
            DestroyableObject destroyable = collider.GetComponent<DestroyableObject>();
            if (destroyable != null) {
                destroyable.destroyObjectEvent.RemoveListener(onPlatformDestroyed);
            }

            CageSensor cageSensor = collider.GetComponent<CageSensor>();
            if (cageSensor != null) {
                cageSensor.cagePlatformResetEvent.RemoveListener(onPlatformDestroyed);
            }
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
