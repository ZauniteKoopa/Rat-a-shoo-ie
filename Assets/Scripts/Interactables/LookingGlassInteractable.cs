using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LookingGlassInteractable : GeneralInteractable
{
    // Camera positions
    public Vector3 cameraPosition = Vector3.zero;
    public Vector3 cameraRotation = Vector3.zero;
    [SerializeField]
    private float fadeTime = 0.3f;

    // Reference Variables
    private ToDoList userInterface = null;
    private bool interrupted = false;
    private bool interactButtonPressed = false;

    // On awake, get access to the user interface
    private void Awake() {
        userInterface = FindObjectOfType<ToDoList>();
    }

    // Overriden interactable sequence
    public override IEnumerator interactionSequence() {
        // Get previous camera properties
        interrupted = false;
        Transform cameraTransform = Camera.main.transform;
        Vector3 prevCameraPosition = cameraTransform.position;
        Vector3 prevCameraRotation = cameraTransform.eulerAngles;

        // Transition camera to world view
        yield return userInterface.blackOutSequence(fadeTime);

        cameraTransform.position = cameraPosition;
        cameraTransform.eulerAngles = cameraRotation;

        yield return new WaitForSeconds(0.2f);
        yield return userInterface.fadeClearSequence(fadeTime);

        // Main loop
        WaitForEndOfFrame waitFrame = new WaitForEndOfFrame();

        // One loop for the initial press just in case player holds E
        while (!interrupted && interactButtonPressed) {
            yield return waitFrame;
        }

        // General loop for Interaction
        while (!interrupted && !interactButtonPressed) {
            yield return waitFrame;
        }

        // If not interrupted, do fade in black sequence and fade out. Else, just immediately show rat dying
        if (!interrupted) {
            yield return userInterface.blackOutSequence(fadeTime);
        }

        cameraTransform.position = prevCameraPosition;
        cameraTransform.eulerAngles = prevCameraRotation;

        if (!interrupted) {
            yield return new WaitForSeconds(0.1f);
            yield return userInterface.fadeClearSequence(fadeTime);
        }
    }

    // When player presses interact button
    public void onInteractButton(InputAction.CallbackContext value) {
        if (value.started) {
            interactButtonPressed = true;
        } else if (value.canceled) {
            interactButtonPressed = false;
        }
    }

    // When player loses grip on interactable, set interrupted to true
    public override void onPlayerInteractEnd() {
        interrupted = true;
    }
}
