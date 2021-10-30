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
    public UnityEvent structureResetEvent;

    // On start, connect all pillars to onPillarDestroyed
    private void Start() {

        pillars = FindObjectsOfType<RestaurantPillar>();
        HibachiToDoList list = FindObjectOfType<HibachiToDoList>();
        list.playerFinishAllTasksEvent.AddListener(onDestroyTaskActivation);

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

    // Main method to reset all structures
    public void resetAllStructures() {
        if (activated) {
            pillarsDestroyed = 0;

            foreach(RestaurantPillar pillar in pillars) {
                pillar.respawn();
            }

            structureResetEvent.Invoke();
        }
    }

    // Event handler methods
    public void onDestroyTaskActivation() {
        if (!activated) {
            foreach(RestaurantPillar pillar in pillars) {
                pillar.activate();
            }

            activated = true;
            structureResetEvent.Invoke();
        }
    }

    public void onPillarDestroyed() {
        pillarsDestroyed++;
        pillarDestroyedEvent.Invoke();
    }
}
