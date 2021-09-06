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

    // Public variable to play an alert sound
    public void playChefAlert() {
        int randomIndex = Random.Range(0, alertSounds.Length);
        AudioClip curClip = alertSounds[randomIndex];

        speaker.clip = curClip;
        speaker.Play();
    }

    public void playChefAttack()
    {
        int randomIndex = Random.Range(0, attackSounds.Length);
        AudioClip curClip = attackSounds[randomIndex];

        speaker.clip = curClip;
        speaker.Play();
    }

    public void playChefLost()
    {
        int randomIndex = Random.Range(0, lostSounds.Length);
        AudioClip curClip = lostSounds[randomIndex];

        speaker.clip = curClip;
        speaker.Play();
    }
}
