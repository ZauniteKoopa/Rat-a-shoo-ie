using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DestroyableObject : MonoBehaviour
{
    // Properties that make sure object can be destroyed
    public bool burnable = false;
    public bool breakable = false;

    // Unity Event on Destroy
    public UnityEvent destroyObjectEvent;

    // Public method to destroy the object
    public void destroyObject() {
        destroyObjectEvent.Invoke();
        Object.Destroy(gameObject);
    }
}
