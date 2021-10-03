using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelector : MonoBehaviour
{
    [SerializeField]
    private int levelIndex = 0;
    private Button levelButton = null;

    private void Awake() {
        levelButton = GetComponent<Button>();
        levelButton.interactable = PersistentData.instance.canAccessLevel(levelIndex);
    }
}
