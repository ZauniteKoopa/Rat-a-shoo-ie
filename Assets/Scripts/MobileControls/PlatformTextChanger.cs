using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlatformTextChanger : MonoBehaviour
{
    [SerializeField]
    private TMP_Text textBox = null;
    [SerializeField]
    private string pcText = null;
    [SerializeField]
    private string androidText = null;

    // Start is called before the first frame update
    void Start()
    {
        textBox.text = (Application.platform == RuntimePlatform.Android) ? androidText : pcText;
    }
}
