using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RestaurantPillar : MonoBehaviour
{
    [SerializeField]
    private GameObject protectiveShell = null;
    [SerializeField]
    private GameObject signSprite = null;
    private bool activated = false;
    private bool destroyed = false;
    public UnityEvent pillarDestroyedEvent;
    public UnityEvent pillarActivatedEvent;

    // Method to activate pillar
    public void activate() {
        protectiveShell.GetComponent<MeshRenderer>().enabled = false;
        protectiveShell.GetComponent<Collider>().enabled = false;
        activated = true;
        pillarActivatedEvent.Invoke();
    }

    // Method to check if you can destroy the pillar
    public bool canDestroy() {
        return activated && !destroyed;
    }

    // Method to destroy object
    public void destroy() {
        destroyed = true;
        pillarDestroyedEvent.Invoke();
        GetComponent<MeshRenderer>().enabled = false;
        GetComponent<Collider>().enabled = false;
        signSprite.GetComponent<SpriteRenderer>().enabled = false;
    }

    // Method to respawn pillars if activated
    public void respawn() {
        if (activated) {
            destroyed = false;
            GetComponent<MeshRenderer>().enabled = true;
            GetComponent<Collider>().enabled = true;
            signSprite.GetComponent<SpriteRenderer>().enabled = true;
        }
    }
}
