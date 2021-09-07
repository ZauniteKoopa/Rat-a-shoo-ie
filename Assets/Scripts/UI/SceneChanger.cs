using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    // Method to change the scene, given the scene name
    public void ChangeScene(string sceneName) {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(sceneName);
    }

    // Method to exit the application
    public void Exit() {
        Application.Quit();
    }
}
