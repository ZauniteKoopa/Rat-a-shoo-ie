using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RatController3D : MonoBehaviour
{

    [Header("Animation")]
    [SerializeField]
    private Animator animator = null;

    // Serialized movement variables
    [Header("Movement variables")]
    [SerializeField]
    private float landSpeed = 7f;
    [SerializeField]
    private float carryingSpeed = 3f;
    [SerializeField]
    private float airControl = 0.75f;
    [SerializeField]
    private float landJumpVelocity = 18f;
    [SerializeField]
    private Transform spotShadow = null;
    [SerializeField]
    private BlockerSensor leftBlockSensor = null;
    [SerializeField]
    private BlockerSensor rightBlockSensor = null;
    [SerializeField]
    private BlockerSensor forwardBlockSensor = null;
    [SerializeField]
    private BlockerSensor backwardBlockSensor = null;
    public LayerMask spotShadowIgnoreLayer;

    // Health management
    [Header("Health Management")]
    [SerializeField]
    private int maxHealth = 3;
    [SerializeField]
    private float invincibilityDuration = 1.5f;
    [SerializeField]
    private Color invinicibleColor = Color.black;
    private Color normalColor;
    private int curHealth;
    private bool invincible = false;
    public UnityEvent playerHealthLossEvent;

    // Variables for jumping
    private bool onGround = false;

    // Ground forward
    private Vector3 groundForward = Vector3.back;

    // Variables for interactables / grabbables
    [Header("Interactables")]
    [SerializeField]
    private Vector3 grabbableHook = Vector3.up;
    private Transform grabbedInteractable = null;
    private Transform targetInteractable = null;

    [Header("User interface")]
    [SerializeField]
    private ToDoList userInterface = null;

    // Reference variables
    private Rigidbody rigidBody;
    private MeshRenderer meshRenderer;
    private RatAudioManager audioManager = null;

    // Start is called before the first frame update
    void Start()
    {
        userInterface.updateHealthUI(maxHealth);
        curHealth = maxHealth;

        rigidBody = GetComponent<Rigidbody>();
        meshRenderer = GetComponent<MeshRenderer>();
        audioManager = GetComponent<RatAudioManager>();

        normalColor = meshRenderer.material.color;
    }

    // Update is called once per frame
    void Update()
    {
        
        handleGroundMovement();

        // Handle jump
        float heightVelocity = rigidBody.velocity.y;

        if (onGround && Input.GetButtonDown("Jump")) {
            heightVelocity = landJumpVelocity;
        }

        rigidBody.velocity = (Vector3.up * heightVelocity);

        // Handling interactable
        handleInteractable();
    }

    /* Main method to handle ground movement */
    private void handleGroundMovement() {
        // Calculate the exact axis values based on blockers
        float horizontalAxis = Input.GetAxis("Horizontal");
        horizontalAxis = (leftBlockSensor.isBlocked() && horizontalAxis < 0f) ? 0f : horizontalAxis;
        horizontalAxis = (rightBlockSensor.isBlocked() && horizontalAxis > 0f) ? 0f : horizontalAxis;

        float verticalAxis = Input.GetAxis("Vertical");
        verticalAxis = (backwardBlockSensor.isBlocked() && verticalAxis < 0f) ? 0f : verticalAxis;
        verticalAxis = (forwardBlockSensor.isBlocked() && verticalAxis > 0f) ? 0f : verticalAxis;

        // Calculate the actual move vector
        Vector3 moveVector = horizontalAxis * Vector3.right;
        moveVector += verticalAxis * Vector3.forward;
        if (moveVector != Vector3.zero) {
            groundForward = moveVector;
        }

        moveVector.Normalize();

        // for animation
        animator.SetFloat("movementspeed", moveVector.magnitude);

        // Add conditional effects to move vector
        if (moveVector != Vector3.zero) {
            moveVector.Normalize();
            float currentSpeed = (grabbedInteractable != null) ? carryingSpeed : landSpeed;

            moveVector *= (currentSpeed * Time.deltaTime);
            if (!onGround) {
                moveVector *= airControl;
            }
        }

        // Do translation and manage spot shadow
        transform.Translate(moveVector);
        manageSpotShadow();
    }


    /* Main method to manage spot shadow */
    private void manageSpotShadow() {
        RaycastHit hit; 

        if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity, ~spotShadowIgnoreLayer)) {
            spotShadow.position = hit.point;
        }
    }

    /* Method to handle interactable */
    private void handleInteractable() {
        // If rat is not grabbing anything and rat has a target, check if the player wants to grab it
        if (onGround && grabbedInteractable == null && targetInteractable != null) {
            if (Input.GetButtonDown("Fire2")) {
                grabbedInteractable = targetInteractable;

                // for animation TODO
                //animator.SetBool("interacting", true);

                audioManager.emitPickupSound();
                // Disable physics and place transform in hook
                grabbedInteractable.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                grabbedInteractable.parent = transform;
                grabbedInteractable.localPosition = grabbableHook;
            }
        }
        else if (grabbedInteractable != null) {
            if (Input.GetButtonDown("Fire2")) {

                //for animation TODO
                //animator.SetBool("interacting", true);

                audioManager.emitDropSound();
                grabbedInteractable.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
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

    /* Method for taking damage */
    public void takeDamage() {
        if (!invincible) {
            curHealth--;
            audioManager.emitDamageSound();
            playerHealthLossEvent.Invoke();

            if (curHealth <= 0) {
                Debug.Log("You died!");
            } else {
                StartCoroutine(invincibilityRoutine());
            }
        }
    }

    /* Main coroutine to do invincibility */
    private IEnumerator invincibilityRoutine() {
        invincible = true;
        meshRenderer.material.color = invinicibleColor;

        yield return new WaitForSeconds(invincibilityDuration);

        meshRenderer.material.color = normalColor;
        invincible = false;
    }
}
