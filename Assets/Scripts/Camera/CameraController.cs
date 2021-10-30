using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private float CAMERA_MOVE_TIME = 0.4f;
    private Vector3 trueCameraPosition;
    private bool isShaking = false;

    public void Awake() {
        trueCameraPosition = transform.localPosition;
    }

    // Main method to move the camera
    public void moveCamera(RoomView roomView) {
        StopAllCoroutines();
        StartCoroutine(moveCameraSequence(roomView));
    }

    // private IEnumerator to move the camera
    private IEnumerator moveCameraSequence(RoomView roomView) {
        transform.parent = roomView.transform;

        // get positions
        Vector3 startLocalPosition = transform.localPosition;
        Vector3 endLocalPosition = roomView.localCameraPosition;

        // get rotation
        Vector3 startLocalRotation = transform.localEulerAngles;
        Vector3 endLocalRotation = roomView.localCameraRotation;

        float timer = 0f;
        WaitForEndOfFrame wait = new WaitForEndOfFrame();

        while (timer < CAMERA_MOVE_TIME) {
            yield return wait;

            timer += Time.deltaTime;
            float progress = timer / CAMERA_MOVE_TIME;

            trueCameraPosition = Vector3.Lerp(startLocalPosition, endLocalPosition, progress);

            // If shaking, don't update localposition because it would be a race condition with shakeCameraSequence
            if (!isShaking)
                transform.localPosition = Vector3.Lerp(startLocalPosition, endLocalPosition, progress);
            
            transform.localEulerAngles = Vector3.Lerp(startLocalRotation, endLocalRotation, progress);
        }
    }

    // Main method to shake the camera
    public IEnumerator shakeCameraSequence(float duration, float magnitude) {
        isShaking = true;
        float timer = 0.0f;

        while (timer < duration) {
            yield return null;
            float x = trueCameraPosition.x + (Random.Range(-1f, 1f) * magnitude);
            float y = trueCameraPosition.y + (Random.Range(-1f, 1f) * magnitude);

            transform.localPosition = new Vector3(x, y, trueCameraPosition.z);
            timer += Time.deltaTime;
        }

        transform.localPosition = trueCameraPosition;
        isShaking = false;
    }
}
