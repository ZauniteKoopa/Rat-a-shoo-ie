using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

public class IssueObject : MonoBehaviour
{
    // An issue object's priority
    [SerializeField]
    private int issuePriority = 1;
    [SerializeField]
    private List<SolutionType> sequentialSolutionSteps = null;
    public bool isBeingDealtWith = false;
    public UnityEvent issueRemovedEvent;
    
    // Accessor method to get the priority
    public int getPriority() {
        return issuePriority;
    }

    // Method to access the Nth solution in the sequence
    public SolutionType getNthStep(int n) {
        Debug.Assert(n >= 0 && n < sequentialSolutionSteps.Count);
        return sequentialSolutionSteps[n];
    }

    // Method to access the number of steps to do the sequence
    public int getTotalSequenceSteps() {
        return sequentialSolutionSteps.Count;
    }

    // Public method to remove issue from the 
    public void removeIssue() {
        issueRemovedEvent.Invoke();
        Object.Destroy(gameObject);
    }
}
