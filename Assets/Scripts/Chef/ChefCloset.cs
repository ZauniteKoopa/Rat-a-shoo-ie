using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChefCloset : MonoBehaviour
{
    [SerializeField]
    private Vector3 localIngredientSpawnPosition = Vector3.zero;
    private LevelInfo levelInfo = null;

    // On awake, get initial level info
    void Awake() {
        levelInfo = FindObjectOfType<LevelInfo>();
    }

    // Public method to spawn a solution object in the closet
    public void spawnSolutionObject(SolutionType solutionType) {
        Transform solutionPrefab = levelInfo.getSolutionPrefab(solutionType);
        Object.Instantiate(solutionPrefab, transform.TransformPoint(localIngredientSpawnPosition), Quaternion.identity);
    }

    // If a solution object hits this area, it must be a solution object obtained by the chef
    void OnCollisionEnter(Collision collision) {
        Collider collider = collision.collider;
        SolutionObject solution = collider.GetComponent<SolutionObject>();

        if (solution != null) {
            Object.Destroy(collider.gameObject);
        }
    }

}
