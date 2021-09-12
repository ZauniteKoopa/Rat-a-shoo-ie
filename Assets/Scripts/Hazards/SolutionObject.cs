using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SolutionType {
    FIRE_EXTINGUISHER,
    TOWEL,
    BROOM
};

public class SolutionObject : MonoBehaviour
{

    public SolutionType solutionType = SolutionType.FIRE_EXTINGUISHER;

    // in getDuration, make a switch statement that decides the duration of the object based on enum
    //  MUST BE UPDATED WHEN ADDING ANOTHER SOLUTION TYPE TO THE ENUM
    public float getDuration() {
        switch(solutionType) {
            case SolutionType.FIRE_EXTINGUISHER:
                return 1.0f;
            case SolutionType.TOWEL:
                return 1.0f;
            case SolutionType.BROOM:
                return 1.0f;
            default:
                throw new System.Exception("Invalid Solution Type");
        }
    }
}
