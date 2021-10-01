using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomSensor : MonoBehaviour
{
    // Always assume that the room view is the room sensor's parent
    private RoomView roomView;

    // On start, set room view
    void Start() {
        roomView = transform.parent.GetComponent<RoomView>();
    }

    // On trigger enter for the player, signal to the room view
    void OnTriggerEnter(Collider collider) {
        RatController3D ratPlayer = collider.GetComponent<RatController3D>();

        if (ratPlayer != null) {
            roomView.onPlayerEnter(ratPlayer);
        }
    }
}
