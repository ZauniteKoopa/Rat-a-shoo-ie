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
}
