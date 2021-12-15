using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.Events;

[System.Serializable]
public class TrapTriggerDelegate : UnityEvent<Transform> {}

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
    [SerializeField]
    private GameObject cageModel = null;
    [SerializeField]
    private AudioClip cageAnticipationSound = null;
    [SerializeField]
    private AudioClip cageLockSound = null;
    [SerializeField]
    private AudioClip cageResetSound = null;
    private AudioSource speaker = null;
    public TrapTriggerDelegate trapTriggerEvent;

    // Current mash variables
    private bool trapped = false;
    private int currentMashTimes = 0;
    public bool caughtPlayer = false;

    private Rigidbody rb;

    // On awake, set color to black
    private void Awake() {
        meshRender = GetComponent<MeshRenderer>();
        rb = GetComponent<Rigidbody>();
        meshRender.material.color = Color.black;
        trapSensor.trapSensedEvent.AddListener(onTrapSensed);
        speaker = GetComponent<AudioSource>();

        GameObject platformPrompt = (Application.platform == RuntimePlatform.Android) ? androidPrompt : pcPrompt;
        platformPrompt.SetActive(true);
    }

    // Main triggering sequence
    private IEnumerator triggeringSequence() {
        triggering = true;

        // Anticipation
        meshRender.material.color = Color.magenta;
        speaker.clip = cageAnticipationSound;
        speaker.PlayOneShot(cageAnticipationSound, 1.75f);
        rb.constraints = RigidbodyConstraints.FreezeAll;
        yield return new WaitForSeconds(anticipationTime);

        // Turn on cage sensor and see if there was something there
        trapTriggerEvent.Invoke(transform);
        meshRender.material.color = Color.red;
        cageModel.SetActive(true);
        speaker.clip = cageLockSound;
        speaker.PlayOneShot(cageLockSound, 1.75f);
        yield return null;

        // If cage caught a player, enforce mashing sequence
        RatController3D caughtPlayer = cageSensor.getCaughtPlayer();
        bool platformMade = false;

        if (caughtPlayer != null) {
            mashUI.SetActive(true);
            mashBar.fillAmount = 0f;
            caughtPlayer.transform.position = transform.position;
            caughtPlayer.setTrappedStatus(false);
            trapped = true;
            currentMashTimes = 0;

            WaitForEndOfFrame waitFrame = new WaitForEndOfFrame();
            while (currentMashTimes < mashRequirement && cageSensor.getCaughtPlayer() != null) {
                yield return waitFrame;
            }

            cageModel.SetActive(false);
            mashUI.SetActive(false);
            caughtPlayer.setTrappedStatus(true);
            trapped = false;
        } else  {
            cageSensor.activatePlatform();
            platformMade = true;
        }

        // Cooldown
        meshRender.material.color = Color.blue;
        yield return new WaitForSeconds(trapCooldown);
        meshRender.material.color = Color.black;

        // If platform made, reset platform
        if (platformMade) {
            cageSensor.resetPlatform();
        }

        cageModel.SetActive(false);
        speaker.clip = cageResetSound;
        speaker.PlayOneShot(cageResetSound, 1.5f);

        // If trap is still sensing an object. Trigger it again.
        triggering = trapSensor.isSensingObject();
        if (triggering) {
            StartCoroutine(triggeringSequence());
        }
    }

    // Event handler method for when player mashes interact button
    public void onMashPress(InputAction.CallbackContext value) {
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

    // Main accessor method to check if the cage currently holds a caught player
    public bool hasCaughtPlayer() {
        return trapped;
    }
}
