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
        }
    }

    // On trigger exit 2D, check if object is a platform, If so, alert falling event
    void OnTriggerExit(Collider collider) {
        if (collider.tag == "Platform") {
            numTouchingObjects--;

            if (numTouchingObjects == 0) {
                leavingEvent.Invoke();
            }
        }
    }
}
