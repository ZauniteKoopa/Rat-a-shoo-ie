using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelInfo : MonoBehaviour
{
    private Dictionary<SolutionType, List<Vector3>> solutionLocations;

    // Run before the first frame
    private void Start() {
        solutionLocations = new Dictionary<SolutionType, List<Vector3>>();
        SolutionObject[] allSolutions = Object.FindObjectsOfType<SolutionObject>();

        // For each solution
        foreach(SolutionObject solution in allSolutions) {
            SolutionType curType = solution.solutionType;
            Vector3 initialLocation = solution.transform.position;

            // If not found in dictionary yet, make a new list
            if (!solutionLocations.ContainsKey(curType)) {
                List<Vector3> newPositions = new List<Vector3>();
                solutionLocations[curType] = newPositions; 
            }

            // Add initial position to the list
            solutionLocations[curType].Add(initialLocation);
        }
    }

    // Method to get the positions of all possible solution of solutionType
    public List<Vector3> getPositions(SolutionType solutionType) {
        return solutionLocations[solutionType];
    }
}
