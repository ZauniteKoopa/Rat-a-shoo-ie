using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RatController : MonoBehaviour
{
    // Editable properties
    [SerializeField]
    private float landSpeed = 7f;
    [SerializeField]
    private float airControl = 0.75f;
    [SerializeField]
    private float landJumpVelocity = 18f;
    [SerializeField]
    private float airJumpVelocity = 10f;
    [SerializeField]
    private int maxJumps = 2;

    // Flags for jumping
    private int jumpsLeft;
    private bool onGround = false;

    // Reference variables
    private Rigidbody2D rigidBody;


    /* Start is called before the first frame update */
    void Start()
    {
        jumpsLeft = maxJumps;
        rigidBody = GetComponent<Rigidbody2D>();
    }

    /* Update is called every frame */
    void Update()
    {
        // Handles horizontal Velocity
        Vector3 horizontalVelocity = Input.GetAxis("Horizontal") * Vector3.right;
        horizontalVelocity *= (landSpeed * Time.deltaTime);
        if (!onGround) {
            horizontalVelocity *= airControl;
        }

        transform.Translate(horizontalVelocity);

        // Handles jump
        if (jumpsLeft > 0 && Input.GetButtonDown("Jump")) {
            float curJumpVelocity = (onGround) ? landJumpVelocity : airJumpVelocity;
            rigidBody.velocity = Vector3.up * curJumpVelocity;
            jumpsLeft--;
        }
    }

    /* Event handler when rat lands on ground */
    public void onLanding() {
        onGround = true;
        jumpsLeft = maxJumps;
    }

    /* Event handler method when rat leaves the ground */
    public void onLeavingGround() {
        onGround = false;
        jumpsLeft = maxJumps - 1;
    }
}
