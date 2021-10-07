using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        while (!interrupted && Input.GetButton("Interact")) {
            yield return waitFrame;
        }

        // General loop for Interaction
        while (!interrupted && !Input.GetButton("Interact")) {
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

    // When player loses grip on interactable, set interrupted to true
    public override void onPlayerInteractEnd() {
        interrupted = true;
    }
}
