using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using TMPro;

public class ToDoList : MonoBehaviour
{
    // List of task types that can be used
    public enum TaskType {
        POISON_SOUP
    }

    // Serialized fields
    [SerializeField]
    private Image listImage = null;
    [SerializeField]
    private float transitionTime = 0.3f;
    [SerializeField]
    private float outOfViewX = -600f;
    [SerializeField]
    private float inViewX = -260f;
    [SerializeField]
    private List<TaskType> initialTasks = null;
    [SerializeField]
    private List<TMP_Text> taskLabels = null;
    [SerializeField]
    private Image indicator = null;
    [SerializeField]
    private TMP_Text healthUI = null;
    [SerializeField]
    private GameObject pauseMenu = null;
    [SerializeField]
    private TMP_Text warningUI = null;
    private bool warningActive = false;
    private const float DELTA_TIME = 0.04f;

    // Task management
    private int numTasksLeft = 0;
    private bool canEscape = false;

    // Health management
    private int curHealth = 0;

    // Pause management
    private bool paused = false;

    //Reference variables
    private UIAudioManager audioManager = null;


    // On start, initialize the list
    private void Start() {
        initializeList();
        audioManager = GetComponent<UIAudioManager>();
    }

    // Main method to initialize list: set all tasks to unfinished (colored red) and give their associated text
    private void initializeList() {
        Assert.IsTrue(initialTasks.Count <= taskLabels.Count - 1);

        for (int i = 0; i < initialTasks.Count; i++) {
            taskLabels[i].color = Color.red;

            // Give the text associated with the task type
            if (initialTasks[i] == TaskType.POISON_SOUP) {
                taskLabels[i].text = "- Poison da soup!";
            }
        }

        numTasksLeft = initialTasks.Count;
    }
    
    // Main method to checks if someone wants to look at the list
    void Update() {
        if (Input.GetButtonDown("Fire3")) {
            audioManager.playUISounds(0);
            StartCoroutine(lookAtList());
        } else if (Input.GetButtonDown("Cancel")) {
            StartCoroutine(pauseMenuSequence());
        }
    }

    // Main IEnumerator process to look at list
    private IEnumerator lookAtList() {
        Time.timeScale = 0.0f;
        Vector3 targetPosition = new Vector3(inViewX, 0, 0);
        Vector3 sourcePosition = new Vector3(outOfViewX, 0, 0);
        indicator.gameObject.SetActive(false);
        float timer = 0f;
        WaitForSecondsRealtime wait = new WaitForSecondsRealtime(DELTA_TIME);

        // Bringing the list up
        while (timer < transitionTime) {
            //audioManager.playUISounds(0);
            yield return wait;
            timer += DELTA_TIME;
            listImage.rectTransform.anchoredPosition3D = Vector3.Lerp(sourcePosition, targetPosition, timer / transitionTime);
        }

        listImage.rectTransform.anchoredPosition3D = targetPosition;

        // Only remove the list if player takes hands off shift key
        while(Input.GetButton("Fire3")) {
            yield return wait;
        }

        // Take list away
        timer = 0.0f;
        Time.timeScale = 1.0f;

        while (timer < transitionTime) {
            yield return wait;
            timer += DELTA_TIME;
            listImage.rectTransform.anchoredPosition3D = Vector3.Lerp(targetPosition, sourcePosition, timer / transitionTime);
        }

        listImage.rectTransform.anchoredPosition3D = sourcePosition;
    }

    // Method to open up the pause menu
    private IEnumerator pauseMenuSequence() {
        WaitForSecondsRealtime wait = new WaitForSecondsRealtime(DELTA_TIME);
        Time.timeScale = 0.0f;
        pauseMenu.SetActive(true);
        paused = true;
        yield return new WaitForSecondsRealtime(0.1f);

        // Don't cancel on the same escape button press that paused the menu on the same frame
        while (paused && Input.GetButton("Cancel")) {
            yield return wait;
        }

        // Main pause menu loop that considers user input
        while (paused) {
            yield return wait;

            if (Input.GetButton("Cancel")) {
                paused = false;
            }
        }

        Time.timeScale = 1.0f;
        pauseMenu.SetActive(false);
    }

    // Sequence to display warning
    private IEnumerator warningSequence(string warning) {
        warningActive = true;
        warningUI.text = warning;
        warningUI.gameObject.SetActive(true);

        yield return new WaitForSeconds(2f);

        warningUI.gameObject.SetActive(false);
        warningActive = false;
    }

    // Public method to unpause the game is paused: meant to be used by buttons
    public void unpauseGame() {
        if (paused) {
            paused = false;
        }
    }


    // Event handler for when a task is done
    private void onTaskDone(TaskType taskType) {
        int i = -1;
        bool found = false;

        // Go through list to get the exact position of the task type
        while (i < initialTasks.Count - 1 && !found) {
            i++;

            TaskType curType = initialTasks[i];
            found = curType == taskType;
        }

        // If found, modify the associated task in the to-do list
        if (found) {
            taskLabels[i].color = Color.green;
            numTasksLeft--;

            indicator.gameObject.SetActive(true);

            // If number of tasks is equal to zero, allow escape
            if (numTasksLeft <= 0) {
                taskLabels[initialTasks.Count].text = "- ESCAPE!!!";
                taskLabels[initialTasks.Count].color = Color.red;
                canEscape = true;
            }
        }
    }

    // Wrapper functions for task done event handlers
    public void onPoisonedMeal() {
        onTaskDone(TaskType.POISON_SOUP);
    }

    // Method when player tries to escape
    public void onPlayerEscape() {
        if (canEscape) {
            GetComponent<SceneChanger>().ChangeScene("MainMenu");
        } else {
            if (!warningActive) {
                StartCoroutine(warningSequence("You still haven't finished tasks yet"));
            }
        }
    }

    // Method to update health to a fixed amount
    public void updateHealthUI(int currentHealth) {
        healthUI.text = "Health: " + currentHealth;
        curHealth = currentHealth;
    }

    // Event handler method when player lose health
    public void onPlayerHealthLoss() {
        curHealth--;
        healthUI.text = "Health: " + curHealth;

        if (curHealth <= 0) {
            GetComponent<SceneChanger>().ChangeScene("MainMenu");
        }
    }
}
