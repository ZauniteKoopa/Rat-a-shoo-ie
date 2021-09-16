using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ThoughtBubble : MonoBehaviour
{
    [SerializeField]
    private Image solutionIconSlot = null;
    [SerializeField]
    private SolutionTypeSpriteMap thoughtsPool = null;

    // Public method to set up thought bubble
    public void thinkOfSolution(SolutionObject solution) {
        solutionIconSlot.sprite = solution.getThoughtSprite();
        gameObject.SetActive(true);
    }

    // Public method to clear up sprites
    public void clearUpThoughts() {
        gameObject.SetActive(false);
    }
}
