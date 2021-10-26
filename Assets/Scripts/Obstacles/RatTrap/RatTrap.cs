using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RatTrap : MonoBehaviour
{
    // Boolean flag for when rat trap is in a triggering state
    private bool triggering = false;
    private MeshRenderer meshRender = null;
    private RatTrapAudioManager audioManager = null;
    [SerializeField]
    private float anticipationTime = 0.3f;
    [SerializeField]
    private float attackingTime = 0.1f;
    [SerializeField]
    private float trapCooldown = 0.3f;
    [SerializeField]
    private GameObject hitbox = null;
    [SerializeField]
    private TrapSensor trapSensor = null;
    [SerializeField]
    private Animator trapAnimator = null;

    // On awake, set color to black
    private void Awake() {
        meshRender = GetComponent<MeshRenderer>();
        audioManager = GetComponent<RatTrapAudioManager>();
        meshRender.material.color = Color.black;
        hitbox.SetActive(false);
        trapSensor.trapSensedEvent.AddListener(onTrapSensed);
    }

    // Main triggering sequence
    private IEnumerator triggeringSequence() {
        triggering = true;

        // Anticipation
        meshRender.material.color = Color.magenta;
        yield return new WaitForSeconds(anticipationTime);

        // Attack
        if (trapAnimator != null) {
            trapAnimator.SetBool("Activated", true);
        }

        meshRender.material.color = Color.red;
        hitbox.SetActive(true);
        audioManager.playSnapSound();
        yield return new WaitForSeconds(attackingTime);

        // Cooldown
        meshRender.material.color = Color.blue;
        hitbox.SetActive(false);
        yield return new WaitForSeconds(trapCooldown);

        // If trap is still sensing an object. Trigger it again.
        meshRender.material.color = Color.black;

        if (trapAnimator != null) {
            trapAnimator.SetBool("Activated", false);
        }

        audioManager.playResetSound();
        yield return new WaitForSeconds(audioManager.getCurrentClipLength());

        triggering = trapSensor.isSensingObject();
        if (triggering) {
            StartCoroutine(triggeringSequence());
        }
    }

    // Event handler method for when player gets caught on trap
    public void onTrapSensed() {
        if (!triggering) {
            StartCoroutine(triggeringSequence());
        }
    }
}
