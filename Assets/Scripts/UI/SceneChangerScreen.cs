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
    [SerializeField]
    private Image rat = null;
    [SerializeField]
    private Image cheese = null;

    private float ratInitialPos = 0.0f;
    // private float pseudo = 0.0f;

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
        rat.transform.position = new Vector3(ratInitialPos + ((cheese.transform.position.x - ratInitialPos) * progress),
            rat.transform.position.y,
            rat.transform.position.z);
        loadingBar.fillAmount = progress;
    }
}
