using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class AudioSlider : MonoBehaviour
{
    // Attachment to audio mixer
    private Slider mainSlider;
    public SliderEvent sliderChangedEvent = null;

    // Start is called before the first frame update
    void Start()
    {
        mainSlider = GetComponent<Slider>();
        mainSlider.onValueChanged.AddListener(delegate {onValueChanged();});

        sliderChangedEvent.AddListener(PersistentData.instance.updateSoundEffectsVolume);
        mainSlider.value = PersistentData.instance.effectsVolume;
    }

    // Event handler when value changed
    public void onValueChanged() {
        sliderChangedEvent.Invoke(mainSlider.value);
    }

}
