using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IssueObject : MonoBehaviour
{
    // An issue object's priority
    [SerializeField]
    private int issuePriority = 1;

    
    // Accessor method to get the priority
    public int getPriority() {
        return issuePriority;
    }
}
