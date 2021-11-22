using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Audio;

// Main event to handle audio volume changes
public class AudioVolumeDelegate : UnityEvent<float> {}

public class PersistentData : MonoBehaviour
{
    public static PersistentData instance = null;

    // Level cleared status
    public bool[] levelCleared;
    public int numLevels = 3;
    public int lastLevelCleared = 0;

    // Audio options
    public float musicVolume = 1.0f;
    private AudioVolumeDelegate musicVolumeChangeEvent;

    // Sound effect variables
    public AudioMixer soundEffectsChannel = null;
    public float effectsVolume = 1.0f;
    private float maxVolume = 0.0f;
    private float minVolume = -20.0f;
    private float noVolume = -80.0f;

    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null) {
            DontDestroyOnLoad(gameObject);

            musicVolumeChangeEvent = new AudioVolumeDelegate();
            onLevelLoad();
            levelCleared = new bool[numLevels];

            instance = this;
        } else if (instance != this) {
            instance.onLevelLoad();
            Destroy(gameObject);
        }
    }

    // On new level load, update audio settings to connect to the current level
    public void onLevelLoad() {
        musicVolumeChangeEvent.RemoveAllListeners();

        MusicManager musicManager = FindObjectOfType<MusicManager>();

        if (musicManager != null) {
            musicVolumeChangeEvent.AddListener(musicManager.onMusicVolumeChanged);
        }
    }

    // Public void method to save level state
    public void saveVictory(int levelIndex) {
        lastLevelCleared = levelIndex;
        PersistentData.instance.levelCleared[levelIndex] = true;
    }

    // Method to check if you can even access the level
    public bool canAccessLevel (int levelIndex) {
        if (levelIndex == 0) {
            return true;
        } else {
            return PersistentData.instance.levelCleared[levelIndex - 1];
        }
    }

    // Method to update music volume data
    public void updateMusicVolume(float musicVolume) {
        this.musicVolume = musicVolume;
        musicVolumeChangeEvent.Invoke(musicVolume);
    }

    // Method to update sound effects volume
    public void updateSoundEffectsVolume(float fxVolume) {
        effectsVolume = fxVolume;

        if (fxVolume < 0.00001) {
            soundEffectsChannel.SetFloat("SoundEffectVolume", noVolume);
        } else {
            float curVolume = Mathf.Lerp(minVolume, maxVolume, fxVolume);
            soundEffectsChannel.SetFloat("SoundEffectVolume", curVolume);
        }
    }
}
