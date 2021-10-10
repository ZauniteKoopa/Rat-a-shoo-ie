using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class ToDoListPrompt : MonoBehaviour
{
    [SerializeField]
    private TMP_Text prompt = null;

    // On awake, change the text of the todo list prompt
    private void Awake() {
        string toDoListAction = (Application.platform == RuntimePlatform.Android) ? "Swipe left" : "Press C";
        prompt.text = toDoListAction + " to Look at Tasks!";
    }

    // Event handler method when task list is shown
    public void onTaskListPress(InputAction.CallbackContext value) {
        Object.Destroy(gameObject);
    }
}
