using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrokenStoveInterferenceHandler : MonoBehaviour
{
    // Boolean locks used to make sure 
    private bool canPlayAnticipation = true;
    private bool canPlayFire = true;
    [SerializeField]
    private int lockDurationInFrames = 2;

    // Initialize broken stoves by simple trigger box and check if player enters trigger box
    private void OnTriggerEnter(Collider collider) {  
        BrokenStoveAudioManager stoveInstanceAudio = collider.GetComponent<BrokenStoveAudioManager>();

        if (stoveInstanceAudio != null) {
            stoveInstanceAudio.setInterferenceHandler(this);
        }       
    }

    // Public method to try to play anticipation sound
    public void tryPlayAnticipation(AudioSource speaker) {
        if (canPlayAnticipation) {
            speaker.Play();
            StartCoroutine(anticipationLockSequence());
        }
    }

    // Sequence IEnumerator to do the anticipation lock sequence
    private IEnumerator anticipationLockSequence() {
        canPlayAnticipation = false;
        WaitForEndOfFrame waitFrame = new WaitForEndOfFrame();

        for (int f = 0; f < lockDurationInFrames; f++) {
            yield return waitFrame;
        }

        canPlayAnticipation = true;
    }

    // Public method to try to play fire sound
    public void tryPlayFire(AudioSource speaker) {
        if (canPlayFire) {
            speaker.Play();
            StartCoroutine(fireLockSequence());
        }
    }

    // Sequence IEnumerator to do the fire lock sequence
    private IEnumerator fireLockSequence() {
        canPlayFire = false;
        WaitForEndOfFrame waitFrame = new WaitForEndOfFrame();

        for (int f = 0; f < lockDurationInFrames; f++) {
            yield return waitFrame;
        }

        canPlayFire = true;
    }
}
