using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CustomerFinishedDelegate : UnityEvent<int> {};

public class Customer : MonoBehaviour
{
    public Recipe orderedMeal = null;
    public int customerIndex = 0;
    private CustomerFinishedDelegate customerFinishedEvent;

    // On awake, set up events
    void Awake() {
        customerFinishedEvent = new CustomerFinishedDelegate();
        LevelInfo levelInfo = FindObjectOfType<LevelInfo>();
        customerFinishedEvent.AddListener(levelInfo.onCustomerLeave);
    }

    // Method to eat food
    public void finishFood() {
        customerFinishedEvent.Invoke(customerIndex);
    }
}
