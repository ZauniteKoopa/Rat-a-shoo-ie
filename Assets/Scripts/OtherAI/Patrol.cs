using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Patrol : Chef
{
    private int patrolIndex = 0;
    [SerializeField]
    private Transform[] patrolPoints = null;

    protected override IEnumerator passiveAction() {
        Vector3 currentPatrolPoint = patrolPoints[patrolIndex].position;
        yield return goToPosition(currentPatrolPoint);
        yield return new WaitForSeconds(2.0f);
        patrolIndex = (patrolIndex + 1) % patrolPoints.Length;
    }
}
