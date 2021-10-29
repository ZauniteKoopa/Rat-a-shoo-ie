using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrokenStoveAudioManager : MonoBehaviour
{
    private AudioSource speaker;
    [SerializeField]
    private AudioClip warningSound = null;
    [SerializeField]
    private AudioClip fireSound = null;
    private BrokenStoveInterferenceHandler interferenceHandler = null;

    // On start, get the audio source
    void Start()
    {
        speaker = GetComponent<AudioSource>();
    }

    // Main method to set the audio interference handler
    public void setInterferenceHandler(BrokenStoveInterferenceHandler interference) {
        interferenceHandler = interference;
    }

    // Method to play anticipation sound
    public void playArmSound()
    {
        speaker.clip = warningSound;

        if (interferenceHandler != null) {
            interferenceHandler.tryPlayAnticipation(speaker);
        } else {
            speaker.Play();
        }
    }

    // Method to play fire sound
    public void playFireSound() {
        speaker.clip = fireSound;
        
        if (interferenceHandler != null) {
            interferenceHandler.tryPlayFire(speaker);
        } else {
            speaker.Play();
        }
    }

    // Method to stop all sounds
    public void stopSounds() {
        speaker.Stop();
    }
}
