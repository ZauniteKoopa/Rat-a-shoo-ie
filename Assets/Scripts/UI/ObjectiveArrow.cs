using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveArrow : MonoBehaviour
{
    public GameObject objectiveIndicator;
    public GameObject player;
    public float arrowDistance;
    public bool active;
    // Start is called before the first frame update
    void Start()
    {
        active = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (active){
            Vector3 playerLocation = player.transform.position;
            Vector3 objectiveLocation = objectiveIndicator.transform.position;
            Vector3 direction = (objectiveLocation - playerLocation).normalized;
            Vector3 arrowPosition = playerLocation + (direction * arrowDistance);
            this.transform.position = arrowPosition;
            float xDegrees = (Mathf.Atan2(direction.z, direction.y) * Mathf.Rad2Deg);
            float yDegrees = (Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg);
            float zDegrees = (Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
            this.transform.eulerAngles = new Vector3(xDegrees, yDegrees, zDegrees);
        }

    }
}
