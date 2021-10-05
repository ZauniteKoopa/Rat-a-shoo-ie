using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OrderWindow : MonoBehaviour
{
    private Recipe currentRecipe = null;
    private LevelInfo levelInfo = null;
    private OrderWindowAudioManager audioManager = null;

    public UnityEvent poisonMealEvent;

    // On awake, get a recipe from the level info
    private void Awake() {
        levelInfo = FindObjectOfType<LevelInfo>();
        currentRecipe = levelInfo.pickRandomMenuItem();
        audioManager = GetComponent<OrderWindowAudioManager>();
    }
    
    // Public method to serve food to potential customers
    public void serveFood(FoodInstance servedFood) {
        if (currentRecipe.isRecipeRuined(servedFood)) {
            audioManager.playBarfSound();
            poisonMealEvent.Invoke();
        } else {
            Debug.Log("customer is content");
        }

        currentRecipe = levelInfo.pickRandomMenuItem();
    }

    // Public method to access the current recipe
    public Recipe getCurrentRecipe() {
        return currentRecipe;
    }
}
