using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum CookingStationState {
    EMPTY,
    NORMAL,
    ROTTEN
}

public class StockPot : MonoBehaviour
{
    private FoodInstance currentCookingMeal = null;
    private CookingStationState currentState = CookingStationState.EMPTY;
    
    [Header("Sprite states")]
    [SerializeField]
    private SpriteRenderer spriteRender = null;
    [SerializeField]
    private Sprite emptyPot = null;
    [SerializeField]
    private Sprite cookingPot = null;
    [SerializeField]
    private Sprite rottenPot = null;
    private CookingStationAudioManager audioManager = null;

    // On awake, set up new food instance
    private void Awake() {
        currentCookingMeal = new FoodInstance();
        audioManager = GetComponent<CookingStationAudioManager>();
    }

    
    // on trigger enter
    private void OnTriggerEnter(Collider collider) {
        Ingredient ingredient = collider.GetComponent<Ingredient>();

        if (ingredient != null && !ingredient.isHeldByChef) {
            if (ingredient.isSpicy) {
                currentCookingMeal.spicy = true;
            }

            if (ingredient.isRotten) {
                currentCookingMeal.poisoned = true;
            }

            if (currentState != CookingStationState.ROTTEN) {
                audioManager.playRuinedDishSound();
            }

            currentState = CookingStationState.ROTTEN;
            spriteRender.sprite = rottenPot;
            Object.Destroy(collider.gameObject);
        }
    }

    // Method used by the chef to take the meal
    //  When this method is called currentCookedMeal will be reset to an empty, clean meal / food instance
    public FoodInstance takeCompletedMeal() {
        FoodInstance completedMeal = currentCookingMeal;
        currentCookingMeal = new FoodInstance();
        currentState = CookingStationState.EMPTY;
        spriteRender.sprite = emptyPot;

        return completedMeal;
    }

    // Public method for chef add an ingredient
    public void addProperIngredient() {
        if (currentState == CookingStationState.EMPTY) {
            currentState = CookingStationState.NORMAL;
            spriteRender.sprite = cookingPot;
        }
    }
}
