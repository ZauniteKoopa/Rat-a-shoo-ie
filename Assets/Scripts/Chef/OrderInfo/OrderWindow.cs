using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Assertions;

// Main customer package class
[System.Serializable]
public class CustomerPackage {
    public Sprite waitingSprite;
    public Sprite happySprite;
    public Sprite poisonedSprite;
}

// Main Order Window class
public class OrderWindow : MonoBehaviour
{
    private Recipe currentRecipe = null;
    private LevelInfo levelInfo = null;
    private OrderWindowAudioManager audioManager = null;

    public UnityEvent poisonMealEvent;
    [Header("Customer Sprites")]
    [SerializeField]
    private SpriteRenderer customerSpriteRender = null;
    [SerializeField]
    private List<CustomerPackage> customerSprites = null;
    private int curCustomerIndex = 0;

    [Header("Customer Timings")]
    [SerializeField]
    private float happyDuration = 0.4f;
    [SerializeField]
    private float customerLeaveDuration = 0.3f;
    [SerializeField]
    private float emptyCustomerWindowDuration = 0.5f;
    [SerializeField]
    private float customerBarfDuration = 0.5f;
    [SerializeField]
    private float customerDeathDuration = 0.5f;

    // On awake, get a recipe from the level info
    private void Awake() {
        levelInfo = FindObjectOfType<LevelInfo>();
        currentRecipe = levelInfo.pickRandomMenuItem();
        audioManager = GetComponent<OrderWindowAudioManager>();

        // Initializing customer
        Debug.Assert(customerSprites.Count > 0);
        curCustomerIndex = Random.Range(0, customerSprites.Count);
        customerSpriteRender.sprite = customerSprites[curCustomerIndex].waitingSprite;

        // Connect to User interface event
        ToDoList toDoList = FindObjectOfType<ToDoList>();
        if (toDoList != null) {
            toDoList.playerFinishAllTasksEvent.AddListener(onToDoListDone);
        }
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
        customerSpriteRender.sprite = customerSprites[curCustomerIndex].happySprite;

        yield return new WaitForSeconds(happyDuration);

        float timer = 0.0f;
        while (timer < customerLeaveDuration) {
            yield return null;

            timer += Time.deltaTime;
            customerSpriteRender.color = Color.Lerp(Color.white, Color.clear, timer / customerLeaveDuration);
        }

        yield return new WaitForSeconds(emptyCustomerWindowDuration);

        // Switch to new customer here
        curCustomerIndex = Random.Range(0, customerSprites.Count);
        customerSpriteRender.sprite = customerSprites[curCustomerIndex].waitingSprite;
        customerSpriteRender.color = Color.white;
    }

    // Main sequence to do customer poisoned animation
    private IEnumerator poisonCustomerSequence() {
        customerSpriteRender.sprite = customerSprites[curCustomerIndex].poisonedSprite;
        customerSpriteRender.color = Color.white;

        yield return new WaitForSeconds(customerBarfDuration);

        // customer dies here
        float timer = 0.0f;
        while (timer < customerDeathDuration) {
            yield return null;
            timer += Time.deltaTime;
            customerSpriteRender.color = Color.Lerp(Color.white, Color.clear, timer / customerDeathDuration);
        }
    }

    // Public method to access the current recipe
    public Recipe getCurrentRecipe() {
        return currentRecipe;
    }

    // On to do list done
    //  all order windows must barf in unison
    public void onToDoListDone() {
        StopAllCoroutines();
        StartCoroutine(poisonCustomerSequence());
    }
}
