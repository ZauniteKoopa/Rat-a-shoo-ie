using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetInteractable : GeneralInteractable
{
    // Variables that keep track of reset state
    private Vector3 resetPosition;

    // Main method to get the reset position 
    private void Start() {
        resetPosition = transform.position;
    }

    // Virtual method to actually reset
    public virtual void reset() {
        if (canBePickedUp) {
            transform.position = resetPosition;
        }
    }
}
