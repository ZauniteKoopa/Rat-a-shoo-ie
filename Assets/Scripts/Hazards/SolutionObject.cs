﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SolutionType {
    FIRE_EXTINGUISHER,
    TOWEL,
    BROOM,
    PEACH,
    APPLE,
    CARROT,
    FISH,
    RICE,
    EGG,
    ONION
};

public class SolutionObject : MonoBehaviour
{

    public SolutionType solutionType = SolutionType.FIRE_EXTINGUISHER;
    private Vector3 initialLocation = Vector3.zero;
    public bool canChefGrab = true;

    [SerializeField]
    private AudioClip[] usageSounds = null;
    private AudioSource speaker = null;
    private float randomVariance = 0.15f;

    // On start, get the initial location of this solution
    private void Start() {
        speaker = GetComponent<AudioSource>();
        initialLocation = transform.position;
    }

    // in getDuration, make a switch statement that decides the duration of the object based on enum
    //  MUST BE UPDATED WHEN ADDING ANOTHER SOLUTION TYPE TO THE ENUM
    public float getDuration() {

        switch(solutionType) {
            case SolutionType.FIRE_EXTINGUISHER:
                return 1.0f;
            case SolutionType.TOWEL:
                return 1.0f;
            case SolutionType.BROOM:
                return 1.0f;
            default:
                float duration = 1.0f;
                return Random.Range(duration - randomVariance, duration + randomVariance);
        }
    }

    // Method to access initial location
    public Vector3 getInitialLocation() {
        return initialLocation;
    }

    // Method to play random audio track from usageSound
    public void playUsageSound() {
        if (usageSounds.Length > 0) {
            int randomIndex = Random.Range(0, usageSounds.Length);

            if (speaker != null) {
                AudioClip curClip = usageSounds[randomIndex];
                speaker.clip = curClip;
                speaker.Play();
            }
        }
    }
}
