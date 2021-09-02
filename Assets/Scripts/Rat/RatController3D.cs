using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RatController3D : MonoBehaviour
{
    [SerializeField]
    private float landSpeed = 7f;
    [SerializeField]
    private float airControl = 0.75f;
    [SerializeField]
    private float landJumpVelocity = 18f;

    // Variables for jumping
    private bool onGround = false;

    // Reference variables
    private Rigidbody rigidBody;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        // Handles ground Velocity
        Vector3 moveVector = Input.GetAxis("Horizontal") * Vector3.right;
        moveVector += Input.GetAxis("Vertical") * Vector3.forward;
        
        if (moveVector != Vector3.zero) {
            moveVector.Normalize();

            moveVector *= (landSpeed * Time.deltaTime);
            if (!onGround) {
                moveVector *= airControl;
            }
        }

        transform.Translate(moveVector);

        // Handle jump
        float heightVelocity = rigidBody.velocity.y;

        if (onGround && Input.GetButtonDown("Jump")) {
            heightVelocity = landJumpVelocity;
        }

        // Actually move the player
        rigidBody.velocity = Vector3.up * heightVelocity;
    }

    /* Event handler when rat lands on ground */
    public void onLanding() {
        onGround = true;
    }

    /* Event handler method when rat leaves the ground */
    public void onLeavingGround() {
        onGround = false;
    }
}
