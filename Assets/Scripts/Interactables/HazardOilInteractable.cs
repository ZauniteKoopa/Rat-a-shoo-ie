using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HazardOilInteractable : HazardInteractable
{
    [SerializeField]
    private float nearFireDuration = 10f;

    // Main method to get the duration
    protected override float getDuration() {
        Collider[] overlappedColliders = Physics.OverlapSphere(curHazard.transform.position, 3.5f);

        for (int i = 0; i < overlappedColliders.Length; i++) {
            Collider curCollider = overlappedColliders[i];
            DestroyableObject destroyObj = curCollider.GetComponent<DestroyableObject>();

            if (destroyObj != null && destroyObj.burnable) {
                return nearFireDuration;
            }
        }

        return HAZARD_EXPIRATION_DURATION;
    }
}
