using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentData : MonoBehaviour
{
    public static PersistentData instance = null;

    public bool[] levelCleared;
    public int numLevels = 3;
    public int lastLevelCleared = 0;

    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null) {
            DontDestroyOnLoad(gameObject);
            levelCleared = new bool[numLevels];
            instance = this;
        } else if (instance != this) {
            Destroy(gameObject);
        }
    }

    // Public void method to save level state
    public void saveVictory(int levelIndex) {
        lastLevelCleared = levelIndex;
        PersistentData.instance.levelCleared[levelIndex] = true;
    }

    // Method to check if you can even access the level
    public bool canAccessLevel (int levelIndex) {
        if (levelIndex == 0) {
            return true;
        } else {
            return PersistentData.instance.levelCleared[levelIndex - 1];
        }
    }
}
