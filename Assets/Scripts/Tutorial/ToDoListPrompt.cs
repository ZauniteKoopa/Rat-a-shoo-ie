using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ToDoListPrompt : MonoBehaviour
{

    // Event handler method when task list is shown
    public void onTaskListPress(InputAction.CallbackContext value) {
        Object.Destroy(gameObject);
    }
}
