using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RestaurantStructure : MonoBehaviour
{
    // Variables concerning pillar management
    private RestaurantPillar[] pillars = null;
    private int pillarsDestroyed = 0;
    private bool activated = false;

    // Events
    public UnityEvent pillarDestroyedEvent;

    // On start, connect all pillars to onPillarDestroyed
    private void Start() {

        pillars = FindObjectsOfType<RestaurantPillar>();

        foreach(RestaurantPillar pillar in pillars) {
            pillar.pillarDestroyedEvent.AddListener(onPillarDestroyed);
        }
    }

    // Accessor methods
    public int getTotalPillars() {
        return pillars.Length;
    }

    public int getPillarsDestroyed() {
        return pillarsDestroyed;
    }

    // Event handler methods
    public void onDestroyTaskActivation() {
        if (!activated) {
            foreach(RestaurantPillar pillar in pillars) {
                pillar.activate();
            }

            activated = true;
        }
    }

    public void onPillarDestroyed() {
        pillarsDestroyed++;
        pillarDestroyedEvent.Invoke();
    }
}
