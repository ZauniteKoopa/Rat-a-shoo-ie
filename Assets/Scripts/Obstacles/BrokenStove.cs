using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrokenStove : MonoBehaviour
{
    private BrokenStoveAudioManager audioManager = null;

    [SerializeField]
    private GameObject hitbox = null;
    [SerializeField]
    private float normalTime = 1.5f;
    [SerializeField]
    private float anticipationTime = 0.5f;
    [SerializeField]
    private float attackTime = 0.5f;
    [SerializeField]
    private float initialStartTime = 1.5f;
    [SerializeField]
    private Color normalColor = Color.black;
    [SerializeField]
    private Color anticipationColor = Color.magenta;
    [SerializeField]
    private Color attackColor = Color.red;

    private MeshRenderer meshRender = null;

    // Start is called before the first frame update
    private void Start() {
        meshRender = GetComponent<MeshRenderer>();
        meshRender.material.color = normalColor;
        audioManager = GetComponent<BrokenStoveAudioManager>();
    }

    // Core Obstacle loop
    private IEnumerator stoveCycle() {
        hitbox.SetActive(false);
        yield return new WaitForSeconds(initialStartTime);

        while (true) {
            meshRender.material.color = anticipationColor;
            audioManager.playArmSound();
            yield return new WaitForSeconds(anticipationTime);

            meshRender.material.color = attackColor;
            hitbox.SetActive(true);
            audioManager.playFireSound();
            yield return new WaitForSeconds(attackTime);

            hitbox.SetActive(false);
            audioManager.stopSounds();
            meshRender.material.color = normalColor;
            yield return new WaitForSeconds(normalTime);
        }
    }

    // Main method to activate it
    public void activate() {
        StartCoroutine(stoveCycle());
    }

    // Main method to deactivate
    public void deactivate() {
        StopAllCoroutines();
    }
}
