using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RatGlue : MonoBehaviour
{
    private RatGlueAudioManager audioManager = null;

    private void Start()
    {
        audioManager = GetComponent<RatGlueAudioManager>();
    }

    // On trigger enter, if collider is a rat. Slow rat down
    private void OnTriggerEnter(Collider collider) {
        RatController3D ratPlayer = collider.GetComponent<RatController3D>();

        if (ratPlayer != null) {
            ratPlayer.setSlowStatus(true);
            audioManager.enterGooSound();
        }
    }

    // On trigger exit, if collider is a rat. Go back to normal
    private void OnTriggerExit(Collider collider) {
        RatController3D ratPlayer = collider.GetComponent<RatController3D>();

        if (ratPlayer != null) {
            ratPlayer.setSlowStatus(false);
            audioManager.exitGooSound();
        }
    }
}
