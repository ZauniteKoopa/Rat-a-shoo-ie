using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HibachiEventAudioManager : MonoBehaviour
{
    [SerializeField]
    private AudioClip reveal = null;
    [SerializeField]
    private AudioClip damage = null;
    [SerializeField]
    private AudioClip collapse = null;

    private AudioSource speaker;

    void Awake()
    {
        speaker = GetComponent<AudioSource>();
    }

    public void pillarReveal()
    {
        speaker.PlayOneShot(reveal, 1.75f);
    }

    public void pillarDamage()
    {
        speaker.PlayOneShot(damage, 2.5f);
    }

    public void endCollapse()
    {
        speaker.PlayOneShot(collapse, 2f);
        GameObject.Find("MusicPlayer").GetComponent<MusicManager>().hibachiCollapseAudio();
    }
}
