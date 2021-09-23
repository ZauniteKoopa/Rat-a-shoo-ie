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

    [SerializeField]
    private List<Transform> solutionTypePrefabs = null;

    // Public method to get sprites from a specified solution type
    public Sprite getSolutionSprite(SolutionType solutionType) {
        return solutionTypeSprites[(int)solutionType];
    }

    // Public method to get the associated dictionary with this map between solution type and sprite
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

    // Public method to get the associated dictionary with this map between solution type and prefab
    //  Pre: length of keys == length of prefabs
    //  Please only use this on initialization of level
    public Dictionary<SolutionType, Transform> getSolutionPrefabDictionary() {
        Debug.Assert(solutionTypeKeys.Count == solutionTypePrefabs.Count);

        Dictionary<SolutionType, Transform> closetDictionary = new Dictionary<SolutionType, Transform>();

        for (int i = 0; i < solutionTypeKeys.Count; i++) {
            closetDictionary[solutionTypeKeys[i]] = solutionTypePrefabs[i];
        }

        return closetDictionary;
    }
}
