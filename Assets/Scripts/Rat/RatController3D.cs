using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RatController3D : MonoBehaviour
{
    // Serialized movement variables
    [SerializeField]
    private float landSpeed = 7f;
    [SerializeField]
    private float airControl = 0.75f;
    [SerializeField]
    private float landJumpVelocity = 18f;

    // Variables for jumping
    private bool onGround = false;

    // Ground forward
    private Vector3 groundForward = Vector3.back;

    // Variables for interactables / grabbables
    [SerializeField]
    private Vector3 grabbableHook = Vector3.up;
    private Transform grabbedInteractable = null;
    private Transform targetInteractable = null;

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
            groundForward = moveVector;
        }
        
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

        rigidBody.velocity = (Vector3.up * heightVelocity);

        // Handling interactable
        handleInteractable();
    }

    /* Method to handle interactable */
    private void handleInteractable() {
        // If rat is not grabbing anything and rat has a target, check if the player wants to grab it
        if (onGround && grabbedInteractable == null && targetInteractable != null) {
            if (Input.GetButtonDown("Fire2")) {
                grabbedInteractable = targetInteractable;

                // Disable physics and place transform in hook
                grabbedInteractable.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                grabbedInteractable.parent = transform;
                grabbedInteractable.localPosition = grabbableHook;
            }
        }
        else if (grabbedInteractable != null) {
            if (Input.GetButtonDown("Fire2")) {
                grabbedInteractable.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                grabbedInteractable.localPosition = groundForward + grabbableHook;
                grabbedInteractable.parent = null;

                grabbedInteractable = null;
            }
        }
    }

    /* Event handler when rat lands on ground */
    public void onLanding() {
        onGround = true;
    }

    /* Event handler method when rat leaves the ground */
    public void onLeavingGround() {
        onGround = false;
    }

    /* Event handler method for targeting interactable */
    public void setTargetedInteractable(Transform interactable) {
        targetInteractable = interactable;
    }

    /* Event handler method for clearing interactable */
    public void clearTargetedInteractable(Transform interactable) {
        if (targetInteractable == interactable) {
            targetInteractable = null;
        }
    }
}
