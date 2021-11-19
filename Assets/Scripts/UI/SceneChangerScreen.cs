using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneChangerScreen : SceneChanger
{
    [SerializeField]
    private GameObject loadingScreen = null;
    [SerializeField]
    private Image loadingBar = null;

    // Main method to load a level
    public void ChangeSceneAsync(string sceneName) {
        loadingScreen.SetActive(true);
        StartCoroutine(loadingSceneSequence(sceneName));
    }

    // Private IEnumerator to do loading screen sequence
    private IEnumerator loadingSceneSequence(string sceneName) {
        AsyncOperation loadingOperation = SceneManager.LoadSceneAsync(sceneName);

        while (!loadingOperation.isDone) {
            float progress = Mathf.Clamp01(loadingOperation.progress / 0.9f);
            updateProgress(progress);

            yield return null;
        }
    }

    // Private helper method to update the progress found on the loading screen
    private void updateProgress(float progress) {
        loadingBar.fillAmount = progress;
    }
}
