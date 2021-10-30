using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Assertions;

public class LevelInfo : MonoBehaviour
{
    // Main dictionaries to keep track of items
    private Dictionary<SolutionType, List<Vector3>> solutionLocations;
    private Dictionary<SolutionType, Sprite> thoughtPool;
    private Dictionary<SolutionType, Transform> closetDictionary;
    private ResetInteractable[] resetInteractables = null;

    // Event handling for when player is attacking versus when player is safe
    private int numNPCsChasing = 0;
    public UnityEvent onPlayerAttacked;
    public UnityEvent onPlayerSafe;
    [SerializeField]
    private int levelIndex = 0;

    // Initialized variables for every levels
    [SerializeField]
    private SolutionTypeSpriteMap initialThoughtMap = null;
    [SerializeField]
    private Recipe[] foodMenu = null;

    // Events that happens to all the chef
    public UnityEvent restaurantDestroyedEvent;
    public UnityEvent destroyChefsEvent;


    // Run before the first frame
    private void Awake() {
        solutionLocations = new Dictionary<SolutionType, List<Vector3>>();
        SolutionObject[] allSolutions = Object.FindObjectsOfType<SolutionObject>();
        resetInteractables = Object.FindObjectsOfType<ResetInteractable>();

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

        // Set up thought pool
        thoughtPool = initialThoughtMap.getThoughtBubbleDictionary();
        closetDictionary = initialThoughtMap.getSolutionPrefabDictionary();

        // Connect all chefs AI to the player when player takes damage
        RatController3D ratPlayer = Object.FindObjectOfType<RatController3D>();
        Chef[] enemyChefs = Object.FindObjectsOfType<Chef>();

        foreach (Chef chef in enemyChefs) {
            ratPlayer.playerHealthLossEvent.AddListener(chef.onRatDamaged);
        }

    }

    // Method to get the positions of all possible solution of solutionType
    public List<Vector3> getPositions(SolutionType solutionType) {
        return (solutionLocations.ContainsKey(solutionType)) ? solutionLocations[solutionType] : new List<Vector3>();
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

    // Public method to access the prefab with the associated solution type
    public Transform getSolutionPrefab(SolutionType solutionType) {
        return closetDictionary[solutionType];
    }

    // Public method to save the level when player wins
    public void saveLevelSuccess() {
        PersistentData.instance.saveVictory(levelIndex);
    }

    // Public method to reset all reset interactables
    public void resetLevel() {
        foreach(ResetInteractable interactable in resetInteractables) {
            if (interactable != null) {
                interactable.reset();
            }
        }
    }

    // Main method to trigger all chef event
    public void onRestaurantDestroyed() {
        restaurantDestroyedEvent.Invoke();
    }

    // On Chef destroy
    public void destroyChefs() {
        destroyChefsEvent.Invoke();
    }
}
