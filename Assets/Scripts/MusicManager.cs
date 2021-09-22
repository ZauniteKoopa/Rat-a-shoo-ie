using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    [SerializeField]
    private AudioClip[] mainTracks = null;
    [SerializeField]
    private AudioClip[] chaseTracks = null;
    [SerializeField]
    private AudioClip[] ambientTracks = null;
    public AudioSource baseMusic;
    public AudioSource chaseMusic;
    public AudioSource ambientMusic;

    public float fadeOutFactor = 0.5f;
    public float fadeInFactor = 0.5f;
    public float maxVolume = 1f;

    private void Awake()
    {
        int curLevel = SceneManager.GetActiveScene().buildIndex;
        // print("level" + curLevel);
        AudioClip baseLevel = mainTracks[curLevel];
        AudioClip chaseLevel = chaseTracks[curLevel];
        AudioClip ambientLevel = ambientTracks[curLevel];

        baseMusic.clip = baseLevel;
        baseMusic.Play();

        chaseMusic.clip = chaseLevel;
        chaseMusic.volume = 0.0f;
        chaseMusic.Play();

        ambientMusic.clip = ambientLevel;
        ambientMusic.Play();
    }

    // Event handler method for when the rat is being chased
    public void ratIsSeen()
    {
        StopAllCoroutines();
        StartCoroutine(fadeInChaseMusic());
    }

    // Private IEnumerator to fade in chase music
    private IEnumerator fadeInChaseMusic() {
        WaitForEndOfFrame waitFrame = new WaitForEndOfFrame();

        while (chaseMusic.volume < maxVolume) {
            yield return waitFrame;
            chaseMusic.volume += fadeInFactor * Time.deltaTime;
        }
    }

    // Event handler method for when the rat is safe after a chase sequence
    public void ratNotSeen()
    {
        StopAllCoroutines();
        Debug.Log("fade out");
        StartCoroutine(fadeOutChaseMusic());
    }

    // Private IEnumerator to fade out chase music
    private IEnumerator fadeOutChaseMusic() {
        WaitForEndOfFrame waitFrame = new WaitForEndOfFrame();

        while (chaseMusic.volume > 0.0f) {
            yield return waitFrame;
            chaseMusic.volume -= fadeInFactor * Time.deltaTime;
        }
    }
}
