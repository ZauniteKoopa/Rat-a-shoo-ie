using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[CreateAssetMenu]
public class SolutionTypeSpriteMap : ScriptableObject
{
    [SerializeField]
    private List<SolutionType> solutionTypeKeys = null;

    [SerializeField]
    private List<Sprite> solutionTypeSprites = null;

    // Public method to get sprites from a specified solution type
    public Sprite getSolutionSprite(SolutionType solutionType) {
        return solutionTypeSprites[(int)solutionType];
    }

    // Public method to get the associated dictionary with this map
    //  Pre: length of keys == length of sprite values
    //  Please only use this on the initialization of a level
    public Dictionary<SolutionType, Sprite> getThoughtBubbleDictionary() {
        Debug.Assert(solutionTypeKeys.Count == solutionTypeSprites.Count);

        Dictionary<SolutionType, Sprite> thoughtPool = new Dictionary<SolutionType, Sprite>();

        for (int i = 0; i < solutionTypeKeys.Count; i++) {
            thoughtPool[solutionTypeKeys[i]] = solutionTypeSprites[i];
        }

        return thoughtPool;
    }

}
