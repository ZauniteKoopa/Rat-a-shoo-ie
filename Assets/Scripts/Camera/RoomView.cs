using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RoomView : MonoBehaviour
{
    // Public variables: decides the camera's local rotation and position to the room
    public Vector3 localCameraPosition;
    public Vector3 localCameraRotation;

    // Method to get camera controller
    private CameraController cameraController;

    // On awake, get camera.main
    void Start() {
        cameraController = Camera.main.transform.GetComponent<CameraController>();
    }

    // Public event method when player enters this room view 
    public void onPlayerEnter() {
        cameraController.moveCamera(this);
    }
}
