using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TrapSensor : MonoBehaviour
{
    // Main Unity event when trap has sensed something
    public UnityEvent trapSensedEvent;
    private int numberPlayersSensed = 0;

    // If player enters trigger box, send out event
    private void OnTriggerEnter(Collider collider) {
        RatController3D ratPlayer = collider.GetComponent<RatController3D>();

        if (ratPlayer != null) {
            if (numberPlayersSensed == 0) {
                trapSensedEvent.Invoke();
            }

            numberPlayersSensed++;
        }
    }

    // If player exits the trigger box, update variables
    private void OnTriggerExit(Collider collider) {
        RatController3D ratPlayer = collider.GetComponent<RatController3D>();

        if (ratPlayer != null) {
            numberPlayersSensed--;
        }
    }

    // Variable to check if sensor is still being triggered
    public bool isSensingObject() {
        return numberPlayersSensed > 0;
    }
}
