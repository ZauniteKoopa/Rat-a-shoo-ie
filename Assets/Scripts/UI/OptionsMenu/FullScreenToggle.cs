using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ToggleEvent : UnityEvent<bool> {}

public class FullScreenToggle : MonoBehaviour
{
    private Toggle mainToggle = null;
    private ToggleEvent toggleChangedEvent = null;

    // Start is called before the first frame update
    void Start()
    {
        if (Application.platform == RuntimePlatform.Android) {
            transform.parent.gameObject.SetActive(false);
        } else {
            mainToggle = GetComponent<Toggle>();
            toggleChangedEvent = new ToggleEvent();
            mainToggle.onValueChanged.AddListener(onToggleChange);

            toggleChangedEvent.AddListener(PersistentData.instance.updateFullscreen);
            mainToggle.isOn = PersistentData.instance.isFullScreen;
        }
    }

    // Event handler method for when toggle has been clicked
    public void onToggleChange(bool newValue) {
        toggleChangedEvent.Invoke(newValue);
    }
}
