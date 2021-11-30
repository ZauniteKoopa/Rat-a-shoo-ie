using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RatAudioManager : MonoBehaviour
{
    public bool isWalking = false;
    private AudioSource speaker;
    [SerializeField]
    private AudioClip dashCloud = null;
    [SerializeField]
    private AudioClip jumpClip = null;
    [SerializeField]
    private AudioClip[] ratDamageSoundClips = null;
    [SerializeField]
    private AudioClip[] ratPickupSoundClips = null;
    [SerializeField]
    private AudioClip[] ratDropSoundClips = null;

    // Footstep methods
    [SerializeField]
    private AudioClip[] ratFootstepClips = null;
    [SerializeField]
    private float walkInterval = 0.3f;
    [SerializeField]
    private float sprintInterval = 0.2f;
    private float curInterval = 0.3f;


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
        speaker.PlayOneShot(curClip);
    }

    // Public method to play pickup sound
    public void emitPickupSound()
    {
        int randomIndex = Random.Range(0, ratPickupSoundClips.Length);
        AudioClip curClip = ratPickupSoundClips[randomIndex];

        speaker.clip = curClip;
        speaker.PlayOneShot(curClip, 1f);
    }

    // Public method to play drop sound
    public void emitDropSound()
    {
        int randomIndex = Random.Range(0, ratDropSoundClips.Length);
        AudioClip curClip = ratDropSoundClips[randomIndex];

        speaker.clip = curClip;
        speaker.PlayOneShot(curClip, 1f);
    }

    // Private method to emit footstep sound
    private void emitFoootstepSound()
    {
        int randomIndex = Random.Range(0, ratFootstepClips.Length);
        AudioClip curClip = ratFootstepClips[randomIndex];

        speaker.clip = curClip;
        speaker.PlayOneShot(curClip, 1.5f);
    }

    // Main footstep looping 
    private IEnumerator FootstepLoop()
    {
        while (true)
        {
            emitFoootstepSound();
            yield return new WaitForSeconds(curInterval);
        }
    }

    // Public method to start footstep with isSprinting on or off
    public void startFootsteps(bool isSprinting)
    {
        curInterval = (isSprinting) ? sprintInterval : walkInterval;

        if (!isWalking)
        {
            //StopAllCoroutines();
            isWalking = true;
            StartCoroutine(FootstepLoop());
        }   
    }

    // Public method to stop footstep
    public void stopFootsteps()
    {
        if (isWalking)
        {
            isWalking = false;
            StopAllCoroutines();
        }
        
    }

    // Public method to play dash sound effect
    public void playDash()
    {
        speaker.clip = dashCloud;
        speaker.PlayOneShot(dashCloud, .5f);
    }

    // Public method to play jump sound effect
    public void playJump() {
        speaker.clip = jumpClip;
        speaker.PlayOneShot(jumpClip, 1.4f);
    }
}
