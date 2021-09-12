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

    // On awake, create unity event
    private void Awake() {
        solutionSensedEvent = new SolutionSensedDelegate();
    }

    // If chef finds a solution object in range, try to grab that solution object
    private void OnTriggerEnter(Collider collider) {
        SolutionObject possibleSolution = collider.GetComponent<SolutionObject>();

        if (possibleSolution != null) {
            Debug.Log("I sensed a solution nearby");
            solutionSensedEvent.Invoke(possibleSolution);
        }
    }
}
