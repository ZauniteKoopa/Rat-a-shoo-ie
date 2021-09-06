using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RatAudioManager : MonoBehaviour
{
    private AudioSource speaker;
    [SerializeField]
    private AudioClip[] ratDamageSoundClips = null;

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
}
