using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HazardInteractable : ResetInteractable
{
    [SerializeField]
    private Transform hazardCaused = null;
    [SerializeField]
    private string initiatedTag = null;
    private bool falling = false;
    private Rigidbody rb;
    private Collider interactableCollider;

    public AudioClip impactSound;
    AudioSource audioSource;

    public UnityEvent hazardCreatedEvent;

    // Spawn position management
    private Vector3 spawnPosition;
    [SerializeField]
    private float HAZARD_EXPIRATION_DURATION = 8.0f;
    private float SPAWN_OFFSET_DURATION = 1.5f;
    private IssueObject curHazard = null;


    // On awake, get rigidbody component
    private void Awake() {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        spawnPosition = transform.position;
        interactableCollider = GetComponent<Collider>();
    }

    public override void onPlayerInteractEnd() {
        StartCoroutine(checkFallingObject());
    }

    // Private IEnumerator to check for falling object
    private IEnumerator checkFallingObject() {
        WaitForFixedUpdate waitFrame = new WaitForFixedUpdate();

        falling = true;

        // Wait for 2 frames
        yield return waitFrame;
        yield return waitFrame;

        // Set the falling flag to true and keep it true until the rigidbody stops moving
        while (rb.velocity.magnitude > 0.001f) {
            yield return waitFrame;
        }

        falling = false;
    }

    // Private onCollisionEnter: if this interactable hits a collider with initiated tag when falling, create hazard object at that location
    private void OnCollisionEnter(Collision collision) {
        if (falling) {
            Collider collider = collision.collider;

            if (collider.tag == initiatedTag) {
                hazardCreatedEvent.Invoke();
                Transform hazardTransform = Object.Instantiate(hazardCaused, transform.position, Quaternion.identity);
                curHazard = hazardTransform.GetComponent<IssueObject>();
                //hazardTransform.up = collision.GetContact(0).normal;
                falling = false;

                // "destroy" object by sending object to purgatory, locked position
                transform.position = 1000f * Vector3.down;
                rb.constraints = RigidbodyConstraints.FreezeAll;
                interactableCollider.enabled = false;
                canBePickedUp = false;

                StartCoroutine(timedHazardExpiration());
                curHazard.issueRemovedEvent.AddListener(onHazardRemove);

            }
            else
            {
                audioSource.PlayOneShot(impactSound, 0.5f);
            }
        }
    }

    // Public event handler method to handle the event that chef has dealt with the hazard associated with this interactable
    //  ONLY RUNS IF THE HAZARDINTERACTABLE IS NOT A TIMED RESPAWN OBJECT (only respawns once the object is destroyed by the chef)
    public void onHazardRemove() {
        curHazard.issueRemovedEvent.RemoveListener(onHazardRemove);

        // Move object back to spawn position
        StartCoroutine(timedRespawn());
    }

    // Private IEnumerator to do a timed expiration for hazard
    private IEnumerator timedHazardExpiration() {
        yield return new WaitForSeconds(HAZARD_EXPIRATION_DURATION);

        if (curHazard != null && !curHazard.isBeingDealtWith) {
            Object.Destroy(curHazard.gameObject);
            yield return timedRespawn();
        }
    }


    // Private IEnumerator to do a timed respawn
    private IEnumerator timedRespawn() {

        yield return new WaitForSeconds(SPAWN_OFFSET_DURATION);

        // Move object back to spawn position
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        canBePickedUp = true;
        interactableCollider.enabled = true;
        transform.position = spawnPosition;
        curHazard = null;
    }
}
