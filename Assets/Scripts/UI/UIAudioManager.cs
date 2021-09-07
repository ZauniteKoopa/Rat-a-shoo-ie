using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAudioManager : MonoBehaviour
{

    [SerializeField]
    private AudioClip[] UISounds = null;

    private AudioSource speaker;

    void Awake()
    {
        speaker = GetComponent<AudioSource>();
    }

    public void playUISounds(int sound)
    {
        AudioClip curClip = UISounds[sound];

        speaker.clip = curClip;
        speaker.Play();
    }
}
