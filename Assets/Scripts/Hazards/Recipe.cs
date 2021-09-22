using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Recipe : ScriptableObject
{
    [SerializeField]
    private List<SolutionType> ingredientSteps = null;

    // Public method to get the total number of steps
    public int getNumSteps() {
        return ingredientSteps.Count;
    }

    // Public method to get the SolutionType at that step
    public SolutionType getSolutionStep(int i) {
        return ingredientSteps[i];
    }
}
