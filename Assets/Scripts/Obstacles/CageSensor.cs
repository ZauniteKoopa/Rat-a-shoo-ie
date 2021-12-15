using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CageSensor : MonoBehaviour
{
    // Main Unity event when trap has sensed something
    private RatController3D caughtPlayer = null;
    public UnityEvent cagePlatformResetEvent;

    // If player enters trigger box, send out event
    private void OnTriggerEnter(Collider collider) {
        RatController3D ratPlayer = collider.GetComponent<RatController3D>();

        if (ratPlayer != null) {
            caughtPlayer = ratPlayer;
            ratPlayer.playerHealthLossEvent.AddListener(onPlayerDies);
        }
    }

    // If player exits the trigger box, update variables
    private void OnTriggerExit(Collider collider) {
        RatController3D ratPlayer = collider.GetComponent<RatController3D>();

        if (ratPlayer != null) {
            ratPlayer.playerHealthLossEvent.RemoveListener(onPlayerDies);
            caughtPlayer = null;
        }
    }

    // Variable to check if sensor is still being triggered
    public RatController3D getCaughtPlayer() {
        return caughtPlayer;
    }

    // Private function that handles when player dies
    private void onPlayerDies() {
        caughtPlayer.playerHealthLossEvent.RemoveListener(onPlayerDies);
        caughtPlayer = null;
    }

    // Public method to activate the platform
    public void activatePlatform() {
        GetComponent<Collider>().isTrigger = false;
        tag = "Platform";
        gameObject.layer = 0;
    }

    // Public method to reset the platform
    public void resetPlatform() {
        GetComponent<Collider>().isTrigger = true;
        tag = "Untagged";
        gameObject.layer = 2;
        cagePlatformResetEvent.Invoke();
    }
}
