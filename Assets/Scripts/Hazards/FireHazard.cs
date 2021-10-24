using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireHazard : MonoBehaviour
{
    [SerializeField]
    private Vector3 finalScale = Vector3.zero;
    [SerializeField]
    private float growingTime = 1.5f;
    [SerializeField]
    private float growthPercentDamage = 0.7f;
    private bool canHurtPlayer = false;

    // On awake, initiate growing sequence
    private void Awake() {
        StartCoroutine(growingSequence());
    }

    // IEnumerator for growing sequence
    private IEnumerator growingSequence() {
        Vector3 startScale = transform.parent.localScale;
        float timer = 0f;
        WaitForEndOfFrame waitFrame = new WaitForEndOfFrame();

        while (timer <= growingTime) {
            yield return waitFrame;
            timer += Time.deltaTime;

            float progress = timer / growingTime;
            canHurtPlayer = progress > growthPercentDamage;
            transform.parent.localScale = Vector3.Lerp(startScale, finalScale, progress);
        }

        transform.parent.localScale = finalScale;
    }

    // If a burnable destroyable object comes in, destroy it
    void OnTriggerEnter(Collider collider) {
        DestroyableObject targetObject = collider.GetComponent<DestroyableObject>();

        if (targetObject != null && targetObject.burnable) {
            targetObject.destroyObject();
        }
    }

    // Method for OnTriggerStay for the player, if player stays in fire, damage the player
    void OnTriggerStay(Collider collider) {
        if (canHurtPlayer) {
            RatController3D ratPlayer = collider.GetComponent<RatController3D>();

            if (ratPlayer != null) {
                ratPlayer.takeDamage();
            }
        }
    }
}
