using ScriptableObjects.Interactable;
using UnityEngine;

public enum AnimationInteractionType
{
    Grab,
    Drop
}

public class AnimationInteraction : MonoBehaviour
{
    [SerializeField] private ItemAnimator itemAnimator;
    
    public void CheckInteraction(GameObject interactionObject, AnimationInteractionType type)
    {
        switch (interactionObject.tag)
        {
            case "Log":
                switch (type)
                {
                    case AnimationInteractionType.Grab:
                        itemAnimator.PlayGrab();
                        break;
                    case AnimationInteractionType.Drop:
                        itemAnimator.PlayDrop();
                        break;
                }
                break;

            // case "Radio":
            //     itemAnimator.PlayOnce("Radio_Pickup");
            //     break;
        }
    }
}
