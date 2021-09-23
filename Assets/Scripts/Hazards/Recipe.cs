using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Recipe : ScriptableObject
{
    [SerializeField]
    private List<SolutionType> ingredientSteps = null;

    [Header("Preferences")]
    [SerializeField]
    private bool resistSpicy = false;
    [SerializeField]
    private bool resistPoison = false;


    // Public method to get the total number of steps
    public int getNumSteps() {
        return ingredientSteps.Count;
    }

    // Public method to get the SolutionType at that step
    public SolutionType getSolutionStep(int i) {
        return ingredientSteps[i];
    }

    // Public method to check if recipe was ruined
    public bool isRecipeRuined(FoodInstance food) {
        return (!resistPoison && food.poisoned) || (!resistSpicy && food.spicy);
    }
}
