using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Main class to check if blocked
public class BlockerSensor : MonoBehaviour
{
    // Public method to check if the sensor is blocked
    public bool isBlocked() {
        return checkBlocker();
    }

    // Main method to check if box cast is sensing something and updateds blocked accordingly
    //  Returns true if an object tagged platform is in the blocker, false otherwise
    private bool checkBlocker() {
        RaycastHit[] hits = Physics.BoxCastAll(transform.position, transform.localScale * 0.5f, transform.forward, Quaternion.identity, 0f);

        if (hits != null) {
            foreach(RaycastHit hit in hits) {
                if (hit.collider.tag == "Platform") {
                    return true;
                }
            }
        }

        return false;
    }

}
