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
    private float carryingLightSpeed = 3f;
    [SerializeField]
    private float carryingHeavySpeed = 1.5f;
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
    private SpriteRenderer characterSprite = null;
    private Color invinicibleColor;
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
    [SerializeField]
    private InteractableSensor interactableSensor = null;
    private GeneralInteractable grabbedInteractable = null;
    private GeneralInteractable targetedInteractable = null;
    private float heavyInteractableDistance = 0.0f;

    [Header("User interface")]
    [SerializeField]
    private ToDoList userInterface = null;

    // Reference variables
    private Rigidbody rigidBody;
    private RatAudioManager audioManager = null;

    // Start is called before the first frame update
    void Start()
    {
        invinicibleColor = new Color(1.0f, 1.0f, 1.0f, 0.5f);
        userInterface.updateHealthUI(maxHealth);
        curHealth = maxHealth;

        rigidBody = GetComponent<Rigidbody>();
        audioManager = GetComponent<RatAudioManager>();
    }

    // Update is called once per frame
    void Update()
    {
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
        if (Input.GetButtonDown("Fire2")) {
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
        
        // Update forward
        if (forwardVector != Vector3.zero) {
            groundForward = forwardVector;
            // set animator controller for sprite look direction
            int direction = WorldSprite.getSpriteLookDirectionTest(groundForward);
            animator.SetFloat("direction", direction);
        }

        moveVector.Normalize();

        // for animation
        animator.SetFloat("movementspeed", moveVector.magnitude);

        // Apply speed to move vector if you're actually moving
        if (moveVector != Vector3.zero) {
            moveVector.Normalize();
            float currentSpeed = landSpeed;

            // If currently grabbing something, change the speed accordingly
            if (grabbedInteractable != null) {
                currentSpeed = (grabbedInteractable.weight == InteractableWeight.LIGHT) ? carryingLightSpeed : carryingHeavySpeed;
            }

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
        if (targetedInteractable != false) {
            grabbedInteractable = targetedInteractable;
            grabbedInteractable.removeHighlight();
            grabbedInteractable.onPlayerInteractStart();

            targetedInteractable = null;

            // for animation TODO
            //animator.SetBool("interacting", true);

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
            else if (grabbedInteractable.weight == InteractableWeight.HEAVY) {
                transform.position = grabbedInteractable.getNearestHeavyItemHook(transform);
                grabbedInteractable.transform.parent = transform;
                grabbedInteractable.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                heavyInteractableDistance = Vector3.Distance(transform.position, grabbedInteractable.transform.position);
            }

        }

        // If the rat is grabbing an interactable, set the interactable's parent to null
        else if (grabbedInteractable != null) {

            //for animation TODO
            //animator.SetBool("interacting", true);

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

        bool isTransparent = false;
        float timer = 0.0f;
        WaitForEndOfFrame waitFrame = new WaitForEndOfFrame();

        while (timer < invincibilityDuration) {
            yield return waitFrame;
            characterSprite.color = (isTransparent) ? Color.white : invinicibleColor;
            isTransparent = !isTransparent;
            timer += Time.deltaTime;
        }

        yield return new WaitForSeconds(invincibilityDuration);

        characterSprite.color = Color.white;
        invincible = false;
    }

    // Private helper method to drop thrown
    private void dropGrabbedInteractable() {
        if (grabbedInteractable != null) {
            audioManager.emitDropSound();

            // If the rat is grabbing a light object, drop light object in front of you and freeze its rotation
            if (grabbedInteractable.weight == InteractableWeight.LIGHT) {
                grabbedInteractable.transform.localPosition = groundForward + grabbableHook;
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
}
