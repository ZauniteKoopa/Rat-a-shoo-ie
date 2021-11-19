using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformIconChanger : MonoBehaviour
{
    // Main GameObject to make public
    [SerializeField]
    private GameObject androidIcon = null;
    [SerializeField]
    private GameObject computerIcon = null;

    // Start is called before the first frame update
    void Start()
    {
        // If on android, enable android on screen controls
        if (Application.platform == RuntimePlatform.Android) {
            androidIcon.SetActive(true);
            computerIcon.SetActive(false);
        } else {
            computerIcon.SetActive(true);
            androidIcon.SetActive(false);
        }
    }
}
