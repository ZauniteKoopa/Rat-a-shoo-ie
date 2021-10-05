using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChefHitboxRotator : MonoBehaviour
{
    // Public method to rotate based off of chef's sprite "forward". Default rotation 0 is to the back
    //  if rotationNum == 0, face back of the room
    //  if rotationNum == 1, face to the camera
    //  else face left or right (would be 0.5). (depends on facing right boolean)
    public void rotateHitbox(float chefRotationNum, bool facingRight) {
        // If not a side animation, either 0 degrees, 90 degrees, or 180 degrees
        float rotationY = chefRotationNum * 180f;

        // If facing the left, add 180 to make sure hitbox faces the left
        if (chefRotationNum == 0.5f && !facingRight) {
            rotationY += 180f;
        }

        transform.eulerAngles = rotationY * Vector3.up;
    }
}
