using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public enum InteractableWeight {
    LIGHT,
    HEAVY,
    IMMOVABLE
}

public class GeneralInteractable : MonoBehaviour
{
   public InteractableWeight weight = InteractableWeight.LIGHT;

   // heavy item hooks in local space of the interactable
   [SerializeField]
   private Vector3[] heavyItemHooks = null;

   // Public method to get nearest heavyItemHook in world space
   public Vector3 getNearestHeavyItemHook(Transform playerTransform) {
       Debug.Assert(heavyItemHooks != null && heavyItemHooks.Length > 0);

       float minDistance = 9999999f;
       Vector3 bestPosition = Vector3.zero;

       for (int i = 0; i < heavyItemHooks.Length; i++) {
           Vector3 localHook = heavyItemHooks[i];
           Vector3 worldHook = transform.TransformPoint(localHook);
           float currentDistance = Vector3.Distance(worldHook, playerTransform.position);

           if (currentDistance < minDistance) {
               minDistance = currentDistance;
               bestPosition = worldHook;
           }
       }

       return bestPosition;
   }
}
