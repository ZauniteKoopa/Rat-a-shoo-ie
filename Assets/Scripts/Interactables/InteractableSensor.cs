using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ASSUMPTION: Sensor's parent is the interactable
public class InteractableSensor : MonoBehaviour
{
    // Variables
    private Transform interactable = null;
    
    // On awake
    void Awake() {
        interactable = transform.parent;
    }

    // If on trigger enter
    void OnTriggerEnter(Collider collider) {
        RatController3D ratPlayer = collider.transform.GetComponent<RatController3D>();

        if (ratPlayer != null) {
            ratPlayer.setTargetedInteractable(interactable);
        }
    }

    // On trigger exit
    void OnTriggerExit(Collider collider) {
        RatController3D ratPlayer = collider.transform.GetComponent<RatController3D>();

        if (ratPlayer != null) {
            ratPlayer.clearTargetedInteractable(interactable);
        }
    }
}
