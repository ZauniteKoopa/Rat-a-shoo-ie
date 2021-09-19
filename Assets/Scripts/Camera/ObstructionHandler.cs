using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstructionHandler : MonoBehaviour
{
    public Material semiTransparent;
    private Dictionary<GameObject, Material> prevObstructions;
    
    private Ray camAngle;
    private GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
  
    }

    // Update is called once per frame
    void Update()
    {
        Dictionary<GameObject, Material> curObstructions = new Dictionary<GameObject, Material>();
        Vector3 camToPlayer = player.transform.position - this.transform.position;
        camAngle = new Ray(this.transform.position, camToPlayer);
        var rayHits = Physics.RaycastAll(camAngle, camToPlayer.magnitude);

        //Store a reference to each obstruction and its material, then change its material to be semitransparent.
        foreach(RaycastHit hit in rayHits) 
        {
            GameObject obs = hit.collider.gameObject;
            curObstructions.Add(obs, obs.GetComponent<Renderer>().material);
            obs.GetComponent<Renderer>().material = semiTransparent;
            
            //Remove obstruction reference from prev so that we don't reset it.
            if (prevObstructions.ContainsKey(obs))
            {
                prevObstructions.Remove(obs);
            }
        }

        //Reset previous obstructions' materials-- i.e. objects that are no longer obstructing the view of the player.
        foreach(GameObject obs in prevObstructions.Keys)
        {
            obs.GetComponent<Renderer>().material = prevObstructions[obs];
        }

        prevObstructions = curObstructions;
       
       
    }
}
