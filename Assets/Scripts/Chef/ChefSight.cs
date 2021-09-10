using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class IntelligenceIssueDelegate : UnityEvent<IssueObject> {}

public class ChefSight : MonoBehaviour
{
    // Serialized variables edited in inspector
    [SerializeField]
    private Transform chefEye = null;
    public LayerMask obstructionMask;

    // Variables concerning the player
    private Transform inRangePlayer = null;
    public Transform currentRatTarget = null;

    // Variables concerning IssueObjects that are in range
    private HashSet<IssueObject> inRangeIssues = null;
    public IntelligenceIssueDelegate issueEnterEvent;

    // On awake, initialize inRangeIssue objects
    private void Awake() {
        issueEnterEvent = new IntelligenceIssueDelegate();
        inRangeIssues = new HashSet<IssueObject>();
    }

    // On update, if player is in range, check raycast
    private void Update() {
        if (inRangePlayer != null) {
            Vector3 directionToTarget = (inRangePlayer.position - chefEye.position).normalized;
            float distanceToTarget = Vector3.Distance(inRangePlayer.position, chefEye.position);
            Debug.DrawRay(chefEye.position, directionToTarget * distanceToTarget, Color.red);

            if (!Physics.Raycast(chefEye.position, directionToTarget, distanceToTarget, obstructionMask)) {
                currentRatTarget = inRangePlayer;
            } else {
                currentRatTarget = null;
            }
        }
        // Assumes that the only possible current target is player
        else if (currentRatTarget != null)
        {
            Vector3 directionToTarget = (currentRatTarget.position - chefEye.position).normalized;
            float distanceToTarget = Vector3.Distance(currentRatTarget.position, chefEye.position);

            if (Physics.Raycast(chefEye.position, directionToTarget, distanceToTarget, obstructionMask)) {
                currentRatTarget = null;
            }
        }
    }
    
    // When player enters trigger, say that the rat is in range
    private void OnTriggerEnter(Collider collider) {
        RatController3D ratPlayer = collider.GetComponent<RatController3D>();
        IssueObject issue = collider.GetComponent<IssueObject>();

        // If ratPlayer enters range, keep track of the player
        if (ratPlayer != null) {
            inRangePlayer = ratPlayer.transform;
        }

        // If an issue went in range, record the issue and alert the parent chef AI that an issue has surfaced
        if (issue != null) {
            Debug.Log("Issue entered");
            inRangeIssues.Add(issue);
            issueEnterEvent.Invoke(issue);
        }
    }

    // When player exits trigger, say that the player is out of range
    private void OnTriggerExit(Collider collider) {
        RatController3D ratPlayer = collider.GetComponent<RatController3D>();
        IssueObject issue = collider.GetComponent<IssueObject>();

        // If player exits range, lose track of the player
        if (ratPlayer != null) {
            inRangePlayer = null;
        }

        // If an issue left the range, remove the issue from the hash set. We do not alert the chef AI because the chef can remember an
        // important issue, even when he's not looking at it
        if (issue != null) {
            inRangeIssues.Remove(issue);
        }
    }

    // Public method to get the highest priority issue seen by AI, accessed by the parent AI with this component
    //  Post: returns either the highest priority issue or null if there isn't any
    public IssueObject getHighestPriorityIssue() {
        if (inRangeIssues.Count == 0) {
            return null;
        }

        IssueObject highestIssue = null;

        foreach(IssueObject currentIssue in inRangeIssues) {
            if (highestIssue == null || highestIssue.getPriority() < currentIssue.getPriority()) {
                highestIssue = currentIssue;
            }
        }

        return highestIssue;
    }
}
