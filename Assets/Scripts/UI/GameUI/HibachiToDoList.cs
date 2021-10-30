using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class HibachiToDoList : ToDoList
{
    public UnityEvent restaurantDestroyedEvent;
    [SerializeField]
    private RestaurantStructure structure = null;

    // Overriden method to set up the last task
    protected override void setUpLastTask() {
        taskLabels[initialTasks.Count].text = "Destroy da restaurant! (0/" + structure.getTotalPillars() + ")";
        taskLabels[initialTasks.Count].color = notFinishedColor;
        playerFinishAllTasksEvent.Invoke();
    }

    // Public event handler method
    public void onPillarDestroyed() {
        activateIndicator();
        taskLabels[initialTasks.Count].text = "Destroy da restaurant! (" + structure.getPillarsDestroyed() + "/" + structure.getTotalPillars() + ")";
        taskLabels[initialTasks.Count].color = (structure.getPillarsDestroyed() >= structure.getTotalPillars()) ? Color.black : notFinishedColor;

        if (structure.getPillarsDestroyed() >= structure.getTotalPillars()) {
            taskLabels[initialTasks.Count].color = Color.black;
            taskLabels[initialTasks.Count].fontStyle = FontStyles.Strikethrough;

            restaurantDestroyedEvent.Invoke();
            taskLabels[initialTasks.Count + 1].text = "Escape!!!!!";
            taskLabels[initialTasks.Count + 1].color = notFinishedColor;
            canEscape = true;
        } else {
            taskLabels[initialTasks.Count].color = notFinishedColor;
        }
    }
}
