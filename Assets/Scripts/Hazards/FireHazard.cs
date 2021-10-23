using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireHazard : MonoBehaviour
{
    // If a burnable destroyable object comes in, destroy it
    void OnTriggerEnter(Collider collider) {
        DestroyableObject targetObject = collider.GetComponent<DestroyableObject>();

        if (targetObject != null && targetObject.burnable) {
            targetObject.destroyObject();
        }
    }

    // Method for OnTriggerStay for the player, if player stays in fire, damage the player
    // void OnTriggerStay(Collider collider) {
    //     RatController3D ratPlayer = collider.GetComponent<RatController3D>();

    //     if (ratPlayer != null) {
    //         ratPlayer.takeDamage();
    //     }
    // }
}
