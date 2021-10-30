using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class HibachiToDoList : ToDoList
{
    public UnityEvent restaurantDestroyedEvent;

    // Reference variables
    private RestaurantStructure structure = null;
    private RatController3D player = null;
    private CameraController cameraControl = null;

    // Designer specific fields
    [SerializeField]
    private float angryStartCameraShakeDuration = 1.0f;
    [SerializeField]
    private float cameraShakeMagnitude = 0.01f;
    [SerializeField]
    private float pillarDestroyedShakeDuration = 0.4f;
    [SerializeField]
    private float destroyRestaurantScreenDuration = 1.9f;
    [SerializeField]
    private float blackFadeOutDuration = 0.75f;
    [SerializeField]
    private float blackOutDuration = 0.75f;
    private float destroyRestaurantShakeDuration;
    

    // Overriden method to do additional initialization
    protected override void additionalInitialization() {
        player = FindObjectOfType<RatController3D>();
        cameraControl = FindObjectOfType<CameraController>();
        structure = FindObjectOfType<RestaurantStructure>();
        destroyRestaurantShakeDuration = destroyRestaurantScreenDuration + blackFadeOutDuration + 0.2f;

        structure.structureResetEvent.AddListener(onStructureReset);
        structure.pillarDestroyedEvent.AddListener(onPillarDestroyed);
    }

    // Overriden method to set up the last task
    protected override void setUpLastTask() {
        taskLabels[initialTasks.Count].text = "Destroy da restaurant! (0/" + structure.getTotalPillars() + ")";
        taskLabels[initialTasks.Count].color = notFinishedColor;
        shakeCamera(angryStartCameraShakeDuration, cameraShakeMagnitude);
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

            shakeCamera(destroyRestaurantShakeDuration, cameraShakeMagnitude);
            StartCoroutine(destroyRestaurantSequence());
        } else {
            taskLabels[initialTasks.Count].color = notFinishedColor;
            shakeCamera(pillarDestroyedShakeDuration, cameraShakeMagnitude);
        }
    }

    // Event handler method for when the structure resets
    public void onStructureReset() {
        taskLabels[initialTasks.Count].text = "Destroy da restaurant! (" + structure.getPillarsDestroyed() + "/" + structure.getTotalPillars() + ")";
        taskLabels[initialTasks.Count].color = (structure.getPillarsDestroyed() >= structure.getTotalPillars()) ? Color.black : notFinishedColor;
    }

    // Private IEnumerator that does the destruction sequence
    private IEnumerator destroyRestaurantSequence() {
        player.makeInvincible();

        // Disable chefs and black out
        restaurantDestroyedEvent.Invoke();
        yield return new WaitForSeconds(destroyRestaurantScreenDuration);
        yield return blackOutSequence(blackFadeOutDuration);

        yield return new WaitForSeconds(blackOutDuration);
        GetComponent<SceneChanger>().ChangeScene("WinScreen");
    }

    // Main method to shake the camera
    private void shakeCamera(float duration, float magnitude) {
        StartCoroutine(cameraControl.shakeCameraSequence(duration, magnitude));
    }
}
