using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstructionHandler : MonoBehaviour
{
    public Material semiTransparent;
    private Dictionary<GameObject, Material[]> curObstructions;
    public LayerMask obsMask;
    
    private Ray camAngle;
    private GameObject player;


    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        curObstructions = new Dictionary<GameObject, Material[]>();

  
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 camToPlayer = player.transform.position - this.transform.position;
        camAngle = new Ray(this.transform.position, camToPlayer);
        //Debug.DrawRay(this.transform.position, camToPlayer);
        var rayHits = Physics.RaycastAll(camAngle, camToPlayer.magnitude, obsMask);


        HashSet<GameObject> curHit = new HashSet<GameObject>();
        //Store a reference to each obstruction and its material, then change its material to be semitransparent.
        foreach (RaycastHit hit in rayHits)
        {
            //get all sibling objects
            if (hit.transform.parent != null)
            {
                GameObject parent = hit.transform.parent.gameObject;


                //additional foreach loop for each child

                for (int c = 0; c < parent.transform.childCount; c++)
                {
                    GameObject obs = parent.transform.GetChild(c).gameObject;
                    curHit.Add(obs);
                    if (!curObstructions.ContainsKey(obs) && obs.GetComponent<Renderer>() != null)
                    {
                        Material[] obsMaterials = obs.GetComponent<Renderer>().materials;
                        curObstructions.Add(obs, obsMaterials);
                        Material[] semiTransArray = new Material[obsMaterials.Length];
                        for (int i = 0; i < obsMaterials.Length; i++)
                        {
                            semiTransArray[i] = semiTransparent;
                        }
                        obs.GetComponent<Renderer>().materials = semiTransArray;
                    }

                }
            }
            else
            {
                GameObject obs = hit.transform.gameObject;

                curHit.Add(obs);
                if (!curObstructions.ContainsKey(obs) && obs.GetComponent<Renderer>() != null)
                {
                    Material[] obsMaterials = obs.GetComponent<Renderer>().materials;
                    curObstructions.Add(obs, obsMaterials);
                    Material[] semiTransArray = new Material[obsMaterials.Length];
                    for (int i = 0; i < obsMaterials.Length; i++)
                    {
                        semiTransArray[i] = semiTransparent;
                    }
                    obs.GetComponent<Renderer>().materials = semiTransArray;
                }
            }

        }
        HashSet<GameObject> resetObjects = new HashSet<GameObject>();
        foreach(GameObject obs in curObstructions.Keys)
        {
            if (!curHit.Contains(obs))
            {
                resetObjects.Add(obs);
                obs.GetComponent<Renderer>().materials = curObstructions[obs];
            }
        }
        foreach(GameObject obs in resetObjects)
        {
            curObstructions.Remove(obs);
        }



       
    }
}
