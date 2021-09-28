using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CookingStationAudioManager : MonoBehaviour
{
    [SerializeField]
    private AudioClip[] dishRuinedSounds = null;
    private AudioSource audioSource = null;

    // On awake, get your own audio source
    private void Awake() {
        audioSource = GetComponent<AudioSource>();
    }

    // Main function to play a ruined dish sound
    public void playRuinedDishSound() {
        int randomIndex = Random.Range(0, dishRuinedSounds.Length);
        audioSource.clip = dishRuinedSounds[randomIndex];
        audioSource.Play();
    }
}
