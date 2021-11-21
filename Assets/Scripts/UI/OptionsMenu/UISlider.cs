using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

[System.Serializable]
public class SliderEvent : UnityEvent<float> {}

public class UISlider : MonoBehaviour
{
    private Slider mainSlider;
    public SliderEvent sliderChangedEvent = null;


    // Start is called before the first frame update
    void Start()
    {
        mainSlider = GetComponent<Slider>();
        mainSlider.onValueChanged.AddListener(delegate {onValueChanged();});

        sliderChangedEvent.AddListener(PersistentData.instance.updateMusicVolume);
        mainSlider.value = PersistentData.instance.musicVolume;
    }

    // Event handler when value changed
    public void onValueChanged() {
        sliderChangedEvent.Invoke(mainSlider.value);
    }
    
}
