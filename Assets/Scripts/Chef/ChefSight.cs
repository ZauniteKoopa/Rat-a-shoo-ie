using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ChefSight : MonoBehaviour
{
    // Variables to keep track
    [SerializeField]
    private Transform chefEye = null;
    public LayerMask obstructionMask;

    private Transform inRangePlayer = null;
    public Transform currentTarget = null;

    // On update, if player is in range, check raycast
    private void Update() {
        if (inRangePlayer != null) {
            Vector3 directionToTarget = (inRangePlayer.position - chefEye.position).normalized;
            float distanceToTarget = Vector3.Distance(inRangePlayer.position, chefEye.position);

            if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask)) {
                currentTarget = inRangePlayer;
            } else {
                currentTarget = null;
            }
        }
        // Assumes that the only possible current target is player
        else if (inRangePlayer == null && currentTarget != null)
        {
            Vector3 directionToTarget = (currentTarget.position - chefEye.position).normalized;
            float distanceToTarget = Vector3.Distance(currentTarget.position, chefEye.position);

            if (Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask)) {
                currentTarget = null;
            }
        }
    }
    
    // When player enters trigger, say that the rat is in range
    private void OnTriggerEnter(Collider collider) {
        RatController3D ratPlayer = collider.GetComponent<RatController3D>();

        if (ratPlayer != null) {
            inRangePlayer = ratPlayer.transform;
        }
    }

    // When player exits trigger, say that the player is out of range
    private void OnTriggerExit(Collider collider) {
        RatController3D ratPlayer = collider.GetComponent<RatController3D>();

        if (ratPlayer != null) {
            inRangePlayer = null;
        }
    }
}
