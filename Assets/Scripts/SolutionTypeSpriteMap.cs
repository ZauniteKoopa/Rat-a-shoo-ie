using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[CreateAssetMenu]
public class SolutionTypeSpriteMap : ScriptableObject
{
    [SerializeField]
    private List<Sprite> solutionTypeSprites = null;

    // Public method to get sprites from a specified solution type
    public Sprite getSolutionSprite(SolutionType solutionType) {
        return solutionTypeSprites[0];
    }
}
