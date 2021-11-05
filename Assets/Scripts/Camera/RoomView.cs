using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RoomView : MonoBehaviour
{
    // Public variables: decides the camera's local rotation and position to the room
    public Vector3 localCameraPosition;
    public Vector3 localCameraRotation;
    [SerializeField]
    private Transform spawnPoint = null;
    private bool wasVisited = false;

    // Method to get camera controller
    private CameraController cameraController;

    // On awake, get camera.main
    void Awake() {
        cameraController = FindObjectOfType<CameraController>();
    }

    // Public event method when player enters this room view 
    public void onPlayerEnter(RatController3D player) {
        if (spawnPoint != null && !wasVisited) {
            wasVisited = true;
            player.changeSpawnPoint(spawnPoint);
        }

        cameraController.moveCamera(this);
    }
}
