using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RatGlue : MonoBehaviour
{
    // On trigger enter, if collider is a rat. Slow rat down
    private void OnTriggerEnter(Collider collider) {
        RatController3D ratPlayer = collider.GetComponent<RatController3D>();

        if (ratPlayer != null) {
            ratPlayer.setSlowStatus(true);
        }
    }

    // On trigger exit, if collider is a rat. Go back to normal
    private void OnTriggerExit(Collider collider) {
        RatController3D ratPlayer = collider.GetComponent<RatController3D>();

        if (ratPlayer != null) {
            ratPlayer.setSlowStatus(false);
        }
    }
}
