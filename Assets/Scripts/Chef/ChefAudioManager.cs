using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChefAudioManager : MonoBehaviour
{
    // Audio clips
    [SerializeField]
    private AudioClip[] alertSounds = null;
    [SerializeField]
    private AudioClip[] attackSounds = null;
    [SerializeField]
    private AudioClip[] lostSounds = null;


    // Reference variable
    private AudioSource speaker;

    // Start is called before the first frame update
    void Awake()
    {
       speaker = GetComponent<AudioSource>(); 
    }

    // Public function to play an alert sound
    public void playChefAlert()
    {
        playRandomTrack(alertSounds);
    }

    // Public function to play an attack sound
    public void playChefAttack()
    {
        playRandomTrack(attackSounds);
    }

    // Public function to play the chef getting lost
    public void playChefLost()
    {
        playRandomTrack(lostSounds);
    }

    // Private helper function to play a random track from an audio clip array
    private void playRandomTrack(AudioClip[] playableClips) {
        int randomIndex = Random.Range(0, playableClips.Length);
        AudioClip curClip = playableClips[randomIndex];
        speaker.clip = curClip;
        speaker.Play();
    }
}
