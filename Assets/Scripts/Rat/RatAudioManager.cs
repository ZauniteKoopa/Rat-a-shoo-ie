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
    private AudioClip[] ratDamageSoundClips = null;
    [SerializeField]
    private AudioClip[] ratPickupSoundClips = null;
    [SerializeField]
    private AudioClip[] ratDropSoundClips = null;
    [SerializeField]
    private AudioClip[] ratFootstepClips = null;

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

    public void emitPickupSound()
    {
        int randomIndex = Random.Range(0, ratPickupSoundClips.Length);
        AudioClip curClip = ratPickupSoundClips[randomIndex];

        speaker.clip = curClip;
        speaker.PlayOneShot(curClip, 0.6f);
    }

    public void emitDropSound()
    {
        int randomIndex = Random.Range(0, ratDropSoundClips.Length);
        AudioClip curClip = ratDropSoundClips[randomIndex];

        speaker.clip = curClip;
        speaker.PlayOneShot(curClip, 0.6f);
    }

    public void emitFoootstepSound()
    {
        int randomIndex = Random.Range(0, ratFootstepClips.Length);
        AudioClip curClip = ratFootstepClips[randomIndex];

        speaker.clip = curClip;
        speaker.PlayOneShot(curClip, 0.1f);
    }

    public IEnumerator FootstepLoop()
    {
        while (true)
        {
            emitFoootstepSound();
            Debug.Log("Step");
            yield return new WaitForSeconds(.2f);
        }
    }

    public void startFootsteps()
    {
        if (!isWalking)
        {
            StopAllCoroutines();
            StartCoroutine(FootstepLoop());
            isWalking = true;
        }     
    }

    public void stopFootsteps()
    {
        if (isWalking)
        {
            StopAllCoroutines();
        }
        
    }

    public IEnumerator DashCloudLoop()
    {
        while (true)
        {
            speaker.clip = dashCloud;
            speaker.PlayOneShot(dashCloud, .2f);
            yield return new WaitForSeconds(.4f);
        }
    }

    public void startDashCloud()
    {
        StartCoroutine(DashCloudLoop());
    }

    public void stopDashCloud()
    {
        StopCoroutine(DashCloudLoop());
    }




}
