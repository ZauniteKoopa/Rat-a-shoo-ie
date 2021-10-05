using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderWindowAudioManager : MonoBehaviour
{
    private AudioSource speaker;
    [SerializeField]
    private AudioClip[] barfSounds = null;

    //On first frame, get the current audio source
    private void Awake() {
        speaker = GetComponent<AudioSource>();
    }

    // Public method to play barf sound when a customer has been poisoned by the food the rat makes
    public void playBarfSound() {
        playRandomTrack(barfSounds);
    }

    // Private helper function to play a random track from an audio clip array
    private void playRandomTrack(AudioClip[] playableClips) {
        int randomIndex = Random.Range(0, playableClips.Length);
        AudioClip curClip = playableClips[randomIndex];
        speaker.clip = curClip;
        speaker.Play();
    }
}
