using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomLoader : MonoBehaviour
{
    List<BrokenStove> roomBrokenStoves = null;
    List<Chef> roomChefs = null;
    private bool activated = false;
    private bool startSequenceEnded = false;

    // On awake
    private void Awake() {
        roomBrokenStoves = new List<BrokenStove>();
        roomChefs = new List<Chef>();
        StartCoroutine(startSequence());
    }

    // Start sequence to make sure that roomBrokenStoves are initialized before activating rooms
    private IEnumerator startSequence() {
        yield return new WaitForSeconds(0.1f);

        // If activated during beginning section, activate room now and then set flag to indicate sequence ended
        if (activated) {
            activateRoom();
        } else {
            deactivateRoom();
        }

        startSequenceEnded = true;
    }

    // Initialize broken stoves by simple trigger box and check if player enters trigger box
    private void OnTriggerEnter(Collider collider) {
        // Get important components within the room
        RatController3D player = collider.GetComponent<RatController3D>();
        

        // If broken stove found: add them to the list. If player is found, activate the room
        if (!startSequenceEnded) {
            BrokenStove stoveInstance = collider.GetComponent<BrokenStove>();
            Chef chefInstance = collider.GetComponent<Chef>();

            if (stoveInstance != null) {
                roomBrokenStoves.Add(stoveInstance);
            } else if (chefInstance != null) {
                roomChefs.Add(chefInstance);
            }
        } 
        
        if (player != null) {
            
            // Makes sure that lists are initialized before activating the room
            if (startSequenceEnded) {
                activateRoom();
            } else {
                activated = true;
            }
        }
    }

    // If player exits, deactivate room
    private void OnTriggerExit(Collider collider) {
        RatController3D player = collider.GetComponent<RatController3D>();

        if (player != null) {
            deactivateRoom();
        }
    }

    // Main method to activate
    private void activateRoom() {
        foreach(BrokenStove stove in roomBrokenStoves) {
            stove.activate();
        }

        foreach(Chef chef in roomChefs) {
            if (chef != null) {
                chef.activateChef();
            }
        }
    }

    // Main method to deactivate room
    private void deactivateRoom() {
        foreach(BrokenStove stove in roomBrokenStoves) {
            stove.deactivate();
        }

        foreach (Chef chef in roomChefs) {
            if (chef != null) {
                chef.deactivateChef();
            }
        }
    }

}
