using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableIndicator : MonoBehaviour
{
    [SerializeField]
    private GameObject pcIndicator = null;
    [SerializeField]
    private GameObject mobileIndicator = null;

    private void Awake() {
        if (Application.platform == RuntimePlatform.Android) {
            pcIndicator.SetActive(false);
        } else {
            mobileIndicator.SetActive(false);
        }
    }
}
