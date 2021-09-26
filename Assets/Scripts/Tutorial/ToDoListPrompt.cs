using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToDoListPrompt : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire3")) {
            Object.Destroy(gameObject);
        } 
    }
}
