using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class DropdownEvent : UnityEvent<int> {}

public class ResolutionDropDown : MonoBehaviour
{
    private Dropdown mainDropdown;
    private DropdownEvent valueChangedEvent;

    // Start is called before the first frame update
    void Start()
    {
        mainDropdown = GetComponent<Dropdown>();
        valueChangedEvent = new DropdownEvent();

        // Set up resolution options
        mainDropdown.ClearOptions();
        Resolution[] screenResolutions = Screen.resolutions;
        List<string> dropOptions = new List<string>();

        if (screenResolutions.Length > 0) {
            for(int i = 0; i < PersistentData.instance.maxScreenResolutionIndex; i++) {
                Resolution resolution = screenResolutions[i];
                string label = "" + resolution.width + " x " + resolution.height;
                dropOptions.Add(label);
            }
        } else {
            string label = Screen.width + " x " + Screen.height;
            dropOptions.Add(label);
        }
        

        mainDropdown.AddOptions(dropOptions);

        // Set up default settings and connect to persistent data
        valueChangedEvent.AddListener(PersistentData.instance.updateScreenResolution);
        mainDropdown.onValueChanged.AddListener(delegate {onDropdownChange();});

        if (PersistentData.instance.screenResolutionIndex >= 0) {
            mainDropdown.value = PersistentData.instance.screenResolutionIndex;
        }
    }

    // Main method for when value changed
    private void onDropdownChange() {
        valueChangedEvent.Invoke(mainDropdown.value);
    }
}
