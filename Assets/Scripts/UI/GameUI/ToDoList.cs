﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

// List of task types that can be used
public enum TaskType {
    POISON_SOUP,
    MAKE_FIRE,
    MAKE_MESS,
    TUTORIAL_MOVE,
    TUTORIAL_JUMP,
    TUTORIAL_CLOSET
}

public class ToDoList : MonoBehaviour
{

    // Serialized fields
    [Header("Serialized UI Elements")]
    [SerializeField]
    private Image listImage = null;
    [SerializeField]
    private float transitionTime = 0.3f;
    [SerializeField]
    private float outOfViewX = -600f;
    [SerializeField]
    private float inViewX = -260f;
    [SerializeField]
    private List<TMP_Text> taskLabels = null;
    [SerializeField]
    private Image indicator = null;
    [SerializeField]
    private GameObject pauseMenu = null;
    [SerializeField]
    private TMP_Text warningUI = null;
    [SerializeField]
    private Color notFinishedColor = Color.red;
    [SerializeField]
    private Image blackOutImage = null;
    private bool warningActive = false;
    private const float DELTA_TIME = 0.04f;

    // Task management
    [Header("Task Management")]
    [SerializeField]
    private List<TaskType> initialTasks = null;
    [SerializeField]
    private List<int> taskRevealSequence = null;
    private Queue<int> taskRevealQueue;
    private int numTasksLeft = 0;
    private bool canEscape = false;

    // Pause management
    private bool paused = false;

    //Reference variables
    private UIAudioManager audioManager = null;
    private LevelInfo levelInfo = null;

    // Unity events
    public UnityEvent playerFinishAllTasksEvent;


    // On start, initialize the list and the queue
    private void Start() {
        levelInfo = FindObjectOfType<LevelInfo>();
        audioManager = GetComponent<UIAudioManager>();
        taskRevealQueue = new Queue<int>();

        if (taskRevealSequence != null) {
            foreach(int breakPoint in taskRevealSequence) {
                taskRevealQueue.Enqueue(breakPoint);
            }
        }

        initializeList();
    }

    // Main method to initialize list: set all tasks to unfinished (colored red) and give their associated text
    private void initializeList() {
        Assert.IsTrue(initialTasks.Count <= taskLabels.Count - 1);
        int initialRevealedTasks = (taskRevealQueue.Count <= 0) ? initialTasks.Count : Mathf.Min(taskRevealQueue.Peek(), initialTasks.Count);

        for (int i = 0; i < initialTasks.Count; i++) {
            taskLabels[i].color = notFinishedColor;

            // Give the text associated with the task type
            if (initialTasks[i] == TaskType.POISON_SOUP) {
                taskLabels[i].text = "Poison da customers!";
            } else if (initialTasks[i] == TaskType.MAKE_FIRE) {
                taskLabels[i].text = "Burn up property!";
            } else if (initialTasks[i] == TaskType.MAKE_MESS) {
                taskLabels[i].text = "Make mess!";
            } else if (initialTasks[i] == TaskType.TUTORIAL_MOVE) {
                taskLabels[i].text = "Move with WASD!";
            } else if (initialTasks[i] == TaskType.TUTORIAL_JUMP) {
                taskLabels[i].text = "Jump with the SPACEBAR!";
            } else if (initialTasks[i] == TaskType.TUTORIAL_CLOSET) {
                taskLabels[i].text = "Hide the chef's ingredients!";
            }

            // If the task is NOT revealed, then hide it
            if (i >= initialRevealedTasks) {
                taskLabels[i].gameObject.SetActive(false);
            }
        }

        numTasksLeft = initialTasks.Count;
    }
    
    // Main method to checks if someone wants to look at the list
    void Update() {
        if (Input.GetButtonDown("TaskList")) {
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
        while(Input.GetButton("TaskList")) {
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
        audioManager.playUISounds(3);
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
        audioManager.playUISounds(4);
        pauseMenu.SetActive(false);
    }

    // Sequence to display warning
    private IEnumerator warningSequence(string warning) {
        warningActive = true;
        warningUI.text = warning;
        warningUI.transform.parent.gameObject.SetActive(true);

        yield return new WaitForSeconds(2f);

        warningUI.transform.parent.gameObject.SetActive(false);
        warningActive = false;
    }

    // Public method to unpause the game is paused: meant to be used by buttons
    public void unpauseGame() {
        if (paused) {
            paused = false;
        }
    }

    // Method when player tries to escape
    public void onPlayerEscape() {
        if (canEscape) {
            levelInfo.saveLevelSuccess();
            GetComponent<SceneChanger>().ChangeScene("WinScreen");
        } else {
            if (!warningActive) {
                StartCoroutine(warningSequence("You still haven't finished tasks yet"));
            }
        }
    }

    // Private IEnumerator to do black out sequence when player gets hit
    public IEnumerator blackOutSequence(float fadeTime) {
        float timer = 0.0f;
        WaitForEndOfFrame waitFrame = new WaitForEndOfFrame();

        // Slow fade to black
        while (timer <= fadeTime) {
            yield return waitFrame;
            timer += Time.deltaTime;

            float progress = timer / fadeTime;
            blackOutImage.color = Color.Lerp(Color.clear, Color.black, progress);
        }

        blackOutImage.color = Color.black;
    }

    // Public method to blink back to consciousness
    public void blinkBack() {
        blackOutImage.color = Color.clear;
    }

    // Event handler for when a task is done
    public void onTaskDone(TaskType taskType) {
        int i = -1;
        int tasksToCheck = (taskRevealQueue.Count <= 0) ? initialTasks.Count : Mathf.Min(taskRevealQueue.Peek(), initialTasks.Count);
        bool found = false;

        // Go through list to get the exact position of the task type
        while (i < tasksToCheck - 1 && !found) {
            i++;

            TaskType curType = initialTasks[i];
            found = curType == taskType;
        }

        // If found, modify the associated task in the to-do list
        if (found) {
            taskLabels[i].fontStyle = FontStyles.Strikethrough;
            taskLabels[i].color = Color.black;
            numTasksLeft--;
            audioManager.playUISounds(5);

            indicator.gameObject.SetActive(true);

            // If the player did the number of tasks indicated by the task reveal queue, update the task reveal queue
            if (taskRevealQueue.Count > 0 && (initialTasks.Count - numTasksLeft) >= taskRevealQueue.Peek()) {
                // Reveal the remaining tasks
                int firstRevealedTask = taskRevealQueue.Dequeue();
                int lastRevealedTask = (taskRevealQueue.Count > 0) ? Mathf.Min(taskRevealQueue.Peek(), initialTasks.Count) : initialTasks.Count;

                for (int t = firstRevealedTask; t < lastRevealedTask; t++) {
                    taskLabels[t].gameObject.SetActive(true);
                }
            }

            // If number of tasks is equal to zero, allow escape
            if (numTasksLeft <= 0) {
                taskLabels[initialTasks.Count].text = "ESCAPE!!!";
                taskLabels[initialTasks.Count].color = notFinishedColor;
                canEscape = true;
                playerFinishAllTasksEvent.Invoke();
            }
        }
    }

    // Wrapper functions for task done event handlers
    public void onPoisonedMeal() {
        onTaskDone(TaskType.POISON_SOUP);
    }

    public void onMadeFire() {
        onTaskDone(TaskType.MAKE_FIRE);
    }

    public void onMadeMess() {
        onTaskDone(TaskType.MAKE_MESS);
    }

    // The int is based off of the enum order, because unity can't serialize enums for events for some stupid reason
    public void onTaskDoneWrapper(int enumInt) {
        onTaskDone((TaskType)enumInt);
    }
}