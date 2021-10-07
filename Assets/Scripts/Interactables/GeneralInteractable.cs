using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public enum InteractableWeight {
    LIGHT,
    IMMOVABLE
}

public class GeneralInteractable : MonoBehaviour
{
   public InteractableWeight weight = InteractableWeight.LIGHT;
   public bool canBePickedUp = true;

   // heavy item hooks in local space of the interactable
   [SerializeField]
   private GameObject highlightedIndicator = null;

   // Event method to indicate interaction start, can be overrided
   public virtual void onPlayerInteractStart() {}

   // Event method to indicate interaction end
   public virtual void onPlayerInteractEnd() {}

   // Main method to do interaction sequence
   public virtual IEnumerator interactionSequence() {
       yield return 0;
   }

   // Public method to highlight this interactable for selection
   public void highlight() {
       highlightedIndicator.SetActive(true);
   }

   // Public method to unhighlight interactable for selection
   public void removeHighlight() {
       highlightedIndicator.SetActive(false);
   }

}
