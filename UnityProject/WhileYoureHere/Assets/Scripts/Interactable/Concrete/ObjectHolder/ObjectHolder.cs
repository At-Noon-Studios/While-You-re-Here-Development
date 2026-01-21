using System.Collections.Generic;
using System.Linq;
using Interactable.Holdable;
using make_a_fire;
using UnityEngine;

namespace Interactable.Concrete.ObjectHolder
{
    public class ObjectHolder : InteractableBehaviour, IObjectHolder
    {
        [Header("Placement")]
        [SerializeField] private Transform placePoint;
        [SerializeField] private Vector3 placedObjectRotation;
        [SerializeField] private List<PlacedObjectData> objectsToBePlaced = new List<PlacedObjectData>();
        public readonly List<GameObject> placedObjectsInHolders = new List<GameObject>();

        [SerializeField] private AudioSource _audioSource;

        public override void Interact(IInteractor interactor)
        {
            var heldObject = interactor.HeldObject;
            if (heldObject == null) return;

            var heldGameObject = (heldObject as Component)?.gameObject;
            if (!heldGameObject) return;

            var placedDatas = objectsToBePlaced.FindAll(e => e.objectPrefab.CompareTag(heldGameObject.tag));
            if (placedDatas.Count == 0) return;
            var placedData = placedDatas[placedObjectsInHolders.FindAll(e => e.gameObject.CompareTag(heldGameObject.tag)).Count];

            _audioSource.PlayOneShot(placedData.audioClip);
            heldObject.Place(
                placePoint.position,
                Quaternion.Euler(placedData.placedObjectRotation),
                this
            );
            placedObjectsInHolders.Add(heldGameObject);
            
            interactor.SetHeldObject(null);
        }

        public override bool IsInteractableBy(IInteractor interactor)
        {
            if (blockInteraction) return false;

            var heldObject = interactor.HeldObject;
            if (heldObject == null) return false;

            var heldGameObject = (heldObject as Component)?.gameObject;
            if (!heldGameObject) return false;

            return objectsToBePlaced.Any(e => e.objectPrefab.GetType() == heldGameObject.GetType());
        }

        public override string InteractionText(IInteractor interactor)
        {
            if (!IsInteractableBy(interactor))
                return string.Empty;

            return "Place " + interactor.HeldObject.InteractionText(interactor);
        }
        
        public void ClearHeldObject(GameObject obj)
        {
            placedObjectsInHolders.Remove(obj);
        }
    }
}