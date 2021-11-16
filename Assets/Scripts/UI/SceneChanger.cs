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

    // Method to take to victory screen
    public void takeToVictoryScreen(int levelIndex) {
        if (levelIndex == 0) {
            ChangeScene("WinScreen_Home");
        } else if (levelIndex == 1) {
            ChangeScene("WinScreen_Diner");
        } else if (levelIndex == 2) {
            ChangeScene("WinScreen_Sushi");
        } else if (levelIndex == 3) {
            ChangeScene("WinScreen_Hibachi");
        }
    }
}
