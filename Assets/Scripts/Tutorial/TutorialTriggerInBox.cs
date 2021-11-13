using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TutorialTriggerInBox : MonoBehaviour
{
    public UnityEvent playerEnterEvent;

    private void OnTriggerEnter(Collider collider) {
        RatController3D ratPlayer = collider.GetComponent<RatController3D>();

        if (ratPlayer != null) {
            playerEnterEvent.Invoke();
            Object.Destroy(gameObject);
        }
    }
}
