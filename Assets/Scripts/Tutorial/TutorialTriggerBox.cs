using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TutorialTriggerBox : MonoBehaviour
{
    public UnityEvent playerExitEvent;

    private void OnTriggerExit(Collider collider) {
        RatController3D ratPlayer = collider.GetComponent<RatController3D>();

        if (ratPlayer != null) {
            playerExitEvent.Invoke();
            Object.Destroy(gameObject);
        }
    }
}
