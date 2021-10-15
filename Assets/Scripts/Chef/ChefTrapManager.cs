using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ChefTrapManager : MonoBehaviour
{
    [SerializeField]
    private Transform trapPrefab = null;
    [SerializeField]
    private int maxNumTraps = 5;
    public LayerMask trapCollisionMask;
    private Queue<Transform> cageManagementQueue = null;
    
    // Start is called before the first frame update
    void Start()
    {
        cageManagementQueue = new Queue<Transform>();
    }

    // Public method to spawn a cage at the current transform position
    //  Pre: none
    //  Post: spawn trap at current transform position. If number of traps spawned greater than maxNumTraps, despawn the oldest trap
    public void spawnTrap(UnityAction<Transform> onRatCageSetOff) {

        // Check if the chef should spawn a cage in the area he is in (is there a trap already there)
        bool isTrapPresent = Physics.CheckBox(transform.position, 0.5f * transform.localScale, Quaternion.identity, trapCollisionMask);

        // If trap is not present in area, set trap down
        if (!isTrapPresent) {
            Transform cageInstance = Object.Instantiate(trapPrefab, transform.position, Quaternion.identity);
            cageManagementQueue.Enqueue(cageInstance);
            cageInstance.GetComponent<RatCage>().trapTriggerEvent.AddListener(onRatCageSetOff);

            // If there are too many cages, despawn the earliest cage
            if (cageManagementQueue.Count > maxNumTraps) {
                Transform destroyedCage = cageManagementQueue.Dequeue();
                destroyedCage.GetComponent<RatCage>().trapTriggerEvent.RemoveListener(onRatCageSetOff);
                Object.Destroy(destroyedCage.gameObject);
            }   
        }
    }
}
