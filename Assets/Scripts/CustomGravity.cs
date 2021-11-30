using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomGravity : MonoBehaviour
{
    [SerializeField]
    private float customGravity = 9.81f;
    [SerializeField]
    private bool gravityEnabled = true;
    private Rigidbody rb;

    // On start, set reference variable to this object's rigidbody
    private void Start() {
        rb = GetComponent<Rigidbody>();
    }

    // On fixed update, apply gravity
    private void FixedUpdate() {
        if (gravityEnabled) {
            rb.AddForce(rb.mass * customGravity * Vector3.down);
        }
    }
}
