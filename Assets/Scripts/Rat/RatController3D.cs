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
    private float sprintSpeed = 10f;
    [SerializeField]
    private float airControl = 0.75f;
    [SerializeField]
    private float landJumpVelocity = 18f;
    [SerializeField]
    private float gravityForce = 9.81f;
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
    public LayerMask spotShadowCollisionLayer;
    private bool canMove = true;

    // Respawn management
    [Header("Respawn Management")]
    [SerializeField]
    private SpriteRenderer characterSprite = null;
    [SerializeField]
    private float deathAnimationDuration = 0.75f;
    [SerializeField]
    private float blackFadeDuration = 1.0f;
    [SerializeField]
    private float blackedOutDuration = 0.5f;
    [SerializeField]
    private float spawnInSequenceDuration = 0.3f;
    [SerializeField]
    private float invincibilityDuration = 1.0f;
    [SerializeField]
    private float haloDuration = 3.0f;
    private Component spawnHalo = null;
    private bool facingRight = false;
    private Color invinicibleColor;
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
    [SerializeField]
    private float throwDistance = 1.5f;
    [SerializeField]
    private InteractableSensor interactableSensor = null;
    private GeneralInteractable grabbedInteractable = null;
    private GeneralInteractable targetedInteractable = null;
    private float heavyInteractableDistance = 0.0f;

    [Header("User interface")]
    [SerializeField]
    private ToDoList userInterface = null;

    // Spawn point management
    private Vector3 spawnPosition;

    // Reference variables
    private Rigidbody rigidBody;
    private Collider myCollider;
    private RatAudioManager audioManager = null;

    // Start is called before the first frame update
    void Start()
    {
        invinicibleColor = new Color(1.0f, 1.0f, 1.0f, 0.4f);
        spawnPosition = transform.position;

        spawnHalo = GetComponent("Halo");
        myCollider = GetComponent<Collider>();
        rigidBody = GetComponent<Rigidbody>();
        audioManager = GetComponent<RatAudioManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (canMove) {
            // Handling target interactable, highlight the nearest interactable that's in front of the player.
            if (onGround && grabbedInteractable == null && interactableSensor.isNearInteractable()) {
                GeneralInteractable previousTarget = targetedInteractable;
                targetedInteractable = interactableSensor.getNearestInteractable(groundForward);

                // Remove highlight if player is looking at another interactable
                if (previousTarget != null && previousTarget != targetedInteractable) {
                    previousTarget.removeHighlight();
                }

                targetedInteractable.highlight();

            // If player moved away from target interactable, remove the highlight
            } else if (targetedInteractable != null) {
                targetedInteractable.removeHighlight();
                targetedInteractable = null;
            }
            
            handleGroundMovement();

            // Handling interactable
            if (Input.GetButtonDown("Interact")) {
                handleInteractable();
            }

            // If handling heavy, check for cases where you drop the interactable automatically. Otherwise, handle jump
            if (grabbedInteractable != null && grabbedInteractable.weight == InteractableWeight.HEAVY) {
                if (!onGround || Vector3.Distance(grabbedInteractable.transform.position, transform.position) > 0.1 + heavyInteractableDistance) {
                    dropGrabbedInteractable();
                }
            } else {
                // Handle jump
                float heightVelocity = rigidBody.velocity.y;

                if (onGround && Input.GetButtonDown("Jump")) {
                    heightVelocity = landJumpVelocity;
                }

                rigidBody.velocity = (Vector3.up * heightVelocity);
            }
        }

        // Manage usability things
        manageSpotShadow();
    }

    // Fixed update to apply gravity to the player
    void FixedUpdate() {
        if (!onGround) {
            rigidBody.AddForce(Vector3.down * rigidBody.mass * gravityForce);
        }
        
    }

    /* Main method to handle ground movement */
    private void handleGroundMovement() {
        // Calculate the exact axis values based on blockers
        float horizontalAxis = Input.GetAxis("Horizontal");
        float verticalAxis = Input.GetAxis("Vertical");

        // Only constrain movement if not dragging heavy object
        if (grabbedInteractable == null || grabbedInteractable.weight != InteractableWeight.HEAVY) {
            horizontalAxis = (leftBlockSensor.isBlocked() && horizontalAxis < 0f) ? 0f : horizontalAxis;
            horizontalAxis = (rightBlockSensor.isBlocked() && horizontalAxis > 0f) ? 0f : horizontalAxis;

            verticalAxis = (backwardBlockSensor.isBlocked() && verticalAxis < 0f) ? 0f : verticalAxis;
            verticalAxis = (forwardBlockSensor.isBlocked() && verticalAxis > 0f) ? 0f : verticalAxis;
        }
        
        // Calculate the actual move vector
        Vector3 forwardVector = (Input.GetAxis("Horizontal") * Vector3.right) + (Input.GetAxis("Vertical") * Vector3.forward);
        Vector3 moveVector = horizontalAxis * Vector3.right + verticalAxis * Vector3.forward;

        // TODO make this neater, flips sprite
        if (horizontalAxis > 0 && !facingRight)
            Flip();
        else if (horizontalAxis < 0 && facingRight)
            Flip();

        // Update forward
        if (forwardVector != Vector3.zero) {
            float direction = WorldSprite.getSpriteLookDirection(forwardVector.normalized);
            animator.SetFloat("direction", direction);

            if (direction <= 0.0) {
                groundForward = Vector3.forward;
            } else if (direction >= 1.0) {
                groundForward = Vector3.back;
            } else if (facingRight) {
                groundForward = Vector3.right;
            } else {
                groundForward = Vector3.left;
            }
        }

        moveVector.Normalize();

        // for animation
        animator.SetFloat("movementspeed", moveVector.magnitude);

        // Apply speed to move vector if you're actually moving
        if (moveVector != Vector3.zero) {
            moveVector.Normalize();
            float currentSpeed = (Input.GetButton("Sprint")) ? sprintSpeed : landSpeed;

            moveVector *= (currentSpeed * Time.deltaTime);
            if (!onGround) {
                moveVector *= airControl;
            }
        }

        // Do translation and manage spot shadow
        transform.Translate(moveVector);
    }


    /* Main method to manage spot shadow */
    private void manageSpotShadow() {
        RaycastHit hit; 

        if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity, ~spotShadowCollisionLayer)) {
            spotShadow.position = hit.point;
        }
    }

    /* Method to handle interactable */
    private void handleInteractable() {
        // If rat is not grabbing anything and rat has a target, check if the player wants to grab it
        if (targetedInteractable != false) {
            StartCoroutine(grabInteractable());
        }

        // If the rat is grabbing an interactable, set the interactable's parent to null
        else if (grabbedInteractable != null) {

            //for animation TODO
            animator.SetBool("interacting", false);
            dropGrabbedInteractable();
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

    /* Method for taking damage */
    public void takeDamage() {
        if (!invincible) {
            StopAllCoroutines();
            dropGrabbedInteractable();
            StartCoroutine(respawnRoutine());

            audioManager.emitDamageSound();
            playerHealthLossEvent.Invoke();
        }
    }

    /* Main coroutine to respawn player at the current spawn position */
    private IEnumerator respawnRoutine() {
        // Disable certain components to make this easier
        //rigidBody.constraints = RigidbodyConstraints.FreezeAll;
        canMove = false;
        invincible = true;

        if (targetedInteractable != null) {
            targetedInteractable.removeHighlight();
        }

        // Do death sequence / animation before fading to black
        yield return new WaitForSeconds(deathAnimationDuration);

        // Fade to black
        yield return userInterface.blackOutSequence(blackFadeDuration);

        // when screen is completely black, teleport character to spawn so player doesn't see camera movement. and then blink bAack
        transform.position = spawnPosition;
        characterSprite.color = invinicibleColor;
        yield return new WaitForSeconds(blackedOutDuration);
        userInterface.blinkBack();

        // Make rat glow so that player notices where their character is. In this section, so some spawning animation or particle effects
        spawnHalo.GetType().GetProperty("enabled").SetValue(spawnHalo, true, null);
        yield return new WaitForSeconds(spawnInSequenceDuration);

        // Actually allow movement, but still have the spawn halo / inviciblity around the player
        canMove = true;
        //rigidBody.constraints = RigidbodyConstraints.FreezeRotation;
        yield return new WaitForSeconds(invincibilityDuration);

        invincible = false;
        characterSprite.color = Color.white;

        yield return new WaitForSeconds(haloDuration - invincibilityDuration);

        // Disable halo and invincibility
        spawnHalo.GetType().GetProperty("enabled").SetValue(spawnHalo, false, null);
    }


    private IEnumerator grabInteractable() {
        grabbedInteractable = targetedInteractable;
        grabbedInteractable.removeHighlight();
        grabbedInteractable.onPlayerInteractStart();

        targetedInteractable = null;

        // for animation TODO
        animator.SetBool("interacting", true);

        if (grabbedInteractable.weight == InteractableWeight.LIGHT) {
            audioManager.emitPickupSound();

            // Disable physics and place transform in hook
            grabbedInteractable.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            grabbedInteractable.transform.parent = transform;
            grabbedInteractable.transform.localPosition = grabbableHook;

            // If it's a solution object, make sure chef can't grab it
            SolutionObject grabbedSolution = grabbedInteractable.GetComponent<SolutionObject>();
            if (grabbedSolution != null) {
                grabbedSolution.canChefGrab = false;
            }
        }

        canMove = false;
        yield return new WaitForSeconds(0.25f);
        canMove = true;
    }

    // Private helper method to drop thrown
    private void dropGrabbedInteractable() {
        if (grabbedInteractable != null) {
            audioManager.emitDropSound();

            // If the rat is grabbing a light object, drop light object in front of you and freeze its rotation
            if (grabbedInteractable.weight == InteractableWeight.LIGHT) {
                Vector3 grabbedPosition = transform.TransformPoint(grabbableHook);
                Vector3 maxThrowPosition = (groundForward * throwDistance) + transform.TransformPoint(grabbableHook);

                // Do a raycast check to avoid throwing interactables through walls
                RaycastHit hit; 
                if (Physics.Raycast(transform.position, groundForward, out hit, throwDistance, ~spotShadowCollisionLayer)) {
                    grabbedInteractable.transform.position = hit.point;
                } else {
                    grabbedInteractable.transform.position = maxThrowPosition;
                }

                grabbedInteractable.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
            }

            grabbedInteractable.onPlayerInteractEnd();

            // If it's a solution object, make sure chef can't grab it
            SolutionObject grabbedSolution = grabbedInteractable.GetComponent<SolutionObject>();
            if (grabbedSolution != null) {
                grabbedSolution.canChefGrab = false;
            }

            grabbedInteractable.transform.parent = null;
            grabbedInteractable = null;
        }
    }

    // Main method to set the spawn point 
    public void changeSpawnPoint(Transform spawnPoint) {
        spawnPosition = spawnPoint.position;
    }

    // Main method to flip the sprite
    void Flip()
    {
        facingRight = !facingRight;
        characterSprite.flipX = !characterSprite.flipX;
    }
}
