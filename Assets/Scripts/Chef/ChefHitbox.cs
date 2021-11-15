using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChefHitbox : MonoBehaviour
{
    [SerializeField]
    private bool canBreak = true;
    [SerializeField]
    private bool canBurn = false;

    // Method to check for player collision
    private void OnTriggerEnter(Collider collider) {
        RatController3D ratPlayer = collider.GetComponent<RatController3D>();
        DestroyableObject destroyableObject = collider.GetComponent<DestroyableObject>();
        RestaurantPillar pillar = collider.GetComponent<RestaurantPillar>();

        if (ratPlayer != null) {
            ratPlayer.takeDamage();
        } else if (destroyableObject != null) {
            bool willBreak = canBreak && destroyableObject.breakable;
            bool willBurn = canBurn && destroyableObject.burnable;

            if (willBreak || willBurn) {
                destroyableObject.destroyObject();
            }
        } else if (pillar != null && pillar.canDestroy()) {
            pillar.destroy();
        }
    }
}
