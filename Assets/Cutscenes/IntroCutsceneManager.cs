﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroCutsceneManager : MonoBehaviour
{
    private bool interrupted = false;
    private UnityEngine.Video.VideoPlayer videoPlayer;

    // Start is called before the first frame update
    void Start()
    {
        videoPlayer = GetComponent<UnityEngine.Video.VideoPlayer>();
        StartCoroutine(playCutscene());
    }

    // Main method to play the coroutine
    private IEnumerator playCutscene() {
        float videoLength = videoPlayer.frameCount / videoPlayer.frameRate;
        yield return new WaitForSeconds(videoLength + 1);
        SceneManager.LoadScene("MainMenu");
    }

    // Event handler method when player interrupts
    public void onPlayerInterrupt() {
        if (!interrupted) {
            interrupted = true;
            StopAllCoroutines();
            SceneManager.LoadScene("MainMenu");
        }
    }
}
