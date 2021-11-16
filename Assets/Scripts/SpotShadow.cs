using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpotShadow : MonoBehaviour
{
    // Spot shadow collision layer
    public LayerMask spotShadowCollisionLayer;
    [SerializeField]
    private float raycastRadius = 0.1f;
    private Vector3[] CARDINAL_DIRECTIONS;

    private void Start() {
        CARDINAL_DIRECTIONS = new Vector3[5];
        CARDINAL_DIRECTIONS[0] = Vector3.left;
        CARDINAL_DIRECTIONS[1] = Vector3.right;
        CARDINAL_DIRECTIONS[2] = Vector3.forward;
        CARDINAL_DIRECTIONS[3] = Vector3.back;
        CARDINAL_DIRECTIONS[4] = Vector3.zero;
    }


    // Update is called once per frame to manage spot shadow
    void Update()
    {
        // Send out 4 raycasts to fill the dictionary
        RaycastHit hit;
        Dictionary<float, int> heightFrequencyMap = new Dictionary<float, int>();

        // Set up the dictionary
        for (int i = 0; i < CARDINAL_DIRECTIONS.Length; i++) {
            Vector3 rayDir = CARDINAL_DIRECTIONS[i];
            Vector3 raycastStart = transform.parent.position + (rayDir * raycastRadius);

            if (Physics.Raycast(raycastStart, Vector3.down, out hit, Mathf.Infinity, ~spotShadowCollisionLayer)) {
                // Main way to debug spot shadow on player
                // if (transform.parent.GetComponent<RatController3D>() != null) {
                //     Debug.Log(hit.collider);
                // }

                if (heightFrequencyMap.ContainsKey(hit.point.y)) {
                    heightFrequencyMap[hit.point.y]++;
                } else {
                    heightFrequencyMap[hit.point.y] = 1;
                }
            }
        }

        // Now get the best point height: the point with the highest frequency. If a tie, just choose the highest height
        float bestHeight = -1000000.0f;
        int bestFreq = 0;

        foreach(KeyValuePair<float, int> entry in heightFrequencyMap) {
            if (entry.Value > bestFreq) {
                bestHeight = entry.Key;
                bestFreq = entry.Value;
            } else if (entry.Value == bestFreq && entry.Key > bestHeight) {
                bestHeight = entry.Key;
                bestFreq = entry.Value;
            }
        }

        bestHeight += 0.05f;

        // Set the position
        transform.position = new Vector3(transform.parent.position.x, bestHeight, transform.parent.position.z);
    }

    // Main method to enable spot shadow
    public void enable() {
        gameObject.SetActive(true);
    }

    // Main method to disable spot shadow
    public void disable() {
        gameObject.SetActive(false);
    }
}
