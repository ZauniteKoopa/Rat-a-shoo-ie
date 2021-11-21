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

    // Max volumes for volume management
    public float maxChaseVolume = 0.4f;
    private float maxBaseVolume;
    private float curChaseVolume;

    private void Start()
    {        
        AudioClip baseLevel = mainTrack;
        AudioClip chaseLevel = chaseTrack;
        AudioClip ambientLevel = ambientTrack;

        baseMusic.clip = baseLevel;
        baseMusic.Play();
        maxBaseVolume = baseMusic.volume;

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
        }
    }

    // Private IEnumerator to fade in chase music
    private IEnumerator fadeInChaseMusic() {
        WaitForEndOfFrame waitFrame = new WaitForEndOfFrame();

        while (chaseMusic.volume < curChaseVolume) {
            yield return waitFrame;
            chaseMusic.volume += fadeInFactor * Time.deltaTime;
        }
    }

    // Event handler method for when the rat is safe after a chase sequence
    public void ratNotSeen()
    {
        if (gameObject.activeInHierarchy) {
            StopAllCoroutines();
            StartCoroutine(fadeOutChaseMusic());
        }
    }

    // Private IEnumerator to fade out chase music
    private IEnumerator fadeOutChaseMusic() {
        WaitForEndOfFrame waitFrame = new WaitForEndOfFrame();

        while (chaseMusic.volume > 0.0f) {
            yield return waitFrame;
            chaseMusic.volume -= fadeInFactor * Time.deltaTime;
        }
    }

    // Event handler for when music manager changed
    public void onMusicVolumeChanged(float newVolume) {
        baseMusic.volume = maxBaseVolume * newVolume;
        curChaseVolume = maxChaseVolume * newVolume;

        if (chaseMusic.volume > 0.0001f) {
            chaseMusic.volume = curChaseVolume;
        }
    }
}
