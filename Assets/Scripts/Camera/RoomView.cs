using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RoomView : MonoBehaviour
{
    // Public variables
    public Vector3 localCameraPosition;
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
