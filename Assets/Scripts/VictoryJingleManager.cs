using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictoryJingleManager : MonoBehaviour
{
    [SerializeField]
    private AudioClip[] jingles = null;

    public AudioSource victoryJingles;

    private void Awake()
    {
        if (PersistentData.instance.levelCleared[2])
        {
            AudioClip jingleLevel = jingles[2];
            victoryJingles.clip = jingleLevel;
            victoryJingles.Play();
        } else if (PersistentData.instance.levelCleared[1])
        {
            AudioClip jingleLevel = jingles[1];
            victoryJingles.clip = jingleLevel;
            victoryJingles.Play();
        } else if (PersistentData.instance.levelCleared[0])
        {
            AudioClip jingleLevel = jingles[0];
            victoryJingles.clip = jingleLevel;
            victoryJingles.Play();
        }
        else
        {
            AudioClip jingleLevel = jingles[3];
            victoryJingles.clip = jingleLevel;
            victoryJingles.Play();
        }
    }
}
