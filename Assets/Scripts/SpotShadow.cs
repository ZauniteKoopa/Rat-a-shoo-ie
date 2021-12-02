using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpotShadow : MonoBehaviour
{
    // Variables for detecting position of shadow
    public LayerMask spotShadowCollisionLayer;
    [SerializeField]
    private float raycastRadius = 0.1f;
    private Vector3[] CARDINAL_DIRECTIONS;

    // Variables for calculating scale of the shadow
    [SerializeField]
    private bool doesShadowScaling = false;
    private float minimumShadowSizePercent = 0.4f;
    private float minShadowSize;
    private float groundShadowSize;
    private float minShadowDistance = 1.0f;
    private float maxShadowDistance = 4.0f;

    // Main way to render the shadow
    private MeshRenderer render = null;
    private bool isActive = true;

    private void Start() {
        // Get the groundShadowSize
        groundShadowSize = transform.localScale.x;
        minShadowSize = groundShadowSize * minimumShadowSizePercent;

        // Set up cardinal directions
        CARDINAL_DIRECTIONS = new Vector3[4];
        CARDINAL_DIRECTIONS[0] = Vector3.left;
        CARDINAL_DIRECTIONS[1] = Vector3.right;
        CARDINAL_DIRECTIONS[2] = Vector3.forward;
        CARDINAL_DIRECTIONS[3] = Vector3.back;

        // Get MeshRenderer
        render = GetComponent<MeshRenderer>();
    }


    // Update is called once per frame to manage spot shadow
    void Update()
    {
        // Get position of shadow
        if (isActive) {
            float bestHeight = getShadowHeightPosition();
            transform.position = new Vector3(transform.parent.position.x, bestHeight, transform.parent.position.z);

            // Get shadow scaling
            if (doesShadowScaling) {
                float shadowScale = getScaledShadowSize(bestHeight);
                transform.localScale = new Vector3(shadowScale, transform.localScale.y, shadowScale);
            }
        }
    }

    // Private helper method to get the height position of the shadow
    private float getShadowHeightPosition() {
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
        return bestHeight;
    }

    // Main private helper method to get the overall size of the shadow
    private float getScaledShadowSize(float currentShadowY) {
        float shadowDistance = transform.parent.position.y - currentShadowY;
        shadowDistance = Mathf.Clamp(shadowDistance, minShadowDistance, maxShadowDistance);
        float interp = (shadowDistance - minShadowDistance) / (maxShadowDistance - minShadowDistance);

        float shadowScale = Mathf.Lerp(groundShadowSize, minShadowSize, interp);
        return shadowScale;
    }

    // Main method to enable spot shadow
    public void enable() {
        isActive = true;
        render.enabled = true;
    }

    // Main method to disable spot shadow
    public void disable() {
        isActive = false;
        render.enabled = false;
    }
}
