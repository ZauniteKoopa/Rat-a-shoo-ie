using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using UnityEngine.InputSystem;

// List of task types that can be used
public enum TaskType {
    POISON_SOUP,
    MAKE_FIRE,
    MAKE_MESS,
    TUTORIAL_MOVE,
    TUTORIAL_JUMP,
    TUTORIAL_CLOSET,
    TUTORIAL_SPRINT,
    FIND_ROOM
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
    protected List<TMP_Text> taskLabels = null;
    [SerializeField]
    private Image indicator = null;
    [SerializeField]
    private GameObject pauseMenu = null;
    [SerializeField]
    private TMP_Text warningUI = null;
    [SerializeField]
    protected Color notFinishedColor = Color.red;
    [SerializeField]
    private Image blackOutImage = null;
    [SerializeField]
    private GameObject androidControls = null;
    private bool warningActive = false;
    private const float DELTA_TIME = 0.04f;

    // Input management variables for TaskList
    private bool taskListShown = false;
    private bool taskListPressed = false;

    // Task management
    [Header("Task Management")]
    [SerializeField]
    protected List<TaskType> initialTasks = null;
    [SerializeField]
    private List<int> taskRevealSequence = null;
    private Queue<int> taskRevealQueue;
    private int numTasksLeft = 0;
    protected bool canEscape = false;

    // Pause management
    private bool paused = false;
    private bool pauseButtonPressed = false;

    //Reference variables
    private UIAudioManager audioManager = null;
    protected LevelInfo levelInfo = null;

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

        // If on android, enable android on screen controls
        if (Application.platform == RuntimePlatform.Android) {
            androidControls.SetActive(true);
        }

        initializeList();
        additionalInitialization();
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
                taskLabels[i].text = "Burn up things!";
            } else if (initialTasks[i] == TaskType.MAKE_MESS) {
                taskLabels[i].text = "Break big fragile things!";
            } else if (initialTasks[i] == TaskType.TUTORIAL_MOVE) {
                string movementControls = (Application.platform == RuntimePlatform.Android) ? "left stick" : "WASD";
                taskLabels[i].text = "Move with " + movementControls + "!";
            } else if (initialTasks[i] == TaskType.TUTORIAL_JUMP) {
                string jumpControls = (Application.platform == RuntimePlatform.Android) ? "JUMP button" : "SPACEBAR";
                taskLabels[i].text = "Jump with da " + jumpControls + "!";
            } else if (initialTasks[i] == TaskType.TUTORIAL_CLOSET) {
                taskLabels[i].text = "Hide chef's ingredients!";
            } else if (initialTasks[i] == TaskType.TUTORIAL_SPRINT) {
                string sprintControls = (Application.platform == RuntimePlatform.Android) ? "SPRINT button" : "Left-Shift";
                taskLabels[i].text = "Make big jump by sprinting! (" + sprintControls + ")";
            } else if (initialTasks[i] == TaskType.FIND_ROOM) {
                taskLabels[i].text = "Find da main kitchen!";
            }

            // If the task is NOT revealed, then hide it
            if (i >= initialRevealedTasks) {
                taskLabels[i].gameObject.SetActive(false);
            }
        }

        numTasksLeft = initialTasks.Count;
    }

    // Method to do additional initialization if needed in subclases
    protected virtual void additionalInitialization() {}

    // Main event handler for handling the task list
    public void onTaskList(InputAction.CallbackContext value) {
        if (value.started && !taskListShown) {
            taskListPressed = true;
            audioManager.playUISounds(0);
            StartCoroutine(lookAtList());
        } else if (value.canceled) {
            taskListPressed = false;
        }
    }

    // Main event handler for the pause button
    public void onPause(InputAction.CallbackContext value) {
        if (!paused && value.started) {
            StartCoroutine(pauseMenuSequence());
        }

        if (value.started) {
            pauseButtonPressed = true;
        } else if(value.canceled) {
            pauseButtonPressed = false;
        }
    }

    // Main event handler for the pause mobile button
    public void onPauseMobileButtonPress() {
        if (!paused) {
            StartCoroutine(pauseMenuSequence());
        }
    }

    // Main IEnumerator process to look at list (ONLY ON PC)
    private IEnumerator lookAtList() {
        yield return bringListUp();
        WaitForSecondsRealtime wait = new WaitForSecondsRealtime(DELTA_TIME);

        // Only remove the list if player takes hands off shift key
        while(taskListPressed) {
            yield return wait;
        }

        // Take list away
        yield return putListAway();
    }

    // Main private helper method to simply bring the list up
    private IEnumerator bringListUp() {
        taskListShown = true;
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
    }

    // Main private helper method to put the list away
    private IEnumerator putListAway() {
        taskListShown = false;
        Time.timeScale = 1.0f;
        Vector3 targetPosition = new Vector3(inViewX, 0, 0);
        Vector3 sourcePosition = new Vector3(outOfViewX, 0, 0);
        indicator.gameObject.SetActive(false);
        float timer = 0f;
        WaitForSecondsRealtime wait = new WaitForSecondsRealtime(DELTA_TIME);

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
        while (paused && pauseButtonPressed) {
            yield return wait;
        }

        // Main pause menu loop that considers user input
        while (paused) {
            yield return wait;

            if (pauseButtonPressed) {
                paused = false;
            }
        }

        Time.timeScale = 1.0f;
        pauseButtonPressed = false;
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

    // Main event handler method for when player swipes mobile device left
    public void onPlayerSwipeLeft() {
        if (taskListShown) {
            StartCoroutine(putListAway());
        }
    }

    // Main event handler method for when player swipes mobile device right
    public void onPlayerSwipeRight() {
        if (!taskListShown) {
            audioManager.playUISounds(0);
            StartCoroutine(bringListUp());
        }
    }

    // Method when player tries to escape
    public void onPlayerEscape() {
        if (canEscape) {
            levelInfo.saveLevelSuccess();
            GetComponent<SceneChanger>().takeToVictoryScreen(levelInfo.getLevelIndex());
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

    // Private IEnumerator to do a fade to clear sequence
    public IEnumerator fadeClearSequence(float fadeTime) {
        float timer = 0.0f;
        WaitForEndOfFrame waitFrame = new WaitForEndOfFrame();

        // Slow fade to black
        while (timer <= fadeTime) {
            yield return waitFrame;
            timer += Time.deltaTime;

            float progress = timer / fadeTime;
            blackOutImage.color = Color.Lerp(Color.black, Color.clear, progress);
        }

        blackOutImage.color = Color.clear;
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
            found = curType == taskType && taskLabels[i].fontStyle != FontStyles.Strikethrough;
        }

        // If found, modify the associated task in the to-do list
        if (found) {
            taskLabels[i].fontStyle = FontStyles.Strikethrough;
            taskLabels[i].color = Color.black;
            numTasksLeft--;

            activateIndicator();

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
                setUpLastTask();
            }
        }
    }

    // Protected virtual method used to set up the last task once to-do list is done
    protected virtual void setUpLastTask() {
        taskLabels[initialTasks.Count].text = "ESCAPE!!!";
        taskLabels[initialTasks.Count].color = notFinishedColor;
        canEscape = true;
        playerFinishAllTasksEvent.Invoke();
    }

    // Protected method to activate the indicator
    protected void activateIndicator() {
        audioManager.playUISounds(5);
        indicator.gameObject.SetActive(true);
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
