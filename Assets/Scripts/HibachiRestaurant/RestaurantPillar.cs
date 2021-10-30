using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RestaurantPillar : MonoBehaviour
{
    [SerializeField]
    private GameObject protectiveShell = null;
    private bool activated = false;
    private bool destroyed = false;
    public UnityEvent pillarDestroyedEvent;

    // Method to activate pillar
    public void activate() {
        Object.Destroy(protectiveShell);
        activated = true;
    }

    // Method to check if you can destroy the pillar
    public bool canDestroy() {
        return activated && !destroyed;
    }

    // Method to destroy object
    public void destroy() {
        destroyed = true;
        pillarDestroyedEvent.Invoke();
        Object.Destroy(gameObject);
    }
}
