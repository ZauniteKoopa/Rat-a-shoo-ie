using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyableObject : MonoBehaviour
{
    // Properties that make sure object can be destroyed
    public bool burnable = false;
    public bool breakable = false;

    // Public method to destroy the object
    public void destroyObject() {
        Object.Destroy(gameObject);
    }
}
