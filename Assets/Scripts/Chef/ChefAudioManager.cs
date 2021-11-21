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
    [SerializeField]
    private AudioClip[] enrageSound = null;
    [SerializeField]
    private AudioClip[] weaponSounds = null;


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

    public void playChefEnrage()
    {
        playRandomTrack(enrageSound);
    }

    public void playWeaponAttack()
    {
        if (weaponSounds != null && weaponSounds.Length > 0) {
            if (weaponSounds[0].name == "fx_chef_hibachi_attack_v2")
            {
                AudioClip curClip = weaponSounds[0];
                speaker.clip = curClip;
                speaker.PlayOneShot(curClip, 1.5f);
            }
            else 
            { 
                playRandomTrack(weaponSounds);
            }
        }
    }


    // Private helper function to play a random track from an audio clip array
    private void playRandomTrack(AudioClip[] playableClips) {
        int randomIndex = Random.Range(0, playableClips.Length);
        AudioClip curClip = playableClips[randomIndex];
        speaker.clip = curClip;
        speaker.PlayOneShot(curClip);
    }
}
