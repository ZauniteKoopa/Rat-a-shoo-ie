using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class ToggleButton : MonoBehaviour
{
    private bool toggledOn = false;
    [SerializeField]
    private Image cancelSprite = null;

    private void Awake() {
        cancelSprite.gameObject.SetActive(false);
    }

    public void toggle(InputAction.CallbackContext value) {
        if (value.started) {
            toggledOn = !toggledOn;
            cancelSprite.gameObject.SetActive(toggledOn);
        }
    }
}
