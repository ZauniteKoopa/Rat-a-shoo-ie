using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EscapeHole : MonoBehaviour
{
    public UnityEvent escapeEvent;

    private void OnTriggerEnter(Collider collider) {
        RatController3D ratPlayer = collider.transform.GetComponent<RatController3D>();

        if (ratPlayer != null) {
            escapeEvent.Invoke();
        }
    }
}
