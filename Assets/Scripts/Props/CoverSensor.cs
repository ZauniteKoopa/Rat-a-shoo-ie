using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoverSensor : MonoBehaviour
{
    // If someone enters the trigger and the collider is a rat, cover the rat
    void OnTriggerEnter2D(Collider2D collider) {
        RatController ratPlayer = collider.GetComponent<RatController>();

        if (ratPlayer != null) {
            ratPlayer.takeCover();
        }
    }

    // If someone exits the trigger and the collider is a rat, reveal the rat
    void OnTriggerExit2D(Collider2D collider) {
        RatController ratPlayer = collider.GetComponent<RatController>();

        if (ratPlayer != null) {
            ratPlayer.revealCover();
        }
    }
}
