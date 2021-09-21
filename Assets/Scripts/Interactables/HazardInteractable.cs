using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HazardInteractable : GeneralInteractable
{
    [SerializeField]
    private Transform hazardCaused = null;
    [SerializeField]
    private string initiatedTag = null;
    private bool falling = false;
    private Rigidbody rb;

    public AudioClip impactSound;
    AudioSource audioSource;

    public UnityEvent hazardCreatedEvent;

    // On awake, get rigidbody component
    private void Awake() {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    public override void onPlayerInteractEnd() {
        StartCoroutine(checkFallingObject());
    }

    // Private IEnumerator to check for falling object
    private IEnumerator checkFallingObject() {
        WaitForFixedUpdate waitFrame = new WaitForFixedUpdate();

        // Wait for 2 frames
        yield return waitFrame;
        yield return waitFrame;

        // Set the falling flag to true and keep it true until the rigidbody stops moving
        falling = true;

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
                Object.Instantiate(hazardCaused, transform.position, Quaternion.identity);
                Object.Destroy(gameObject);
            }
            else
            {
                audioSource.PlayOneShot(impactSound, 0.5f);
            }
        }
    }
}
