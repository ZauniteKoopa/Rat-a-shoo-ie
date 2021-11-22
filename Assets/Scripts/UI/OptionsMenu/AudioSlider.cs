using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.Events;

public class AudioSlider : MonoBehaviour
{
    // Attachment to audio mixer
    [SerializeField]
    private AudioMixer audioChannel = null;

    private Slider mainSlider;
    public SliderEvent sliderChangedEvent = null;

    private float maxVolume = 0.0f;
    private float minVolume = -20.0f;
    private float noVolume = -80.0f;

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
        //sliderChangedEvent.Invoke(mainSlider.value);
        if (mainSlider.value < 0.00001) {
            audioChannel.SetFloat("SoundEffectVolume", noVolume);
        } else {
            float curVolume = Mathf.Lerp(minVolume, maxVolume, mainSlider.value);
            audioChannel.SetFloat("SoundEffectVolume", curVolume);
        }
    }

}
