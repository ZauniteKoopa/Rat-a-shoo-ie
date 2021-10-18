using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DestroyableObject : MonoBehaviour
{
    // Properties that make sure object can be destroyed
    public bool burnable = false;
    public bool breakable = false;
    [SerializeField]
    private AudioClip destroyClip = null;

    // Reference variables
    private AudioSource speaker = null;
    private Collider myCollider = null;
    private MeshRenderer meshRenderer = null;

    // Unity Event on Destroy
    public UnityEvent destroyObjectEvent;


    // On start, set up reference variables
    private void Start() {
        myCollider = GetComponent<Collider>();
        speaker = GetComponent<AudioSource>();
        meshRenderer = GetComponent<MeshRenderer>();
        speaker.clip = destroyClip;
    }


    // Public method to destroy the object
    public void destroyObject() {
        destroyObjectEvent.Invoke();
        StartCoroutine(destroySequence());
    }

    // Main sequence variables to play sound when object is disabled and then actually destroy object
    private IEnumerator destroySequence() {
        myCollider.enabled = false;
        meshRenderer.enabled = false;
        speaker.Play();

        // Disable all children
        for (int i = 0; i < transform.childCount; i++) {
            transform.GetChild(i).gameObject.SetActive(false);
        }

        yield return new WaitForSeconds(destroyClip.length);

        Object.Destroy(gameObject);
    }
}
