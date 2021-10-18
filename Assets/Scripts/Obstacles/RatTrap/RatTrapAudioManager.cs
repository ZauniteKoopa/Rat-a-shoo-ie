using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RatTrapAudioManager : MonoBehaviour
{
    private AudioSource speaker;
    [SerializeField]
    private AudioClip[] snapSounds = null;
    [SerializeField]
    private AudioClip[] resetSounds = null;

    // Start is called before the first frame update
    void Start()
    {
        speaker = GetComponent<AudioSource>();   
    }

    // Public method to play snap sound
    public void playSnapSound() {
        speaker.clip = pickRandomSound(snapSounds);
        speaker.Play();
    }

    // Public method to play reset sounds
    public void playResetSound() {
        speaker.clip = pickRandomSound(resetSounds);
        speaker.Play();
    }

    // Public method to access the current clip length
    public float getCurrentClipLength() {
        return speaker.clip.length;
    }

    // Private helper method to pick a random sound in the array
    private AudioClip pickRandomSound(AudioClip[] clips) {
        int index = Random.Range(0, clips.Length);
        return clips[index];
    }
}
