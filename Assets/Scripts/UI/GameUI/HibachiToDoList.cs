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

            StartCoroutine(destroyRestaurantSequence());
        } else {
            taskLabels[initialTasks.Count].color = notFinishedColor;
        }
    }

    // Private IEnumerator that does the destruction sequence
    private IEnumerator destroyRestaurantSequence() {
        RatController3D player = FindObjectOfType<RatController3D>();
        player.makeInvincible();

        // Disable chefs and black out
        restaurantDestroyedEvent.Invoke();
        yield return new WaitForSeconds(0.2f);
        yield return blackOutSequence(0.75f);

        yield return new WaitForSeconds(0.75f);
        GetComponent<SceneChanger>().ChangeScene("WinScreen");
    }
}
