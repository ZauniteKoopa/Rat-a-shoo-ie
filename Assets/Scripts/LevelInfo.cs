using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Assertions;

public class LevelInfo : MonoBehaviour
{
    private Dictionary<SolutionType, List<Vector3>> solutionLocations;
    private int numNPCsChasing = 0;
    private Dictionary<SolutionType, Sprite> thoughtPool;

    public UnityEvent onPlayerAttacked;
    public UnityEvent onPlayerSafe;
    [SerializeField]
    private SolutionTypeSpriteMap initialThoughtMap = null;
    [SerializeField]
    private Recipe[] foodMenu = null;
    private Customer[] customers = null;
    private Queue<Customer> orderQueue = null;


    // Run before the first frame
    private void Awake() {
        solutionLocations = new Dictionary<SolutionType, List<Vector3>>();
        SolutionObject[] allSolutions = Object.FindObjectsOfType<SolutionObject>();
        customers = Object.FindObjectsOfType<Customer>();
        orderQueue = new Queue<Customer>();

        // For each solution
        foreach(SolutionObject solution in allSolutions) {
            SolutionType curType = solution.solutionType;
            Vector3 initialLocation = solution.transform.position;

            // If not found in dictionary yet, make a new list
            if (!solutionLocations.ContainsKey(curType)) {
                List<Vector3> newPositions = new List<Vector3>();
                solutionLocations[curType] = newPositions; 
            }

            // Add initial position to the list
            solutionLocations[curType].Add(initialLocation);
        }

        // For each customer in customers
        int i = 0;
        foreach(Customer customer in customers) {
            customer.customerIndex = i;
            customer.orderedMeal = pickRandomMenuItem();
            orderQueue.Enqueue(customer);
            i++;
        }

        // Set up thought pool
        thoughtPool = initialThoughtMap.getThoughtBubbleDictionary();
    }

    // Method to get the positions of all possible solution of solutionType
    public List<Vector3> getPositions(SolutionType solutionType) {
        return solutionLocations[solutionType];
    }

    // Public method to update that someone's chasing the player
    public void onChefChaseStart() {
        if (numNPCsChasing == 0) {
            onPlayerAttacked.Invoke();
        }

        numNPCsChasing++;
    }

    // Public method to update someone losing the player
    public void onChefChaseEnd() {
        numNPCsChasing--;

        if (numNPCsChasing == 0) {
            onPlayerSafe.Invoke();
        }
    }

    // Public method to access from thought bubble dictionary
    public Sprite getThoughtSprite(SolutionType solutionType) {
        return thoughtPool[solutionType];
    }

    // Public method to get random food from menu
    public Recipe pickRandomMenuItem() {
        int i = Random.Range(0, foodMenu.Length);
        return foodMenu[i];
    }

    // Public method to get the next order
    public Customer getNextOrder() {
        return orderQueue.Dequeue();
    }

    // Event handler when a customer's is leaving
    public void onCustomerLeave(int customerIndex) {
        StartCoroutine(customerLeaveSequence(customerIndex));
    }

    // IEnumerator for customer leave sequence
    private IEnumerator customerLeaveSequence(int customerIndex) {
        customers[customerIndex].gameObject.SetActive(false);
        yield return new WaitForSeconds(1.0f);

        customers[customerIndex].gameObject.SetActive(true);
        customers[customerIndex].orderedMeal = pickRandomMenuItem();
        orderQueue.Enqueue(customers[customerIndex]);

    }

}
