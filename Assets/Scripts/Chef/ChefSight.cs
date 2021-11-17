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
    private Transform prevRatTarget = null;
    public UnityEvent ratPlayerFoundEvent;

    // Variables concerning IssueObjects that are in range
    private HashSet<IssueObject> inRangeIssues = null;
    private HashSet<IssueObject> unseenIssues = null;

    public IntelligenceIssueDelegate issueEnterEvent;

    // On awake, initialize inRangeIssue objects
    private void Awake() {
        issueEnterEvent = new IntelligenceIssueDelegate();
        inRangeIssues = new HashSet<IssueObject>();
        unseenIssues = new HashSet<IssueObject>();
    }

    // On update, if player is in range, check raycast
    private void Update() {
        // Main method to check player
        if (inRangePlayer != null) {
            Vector3 directionToTarget = (inRangePlayer.position - chefEye.position).normalized;
            float distanceToTarget = Vector3.Distance(inRangePlayer.position, chefEye.position);
            Debug.DrawRay(chefEye.position, directionToTarget * distanceToTarget, Color.red);

            if (!Physics.Raycast(chefEye.position, directionToTarget, distanceToTarget, obstructionMask)) {
                prevRatTarget = currentRatTarget;
                currentRatTarget = inRangePlayer;

                if (prevRatTarget == null) {
                    ratPlayerFoundEvent.Invoke();
                }
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

        // Method to check unseen issues
        checkUnseenIssues();
    }

    // Private helper method to checj unseen issues
    private void checkUnseenIssues() {
        if (unseenIssues.Count > 0) {
            List<IssueObject> issuesToRemove = new List<IssueObject>();

            foreach(IssueObject unseenIssue in unseenIssues) {
                // Do a basic check to see if its already being dealt with or been deleted
                if (unseenIssue == null || unseenIssue.isBeingDealtWith) {
                    issuesToRemove.Add(unseenIssue);

                // If you can actually see the issue, put it in inRangeIssues and trigger event
                } else if (canSeeIssue(unseenIssue)) {
                    issuesToRemove.Add(unseenIssue);

                    inRangeIssues.Add(unseenIssue);
                    issueEnterEvent.Invoke(unseenIssue);
                }
            }

            // Remove issues
            foreach(IssueObject removedIssue in issuesToRemove) {
                unseenIssues.Remove(removedIssue);
            }
        }
    }

    // Main method to check if you can see the issue
    private bool canSeeIssue(IssueObject targetIssue) {
        Vector3 directionToTarget = (targetIssue.transform.position - chefEye.position).normalized;
        float distanceToTarget = Vector3.Distance(targetIssue.transform.position, chefEye.position);

        if (Physics.Raycast(chefEye.position, directionToTarget, distanceToTarget, obstructionMask)) {
            return false;
        } else {
            return true;
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
        if (issue != null && !issue.isBeingDealtWith) {

            if (canSeeIssue(issue)) {
                inRangeIssues.Add(issue);
                issueEnterEvent.Invoke(issue);
            } else {
                unseenIssues.Add(issue);
            }
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
            unseenIssues.Remove(issue);
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
