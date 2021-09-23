using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ingredient : MonoBehaviour
{
    // Public variables to access properties
    public bool isSpicy = false;
    public bool isRotten = false;

    // Flag for case where ingredient is being held by the chef
    public bool isHeldByChef = false;
}
