using System.Collections;
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

            if ((bestInteractable == null || curAngle < lowestAngle) && curInteractable.canBePickedUp) {
                bestInteractable = curInteractable;
                lowestAngle = curAngle;
            }
        }

        return bestInteractable;
    }

    // Public method to check if you're nearby an interactable: cull out null interactables first before checking the count
    public bool isNearInteractable() {
        // Cull out any objects that have been destroyed if there are any objects to process
        if (inRangeInteractables.Count > 0) {
            List<GeneralInteractable> destroyedInteractables = new List<GeneralInteractable>();

            foreach (GeneralInteractable interactable in inRangeInteractables) {
                if (interactable == null || !interactable.canBePickedUp) {
                    destroyedInteractables.Add(interactable);
                }
            }

            foreach (GeneralInteractable interactable in destroyedInteractables) {
                inRangeInteractables.Remove(interactable);
            }
        }


        // Return if there's any way to interact
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
            inRangeInteractables.Remove(hitInteractable);
        }
    }
}
