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
    private const float DELTA_TIME = 0.04f;

    // On start, initialize the list
    private void Start() {
        initializeList();
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
    }
    
    // Main method to checks if someone wants to look at the list
    void Update() {
        if (Input.GetButtonDown("Fire3")) {
            StartCoroutine(lookAtList());
        }
    }

    // Main IEnumerator process to look at list
    private IEnumerator lookAtList() {
        Time.timeScale = 0.0f;
        Vector3 targetPosition = new Vector3(inViewX, 0, 0);
        Vector3 sourcePosition = new Vector3(outOfViewX, 0, 0);
        float timer = 0f;
        WaitForSecondsRealtime wait = new WaitForSecondsRealtime(DELTA_TIME);

        // Bringing the list up
        while (timer < transitionTime) {
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
}
