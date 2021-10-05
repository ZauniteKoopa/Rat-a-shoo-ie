using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;

public class OrderBoard : MonoBehaviour
{
    // Serialized Fields
    [SerializeField]
    private Image mealImage = null;
    [SerializeField]
    private Image[] ingredientImages = null;
    [SerializeField]
    private Recipe shownRecipe = null;
    private LevelInfo levelInfo = null;

    // On awake, get LevelInfo
    private void Start() {
        levelInfo = FindObjectOfType<LevelInfo>();
        showRecipe(shownRecipe);
    }
    
    // Method to set recipe up
    public void showRecipe(Recipe recipe) {
        Debug.Assert(ingredientImages.Length >= recipe.getNumSteps());

        // Show the meal
        mealImage.sprite = recipe.foodSprite;

        // Show the ingredients individually
        for (int i = 0; i < ingredientImages.Length; i++) {
            if (i < recipe.getNumSteps()) {
                ingredientImages[i].gameObject.SetActive(true);
                Sprite ingredientSprite = levelInfo.getThoughtSprite(recipe.getSolutionStep(i));
                ingredientImages[i].sprite = ingredientSprite;
            } else {
                ingredientImages[i].gameObject.SetActive(false);
            }
        }
    }
}
