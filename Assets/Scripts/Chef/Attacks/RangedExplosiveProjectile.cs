﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TrailingStatus {
    STABLE,
    FIRED,
    CONVERGED
}

public class RangedExplosiveProjectile : RangedProjectile
{
    // Boolean flags
    private bool bombTimerActivated = false;
    private bool hookEnlarged = false;

    // Serialized fields
    [SerializeField]
    private GameObject anticipationZone = null;
    [SerializeField]
    private GameObject explosionZone = null;
    [SerializeField]
    private float explosiveAnticipationTime = 0.5f;
    [SerializeField]
    private float explosiveAttackTime = 0.3f;
    [SerializeField]
    private Color startAnticipationColor = Color.black;
    [SerializeField]
    private Color endAnticipationColor = Color.black;

    // Hook
    public LayerMask hookLayer;
    private float curDistance = 0.0f;
    private float hookedDistance = 0.0f;

    // Handling of trailing children
    [SerializeField]
    private Transform[] trailingChildren = null;
    [SerializeField]
    private float childrenInterval = 0.3f;
    private TrailingStatus[] childrenFiredStatus = null;
    private Vector3[] childrenPositions = null;
    private Vector3 originalPos = Vector3.zero;
    private int numChildrenConverged = 0;
    private int numChildrenFired = 0;
    private float trailTimer = 0.0f;


    // On awake, set up variables for trailing children
    private void Awake() {
        childrenFiredStatus = new TrailingStatus[trailingChildren.Length];
        childrenPositions = new Vector3[trailingChildren.Length];
        originalPos = transform.position;

        for (int i = 0; i < trailingChildren.Length; i++) {
            childrenFiredStatus[i] = TrailingStatus.STABLE;
            childrenPositions[i] = originalPos;
        }
    }

    // Main method to move the projectile
    protected override void moveProjectile() {
        if (!bombTimerActivated) {
            Vector3 translateVector = Time.fixedDeltaTime * projectileSpeed * projectileDirection;
            transform.Translate(translateVector);

            // At a certain distance, hook to the ground below
            if (!hookEnlarged) {
                curDistance += translateVector.magnitude;

                if (curDistance >= hookedDistance) {
                    hookEnlarged = true;
                }
            } else {
                RaycastHit hit;

                if (Physics.Raycast(transform.position, Vector3.down, out hit, 3f, hookLayer)) {
                    executeAfterEffects(false);
                }
            }
        }

        handleTrailingChildren();
    }

    // Private method to handle trailing children
    private void handleTrailingChildren() {
        // Update trailing status from STABLE -> MOVING depending on trailing time
        if (numChildrenFired < trailingChildren.Length && trailTimer >= childrenInterval * (numChildrenFired + 1)){
            childrenFiredStatus[numChildrenFired] = TrailingStatus.FIRED;
            trailingChildren[numChildrenFired].gameObject.SetActive(true);
            trailingChildren[numChildrenFired].position = originalPos;
            childrenPositions[numChildrenFired] = originalPos;

            numChildrenFired++;
        }

        // Update trailing status MOVING -> STOPPED if trailing children distance to stop distance is less than a threshold
        if (numChildrenConverged < trailingChildren.Length && childrenFiredStatus[numChildrenConverged] == TrailingStatus.FIRED) {
            bool didConverge = Vector3.Distance(trailingChildren[numChildrenConverged].position, transform.position) < 0.3f;

            if (didConverge) {
                trailingChildren[numChildrenConverged].gameObject.SetActive(false);
                childrenFiredStatus[numChildrenConverged] = TrailingStatus.CONVERGED;
                numChildrenConverged++;
            }
        }

        // Update transform of trailing children
        for (int i = 0; i < trailingChildren.Length; i++) {
            TrailingStatus currentStatus = childrenFiredStatus[i];

            if (currentStatus == TrailingStatus.FIRED) {
                childrenPositions[i] += (Time.fixedDeltaTime * projectileSpeed * projectileDirection);
                trailingChildren[i].position = childrenPositions[i];
            }
        }

        // Update trail timer
        trailTimer += Time.deltaTime;
    }

    // Main method to execute the after effects
    protected override void executeAfterEffects(bool hitPlayer) {
        if (hitPlayer) {
            Object.Destroy(gameObject);
        } else  {
            bombTimerActivated = true;
            GetComponent<Collider>().enabled = false;
            StartCoroutine(bombSequence());
        }
    }

    // IEnumerator to start bomb sequence
    private IEnumerator bombSequence() {
        // Set anticipation portion
        anticipationZone.SetActive(true);
        MeshRenderer anticipationRender = anticipationZone.GetComponent<MeshRenderer>();
        anticipationRender.material.color = startAnticipationColor;

        // Anticipation timer
        WaitForEndOfFrame waitFrame = new WaitForEndOfFrame();

        while (numChildrenConverged < trailingChildren.Length) {
            yield return waitFrame;
            
            float progress = (float)numChildrenConverged / (float)trailingChildren.Length;
            anticipationRender.material.color = Color.Lerp(startAnticipationColor, endAnticipationColor, progress);
        }

        // Small anticipation after
        yield return new WaitForSeconds(explosiveAnticipationTime);

        // Set explosive and then destroy object
        spriteTransform.GetComponent<SpriteRenderer>().enabled = false;
        anticipationRender.enabled = false;
        explosionZone.SetActive(true);

        yield return new WaitForSeconds(explosiveAttackTime);

        Object.Destroy(gameObject);
    }

    // Main method to set the hook distance
    public void setHookDistance(float hDist) {
        hookedDistance = hDist;
    }
    
}