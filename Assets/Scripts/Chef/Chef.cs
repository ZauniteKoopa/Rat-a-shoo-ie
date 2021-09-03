using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions;

public class Chef : MonoBehaviour
{
    // Variables for navigation
    private NavMeshAgent navMeshAgent;
    private int waypointIndex = 0;
    [SerializeField]
    private List<Transform> waypoints = null;
    [SerializeField]
    private float navDistance = 1f;


    // Start is called before the first frame update
    void Start()
    {
        Assert.IsTrue(waypoints != null && waypoints.Count > 0);
        navMeshAgent = GetComponent<NavMeshAgent>();

        StartCoroutine(mainIntelligenceLoop());
    }

    // Main intelligence loop
    private IEnumerator mainIntelligenceLoop() {
        while (true) {
            yield return moveToNextWaypoint();
            yield return doPassiveAction();
        }
    }

    // Main IEnumerator to move to waypoint
    private IEnumerator moveToNextWaypoint() {
        // Set up and update variables
        waypointIndex = (waypointIndex + 1) % waypoints.Count;
        Vector3 targetPosition = waypoints[waypointIndex].position;
        WaitForEndOfFrame waitFrame = new WaitForEndOfFrame();

        navMeshAgent.destination = targetPosition;

        // Modify vector so that its flatten in the x, z plane
        Vector3 flatTargetPosition = new Vector3(targetPosition.x, 0, targetPosition.z);
        Vector3 flatCurrentPosition = new Vector3(transform.position.x, 0, transform.position.z);

        // Main wait loop until agent gets to destination point
        while (Vector3.Distance(flatTargetPosition, flatCurrentPosition) > navDistance) {
            yield return waitFrame;
            flatCurrentPosition = new Vector3(transform.position.x, 0, transform.position.z);
        }
    }

    // Main IEnumerator to do passive action
    private IEnumerator doPassiveAction() {
        Debug.Log("doing chef things");
        yield return new WaitForSeconds(3f);
        Debug.Log("chef things are over");
    }


}
