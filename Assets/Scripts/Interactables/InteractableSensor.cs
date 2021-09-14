﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ASSUMPTION: Sensor's parent is the interactable
public class InteractableSensor : MonoBehaviour
{
    // HashSet of Interactables
    private HashSet<GeneralInteractable> inRangeInteractables;
    
    // On awake
    void Awake() {
        inRangeInteractables = new HashSet<GeneralInteractable>();
    }

    // Public method to access interactable
    //  Pre: assumes that the parent is the rat character being controlled
    //  Post: returns the nearest general interactable that the player is facing towards (lowest angle to forward)
    public GeneralInteractable getNearestInteractable(Vector3 playerForward) {
        if (inRangeInteractables.Count == 0) {
            return null;
        }

        float lowestAngle = 0.0f;
        GeneralInteractable bestInteractable = null;

        foreach(GeneralInteractable curInteractable in inRangeInteractables) {
            // Get the angle between the direction from rat to interactable and the player forward
            Vector3 interactableDir = (curInteractable.transform.position - transform.parent.position).normalized;
            float curAngle = Vector3.Angle(interactableDir, playerForward);

            if (bestInteractable == null || curAngle < lowestAngle) {
                bestInteractable = curInteractable;
                lowestAngle = curAngle;
            }
        }

        return bestInteractable;
    }

    // Public method to check if you're nearby an interactable
    public bool isNearInteractable() {
        return inRangeInteractables.Count > 0;
    }

    // If on trigger enter, put interactable in the set
    void OnTriggerEnter(Collider collider) {
        GeneralInteractable hitInteractable = collider.transform.GetComponent<GeneralInteractable>();

        if (hitInteractable != null) {
            inRangeInteractables.Add(hitInteractable);
        }
    }

    // On trigger exit, remove interactable from the set
    void OnTriggerExit(Collider collider) {
        GeneralInteractable hitInteractable = collider.transform.GetComponent<GeneralInteractable>();

        if (hitInteractable != null) {
            inRangeInteractables.Add(hitInteractable);
        }
    }
}
