using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldSprite : MonoBehaviour
{

    // Update is called once per frame: make sure the sprite is always facing the camera
    void Update()
    {
        transform.forward = (transform.position - Camera.main.transform.position).normalized;
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, 0f, transform.eulerAngles.z);
    }

    // Static method to decide what type of sprite to present based on what the unit is facing
    //  Returns an int representing what the sprite should look at
    //      0: back
    //      1: forward
    //      2: side
    public static int getSpriteLookDirection(Vector3 spriteForward) {
        // Get the 2 divider vector, should make an X with the 4 empty spaces representing quadrants
        //  the 2 divider vectors will point back (+Z direction)
        Vector3 dividerVector1 = new Vector3(1f, 0f, 1f);
        Vector3 dividerVector2 = new Vector3(-1f, 0f, 1f);

        float dotProduct1 = Vector3.Dot(spriteForward, dividerVector1);
        float dotProduct2 = Vector3.Dot(spriteForward, dividerVector2);

        // If the forward is pointing to the back quadrant
        if (dotProduct1 > 0.0f && dotProduct2 > 0.0f) {
            return 0;
        }
        // If the forward is pointing to the forward quadrant
        else if(dotProduct1 <= 0.0f && dotProduct2 <= 0.0f)
        {
            return 1;
        }
        // If its in any other quadrant, use a side
        else
        {
            // If dotProduct1 > 0.0f && dotProduct2 <= 0.0f, you are facing right
            // If dotProduct1 <= 0.0f && dotProduct2 > 0.0f, you are facing left
            return 2;
        }

    }

    //  0: back
    //  1: forward
    public static int getSpriteLookDirectionTest(Vector3 spriteForward) {
        Vector3 dividerVector = Vector3.forward;
        float dotProduct = Vector3.Dot(dividerVector, spriteForward);

        if (dotProduct >= 0.0f) {
            return 0;
        } else {
            return 1;
        }
    }
}
