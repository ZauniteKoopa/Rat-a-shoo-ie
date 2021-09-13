using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    [SerializeField]
    private AudioClip[] tracks = null;
    public AudioSource baseMusic;
    public AudioSource chaseMusic;

    private void Awake()
    {
        int curLevel = SceneManager.GetActiveScene().buildIndex;
        int curChaseLevel = SceneManager.GetActiveScene().buildIndex + SceneManager.sceneCountInBuildSettings;
        print("level" + curLevel);
        AudioClip baseLevel = tracks[curLevel];
        //AudioClip chaseLevel = tracks[curLevel + SceneManager.sceneCountInBuildSettings];
        print("chase index" + curChaseLevel);

        baseMusic.clip = baseLevel;
        baseMusic.Play();

        //chaseMusic.clip = chaseLevel;
        //chaseMusic.volume = 0.0f;
        //chaseMusic.Play();
    }
}
