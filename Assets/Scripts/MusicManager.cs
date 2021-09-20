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
    public AudioSource baseMusic;
    public AudioSource chaseMusic;

    public float fadeOutFactor = 0.5f;
    public float fadeInFactor = 0.5f;
    public float maxVolume = 1f;
    private bool fadeInOut = false;

    private void Awake()
    {
        int curLevel = SceneManager.GetActiveScene().buildIndex;
        print("level" + curLevel);
        AudioClip baseLevel = mainTracks[curLevel];
        AudioClip chaseLevel = chaseTracks[curLevel];

        baseMusic.clip = baseLevel;
        baseMusic.Play();

        chaseMusic.clip = chaseLevel;
        chaseMusic.volume = 0.0f;
        chaseMusic.Play();
    }

    private void Update()
    {
        Debug.Log(fadeInOut);
        if (fadeInOut)
        {
            Debug.Log("update true");
            if (chaseMusic.volume < maxVolume)
            {
                chaseMusic.volume += fadeInFactor * Time.deltaTime;
            }
        }

        if (!fadeInOut)
        {
            //Debug.Log("update false");
            if (chaseMusic.volume > 0.0f)
            {
                chaseMusic.volume -= fadeOutFactor * Time.deltaTime;
            }
        }
    }

    public void ratIsSeen()
    {
        fadeInOut = true;
        Debug.Log("seen and chasing");
    }

    public void ratNotSeen()
    {
        fadeInOut = false;
        Debug.Log("safe");
    }
}
