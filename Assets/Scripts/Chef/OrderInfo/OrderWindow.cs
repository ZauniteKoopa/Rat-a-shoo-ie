using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum CustomerStatus {
    WAITING,
    CONTENT,
    POISONED
}

public class OrderWindow : MonoBehaviour
{
    private Recipe currentRecipe = null;
    private LevelInfo levelInfo = null;
    private OrderWindowAudioManager audioManager = null;

    private CustomerStatus customerStatus = CustomerStatus.WAITING;
    public UnityEvent poisonMealEvent;
    [SerializeField]
    private SpriteRenderer customerSprite = null;
    [SerializeField]
    private Sprite[] customer1Sprites = null;

    // On awake, get a recipe from the level info
    private void Awake() {
        levelInfo = FindObjectOfType<LevelInfo>();
        currentRecipe = levelInfo.pickRandomMenuItem();
        audioManager = GetComponent<OrderWindowAudioManager>();

        // Initializing customer
        customerStatus = CustomerStatus.WAITING;
        customerSprite.sprite = customer1Sprites[(int)customerStatus];
    }
    
    // Public method to serve food to potential customers
    public void serveFood(FoodInstance servedFood) {
        if (currentRecipe.isRecipeRuined(servedFood)) {
            audioManager.playBarfSound();
            poisonMealEvent.Invoke();
            StartCoroutine(poisonCustomerSequence());
        } else {
            audioManager.playYummySound();
            StartCoroutine(switchToNewCustomer());
        }

        currentRecipe = levelInfo.pickRandomMenuItem();
    }

    // Main sequence to switch to new customer
    private IEnumerator switchToNewCustomer() {
        customerStatus = CustomerStatus.CONTENT;
        customerSprite.sprite = customer1Sprites[(int)customerStatus];

        yield return new WaitForSeconds(0.2f);

        float timer = 0.0f;
        while (timer < 0.3f) {
            yield return null;

            timer += Time.deltaTime;
            customerSprite.color = Color.Lerp(Color.white, Color.clear, timer / 0.3f);
        }

        yield return new WaitForSeconds(0.1f);

        customerSprite.color = Color.white;

        // Switch to new customer here
        customerStatus = CustomerStatus.WAITING;
        customerSprite.sprite = customer1Sprites[(int)customerStatus];
    }

    // Main sequence to do customer poisoned animation
    private IEnumerator poisonCustomerSequence() {
        customerStatus = CustomerStatus.POISONED;
        customerSprite.sprite = customer1Sprites[(int)customerStatus];
        customerSprite.color = Color.white;

        // customer dies here
        float timer = 0.0f;
        while (timer < 0.5f) {
            yield return null;
            timer += Time.deltaTime;
            customerSprite.color = Color.Lerp(Color.white, Color.clear, timer / 0.5f);
        }
    }

    // Public method to access the current recipe
    public Recipe getCurrentRecipe() {
        return currentRecipe;
    }
}
