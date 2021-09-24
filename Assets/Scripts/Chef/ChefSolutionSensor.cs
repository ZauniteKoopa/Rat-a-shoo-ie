using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Main class for the chef to sense solution objects when dealing with an issue object

[System.Serializable]
public class SolutionSensedDelegate : UnityEvent<SolutionObject> {}

public class ChefSolutionSensor : MonoBehaviour
{

    public SolutionSensedDelegate solutionSensedEvent;
    private HashSet<SolutionObject> inRangeSolutions;

    // On awake, create unity event
    private void Awake() {
        solutionSensedEvent = new SolutionSensedDelegate();
        inRangeSolutions = new HashSet<SolutionObject>();
    }

    // If chef finds a solution object in range, try to grab that solution object
    private void OnTriggerEnter(Collider collider) {
        SolutionObject possibleSolution = collider.GetComponent<SolutionObject>();

        if (possibleSolution != null) {
            inRangeSolutions.Add(possibleSolution);

            if (possibleSolution.canChefGrab)
                solutionSensedEvent.Invoke(possibleSolution);
        }
    }

    // If a solution object has left the trigger box,take it out of the inRangeSolutions
    private void OnTriggerExit(Collider collider) {
        SolutionObject possibleSolution = collider.GetComponent<SolutionObject>();

        if (possibleSolution != null) {
            inRangeSolutions.Remove(possibleSolution);
        }
    }

    // Public method to check if there's a solution in range with the specified solution type
    //  If no solution object of solutionType exists, return null
    public SolutionObject getSolutionInRange(SolutionType solutionType) {
        foreach(SolutionObject solution in inRangeSolutions) {
            if (solution.solutionType == solutionType) {
                return solution;
            }
        }

        return null;
    }
}
