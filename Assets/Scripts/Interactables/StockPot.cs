using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StockPot : MonoBehaviour
{
    // Public methods
    private bool poisoned = false;
    public UnityEvent poisonMealEvent;
    [SerializeField]
    private int numPoisonIngredientsReq = 1;

    
    // on trigger enter
    private void OnTriggerEnter(Collider collider) {
        if (!poisoned && collider.tag == "BadFood") {
            numPoisonIngredientsReq--;
            Object.Destroy(collider.gameObject);

            if (numPoisonIngredientsReq <= 0) {
                poisonMealEvent.Invoke();
                Object.Destroy(gameObject);
            }
        }
    }

}
