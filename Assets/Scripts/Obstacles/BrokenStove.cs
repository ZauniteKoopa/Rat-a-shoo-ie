using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrokenStove : MonoBehaviour
{
    [SerializeField]
    private GameObject hitbox = null;
    [SerializeField]
    private float normalTime = 1.5f;
    [SerializeField]
    private float anticipationTime = 0.5f;
    [SerializeField]
    private float attackTime = 0.5f;

    private MeshRenderer meshRender = null;

    // Start is called before the first frame update
    private void Start() {
        meshRender = GetComponent<MeshRenderer>();
        StartCoroutine(stoveCycle());
    }

    // Core Obstacle loop
    private IEnumerator stoveCycle() {
        while (true) {
            hitbox.SetActive(false);
            meshRender.material.color = Color.black;
            yield return new WaitForSeconds(normalTime);

            meshRender.material.color = Color.magenta;
            yield return new WaitForSeconds(anticipationTime);

            meshRender.material.color = Color.red;
            hitbox.SetActive(true);
            yield return new WaitForSeconds(attackTime);
        }
    }
}
