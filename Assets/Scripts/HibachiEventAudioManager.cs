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
        Debug.Log("reveal");
        speaker.PlayOneShot(reveal, 1f);
    }

    public void pillarDamage()
    {
        speaker.PlayOneShot(damage);
    }

    public void endCollapse()
    {
        speaker.PlayOneShot(collapse);
    }
}
