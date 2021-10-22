using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SwipeDetection : MonoBehaviour
{

    // Swipe variables
    [SerializeField]
    private float minimumSwipeDistance = 0.2f;
    [SerializeField]
    private float maximumSwipeTime = 1.0f;
    [SerializeField, Range(0f, 1f)]
    private float swipeDirectionThreshold = 0.9f;

    // Unity Events on swipe left and swipe right
    public UnityEvent leftSwipeEvent;
    public UnityEvent rightSwipeEvent;

    // Variables to keep track of for swiping
    private Vector2 startPosition;
    private float startTime;
    private Vector2 endPosition;
    private float endTime;
    private Vector2 currentPosition;

    // On awake, get input manager
    private void Awake() {
        if (Application.platform != RuntimePlatform.Android) {
            Object.Destroy(gameObject);
        }
    }

    // Method event handler for starting a swipe
    private void swipeStart(Vector2 position, float time) {
        startPosition = position;
        startTime = time;
    }

    // Method event handler for ending a swipe
    private void swipeEnd(Vector2 position, float time) {
        endPosition = position;
        endTime = time;
        detectSwipe();
    }

    // Main method to check swipe
    private void detectSwipe() {
        if (Vector2.Distance(startPosition, endPosition) >= minimumSwipeDistance && endTime - startTime <= maximumSwipeTime) {
            swipeDirection(endPosition - startPosition);
        }
    }

    // Method to check direction if it's a valid swipe
    private void swipeDirection(Vector2 distanceVector) {
        Vector2 direction = distanceVector.normalized;

        if (Vector2.Dot(Vector2.left, direction) > swipeDirectionThreshold) {
            leftSwipeEvent.Invoke();
        } else if (Vector2.Dot(Vector2.right, direction) > swipeDirectionThreshold) {
            rightSwipeEvent.Invoke();
        }

    }

    // Event handler for Unity Event on swiping behavior
    public void swipeEventHandler(InputAction.CallbackContext context) {
        if (context.started) {
            swipeStart(currentPosition, (float)context.startTime);
        } else if (context.canceled) {
            swipeEnd(currentPosition, (float)context.time);
        }
    }

    // Method for updating position
    public void updateTouchPosition(InputAction.CallbackContext context) {
        currentPosition = context.ReadValue<Vector2>();
    }
}
