using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class RatCage : MonoBehaviour
{
    // Boolean flag for when rat trap is in a triggering state
    private bool triggering = false;
    private MeshRenderer meshRender = null;
    [SerializeField]
    private float anticipationTime = 0.3f;
    [SerializeField]
    private float trapCooldown = 0.3f;
    [SerializeField]
    private int mashRequirement = 10;
    [SerializeField]
    private TrapSensor trapSensor = null;
    [SerializeField]
    private CageSensor cageSensor = null;
    [SerializeField]
    private GameObject mashUI = null;
    [SerializeField]
    private Image mashBar = null;
    [SerializeField]
    private GameObject pcPrompt = null;
    [SerializeField]
    private GameObject androidPrompt = null;

    // Current mash variables
    private bool trapped = false;
    private int currentMashTimes = 0;

    // On awake, set color to black
    private void Awake() {
        meshRender = GetComponent<MeshRenderer>();
        meshRender.material.color = Color.black;
        trapSensor.trapSensedEvent.AddListener(onTrapSensed);
        
        GameObject platformPrompt = (Application.platform == RuntimePlatform.Android) ? androidPrompt : pcPrompt;
        platformPrompt.SetActive(true);
    }

    // Main triggering sequence
    private IEnumerator triggeringSequence() {
        triggering = true;

        // Anticipation
        meshRender.material.color = Color.magenta;
        yield return new WaitForSeconds(anticipationTime);

        // Turn on cage sensor and see if there was something there
        meshRender.material.color = Color.red;
        cageSensor.GetComponent<MeshRenderer>().enabled = true;

        // If cage caught a player, enforce mashing sequence
        RatController3D caughtPlayer = cageSensor.getCaughtPlayer();
        if (caughtPlayer != null) {
            mashUI.SetActive(true);
            mashBar.fillAmount = 0f;
            caughtPlayer.setTrappedStatus(false);
            trapped = true;
            currentMashTimes = 0;

            WaitForEndOfFrame waitFrame = new WaitForEndOfFrame();
            while (currentMashTimes < mashRequirement && cageSensor.getCaughtPlayer() != null) {
                yield return waitFrame;
            }

            cageSensor.GetComponent<MeshRenderer>().enabled = false;
            mashUI.SetActive(false);
            caughtPlayer.setTrappedStatus(true);
            trapped = false;
        }

        // Cooldown
        meshRender.material.color = Color.black;
        yield return new WaitForSeconds(trapCooldown);
        cageSensor.GetComponent<MeshRenderer>().enabled = false;


        // If trap is still sensing an object. Trigger it again.
        triggering = trapSensor.isSensingObject();
        if (triggering) {
            StartCoroutine(triggeringSequence());
        }
    }

    // Event handler method for when player mashes interact button
    public void onInteractPress(InputAction.CallbackContext value) {
        if (trapped && value.started) {
            currentMashTimes++;
            mashBar.fillAmount = (float)currentMashTimes / (float)mashRequirement;
        }
    }

    // Event handler method for when player gets caught on trap
    public void onTrapSensed() {
        if (!triggering) {
            StartCoroutine(triggeringSequence());
        }
    }
}
