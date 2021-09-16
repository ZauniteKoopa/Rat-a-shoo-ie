using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ThoughtBubble : MonoBehaviour
{
    [SerializeField]
    private Image solutionIconSlot = null;
    public LevelInfo mainLevel = null;

    // Public method to set up thought bubble
    public void thinkOfSolution(SolutionType solutionThought) {
        solutionIconSlot.sprite = mainLevel.getThoughtSprite(solutionThought);
        gameObject.SetActive(true);
    }

    // Public method to clear up sprites
    public void clearUpThoughts() {
        gameObject.SetActive(false);
    }
}
