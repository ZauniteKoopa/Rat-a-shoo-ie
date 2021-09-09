using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private float CAMERA_MOVE_TIME = 0.4f;

    // Main method to move the camera
    public void moveCamera(RoomView roomView) {
        StopAllCoroutines();
        StartCoroutine(moveCameraSequence(roomView));
    }

    // private IEnumerator to move the camera
    private IEnumerator moveCameraSequence(RoomView roomView) {
        transform.parent = roomView.transform;
        Vector3 startLocalPosition = transform.localPosition;
        Vector3 endLocalPosition = roomView.localCameraPosition;

        float timer = 0f;
        WaitForEndOfFrame wait = new WaitForEndOfFrame();

        while (timer < CAMERA_MOVE_TIME) {
            yield return wait;

            timer += Time.deltaTime;
            float progress = timer / CAMERA_MOVE_TIME;

            transform.localPosition = Vector3.Lerp(startLocalPosition, endLocalPosition, progress);
        }
    }
}
