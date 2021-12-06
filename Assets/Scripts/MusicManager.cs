using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    [SerializeField]
    private AudioClip mainTrack = null;
    [SerializeField]
    private AudioClip chaseTrack = null;
    [SerializeField]
    private AudioClip ambientTrack = null;
    public AudioSource baseMusic;
    public AudioSource chaseMusic;
    public AudioSource ambientMusic;

    // Fade in / out
    public float fadeOutFactor = 0.5f;
    public float fadeInFactor = 0.5f;
    public float hibachiFadeOut = 0.5f;

    // Max volumes for volume management
    public float maxChaseVolume = 0.4f;
    public float minBaseVolume = 0.2f;
    private float maxBaseVolume;
    private float curChaseVolume;
    private float curBaseVolume;

    // Boolean flags for when music is playing
    private bool chaseMusicPlaying = false;
    private bool baseMusicDucked = false;

    private void Start()
    {        
        AudioClip baseLevel = mainTrack;
        AudioClip chaseLevel = chaseTrack;
        AudioClip ambientLevel = ambientTrack;

        baseMusic.clip = baseLevel;
        maxBaseVolume = baseMusic.volume;
        curBaseVolume = maxBaseVolume;
        baseMusic.Play();

        chaseMusic.clip = chaseLevel;
        chaseMusic.volume = 0.0f;
        curChaseVolume = maxChaseVolume;
        chaseMusic.Play();

        ambientMusic.clip = ambientLevel;
        ambientMusic.Play();

        onMusicVolumeChanged(PersistentData.instance.musicVolume);
    }

    // Event handler method for when the rat is being chased
    public void ratIsSeen()
    {
        if (gameObject.activeInHierarchy) {
            StopAllCoroutines();
            StartCoroutine(fadeInChaseMusic());
            StartCoroutine(duckBaseMusic());
        }
    }

    // Private IEnumerator to fade in chase music
    private IEnumerator fadeInChaseMusic() {
        WaitForEndOfFrame waitFrame = new WaitForEndOfFrame();

        while (chaseMusic.volume < curChaseVolume) {
            yield return waitFrame;
            chaseMusic.volume += fadeInFactor * Time.deltaTime;
        }

        chaseMusicPlaying = true;
    }

    // Private IEnumerator to duck base music
    private IEnumerator duckBaseMusic()
    {
        WaitForEndOfFrame waitFrame = new WaitForEndOfFrame();

        while(baseMusic.volume > minBaseVolume)
        {
            yield return waitFrame;
            baseMusic.volume -= fadeOutFactor * Time.deltaTime;
        }

        baseMusicDucked = true;
    }

    // Event handler method for when the rat is safe after a chase sequence
    public void ratNotSeen()
    {
        if (gameObject.activeInHierarchy) {
            StopAllCoroutines();
            StartCoroutine(fadeOutChaseMusic());
            StartCoroutine(unduckBaseMusic());
        }
    }

    // Private IEnumerator to fade out chase music
    private IEnumerator fadeOutChaseMusic() {
        WaitForEndOfFrame waitFrame = new WaitForEndOfFrame();

        while (chaseMusic.volume > 0.0f) {
            yield return waitFrame;
            chaseMusic.volume -= fadeOutFactor * Time.deltaTime;
        }

        chaseMusicPlaying = false;
    }

    //Private IEnumerator to return base music
    private IEnumerator unduckBaseMusic()
    {
        WaitForEndOfFrame waitFrame = new WaitForEndOfFrame();

        while(baseMusic.volume < curBaseVolume)
        {
            yield return waitFrame;
            baseMusic.volume += fadeInFactor * Time.deltaTime;
        }

        baseMusicDucked = false;
    }

    private IEnumerator finalFadeOut()
    {
        WaitForEndOfFrame waitFrame = new WaitForEndOfFrame();

        while(baseMusic.volume > 0.0f)
        {
            yield return waitFrame;
            baseMusic.volume -= hibachiFadeOut * Time.deltaTime;
            chaseMusic.volume -= (2 * hibachiFadeOut) * Time.deltaTime;
        }
    }
    public void hibachiCollapseAudio()
    {
        Debug.Log("THIS IS IT LUIGI");
        StopAllCoroutines();
        StartCoroutine(finalFadeOut());
    }

    // Event handler for when music manager changed
    public void onMusicVolumeChanged(float newVolume) {
        curBaseVolume = maxBaseVolume * newVolume;
        curChaseVolume = maxChaseVolume * newVolume;

        if (chaseMusicPlaying) {
            chaseMusic.volume = curChaseVolume;
        }
        if (!baseMusicDucked)
        {
            baseMusic.volume = curBaseVolume;
        }
    }
}
