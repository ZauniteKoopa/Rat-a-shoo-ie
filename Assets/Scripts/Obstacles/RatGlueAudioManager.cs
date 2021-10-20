using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RatGlueAudioManager : MonoBehaviour
{
    private AudioSource speaker;
    [SerializeField]
    private AudioClip enterGoo = null;
    [SerializeField]
    private AudioClip exitGoo = null;

    void Start()
    {
        speaker = GetComponent<AudioSource>();
    }

    public void enterGooSound()
    {
        speaker.clip = enterGoo;
        speaker.Play();
    }

    public void exitGooSound()
    {
        speaker.clip = exitGoo;
        speaker.Play();
    }
}
