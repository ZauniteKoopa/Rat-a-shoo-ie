using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChefHitbox : MonoBehaviour
{
    // Method to check for player collision
    private void OnTriggerEnter(Collider collider) {
        RatController3D ratPlayer = collider.GetComponent<RatController3D>();

        if (ratPlayer != null) {
            ratPlayer.takeDamage();
        }
    }
}
