using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChefHitbox : MonoBehaviour
{
    // Method to check for player collision
    private void OnTriggerEnter(Collider collider) {
        RatController3D ratPlayer = collider.GetComponent<RatController3D>();
        DestroyableObject destroyableObject = collider.GetComponent<DestroyableObject>();

        if (ratPlayer != null) {
            ratPlayer.takeDamage();
        } else if (destroyableObject != null && destroyableObject.breakable) {
            destroyableObject.destroyObject();
        }
    }
}
