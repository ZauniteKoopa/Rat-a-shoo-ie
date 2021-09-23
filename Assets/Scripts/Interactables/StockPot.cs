using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StockPot : MonoBehaviour
{
    private FoodInstance currentCookingMeal = null;

    // On awake, set up new food instance
    private void Awake() {
        currentCookingMeal = new FoodInstance();
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

            Object.Destroy(collider.gameObject);
        }
    }

    // Method used by the chef to take the meal
    //  When this method is called currentCookedMeal will be reset to an empty, clean meal / food instance
    public FoodInstance takeCompletedMeal() {
        FoodInstance completedMeal = currentCookingMeal;
        currentCookingMeal = new FoodInstance();
        return completedMeal;
    }

}
