using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RatAudioManager : MonoBehaviour
{
    private AudioSource speaker;
    [SerializeField]
    private AudioClip[] ratDamageSoundClips = null;
    [SerializeField]
    private AudioClip[] ratPickupSoundClips = null;
    [SerializeField]
    private AudioClip[] ratDropSoundClips = null;

    // Start is called before the first frame update
    void Awake()
    {
        speaker = GetComponent<AudioSource>();
    }

    // Public method to do damage sound
    public void emitDamageSound() {
        int randomIndex = Random.Range(0, ratDamageSoundClips.Length);
        AudioClip curClip = ratDamageSoundClips[randomIndex];

        speaker.clip = curClip;
        speaker.Play();
    }

    public void emitPickupSound()
    {
        int randomIndex = Random.Range(0, ratPickupSoundClips.Length);
        AudioClip curClip = ratPickupSoundClips[randomIndex];

        speaker.clip = curClip;
        speaker.Play();
    }

    public void emitDropSound()
    {
        int randomIndex = Random.Range(0, ratDropSoundClips.Length);
        AudioClip curClip = ratDropSoundClips[randomIndex];

        speaker.clip = curClip;
        speaker.Play();
    }
}
