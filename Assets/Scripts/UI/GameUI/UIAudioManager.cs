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
        if (sound == 0)
        {
            //speaker.volume = 0.5f;
            speaker.PlayOneShot(UISounds[0], 0.5f);
        }
        else if (sound == 1)
        {
            //speaker.volume = 0.5f;
            speaker.PlayOneShot(UISounds[1], 0.4f);
        } else if (sound == 2) 
        {
            speaker.PlayOneShot(UISounds[2], 0.3f);
        } else if (sound == 5)
        {
            //speaker.volume = 0.6f;
            speaker.PlayOneShot(UISounds[5], 0.5f);
        }
        else
        {
            speaker.volume = 0.5f;
        }

        speaker.clip = curClip;
        speaker.PlayOneShot(curClip);
    }
}
