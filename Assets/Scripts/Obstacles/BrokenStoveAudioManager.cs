using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrokenStoveAudioManager : MonoBehaviour
{
    private AudioSource speaker;
    [SerializeField]
    private AudioClip warningSound = null;

    void Start()
    {
        speaker = GetComponent<AudioSource>();
    }

    public void playArmSound()
    {
        speaker.clip = warningSound;
        speaker.Play();
    }
}
