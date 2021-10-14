using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class AbstractAggressiveChefAction : MonoBehaviour
{
    // 
    protected NavMeshAgent navMeshAgent;
    [SerializeField]
    protected Animator animator;
    protected ChefAudioManager audioManager;

    // On awake, get access to the NavMeshAgent
    private void Awake() {
        navMeshAgent = GetComponent<NavMeshAgent>();
        audioManager = GetComponent<ChefAudioManager>();
    }

    // Public method to override
    //  ChefSight: main way for chef to sense the rat
    //  lastSeenTarget: method just in case chef doesn't initially see rat anymore
    //  chaseMovementSpeed: the movement speed that the chef will go when chef chasing rat
    //
    //  POST: chef chases rat and does aggressive action (relationship between 2 action depends on the chef)
    public abstract IEnumerator doAggressiveAction(ChefSight chefSensing, Vector3 lastSeenTarget, float chaseMovementSpeed);

    // Main method to get rid of any lingering side effects for when chef's aggression sequence gets cancelled
    public abstract void cancelAggressiveAction();

    // Main method to make the attack action more angry
    public abstract void makeAngry();

}
