using System.Linq;
using Interactable;
using Interactable.Holdable;
using UnityEngine;

namespace chopping_logs
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(HoldableObjectBehaviour))]
    public class LogBasket : InteractableBehaviour
    {
        [SerializeField] private Transform[] logSlots;
        [SerializeField] private string acceptedTag = "HalfLog";

        private HoldableObjectBehaviour _holdable;

        private int CurrentCount => logSlots.Count(s => s.childCount > 0);
        private bool IsFull => CurrentCount >= logSlots.Length;

        protected override void Awake()
        {
            base.Awake();
            _holdable = GetComponent<HoldableObjectBehaviour>();
        }

        public override bool IsInteractableBy(IInteractor interactor)
        {
            if (blockInteraction) return false;

            if (interactor.HeldObject != null)
            {
                if (!(interactor.HeldObject is MonoBehaviour mb)) return false;
                if (!IsAccepted(mb.gameObject)) return false;
                return !IsFull;
            }

            return !_holdable.IsCurrentlyHeld;
        }

        public override void Interact(IInteractor interactor)
        {
            if (interactor.HeldObject != null)
            {
                TryStoreLog(interactor.HeldObject, interactor);
                return;
            }

            _holdable.Interact(interactor);
        }

        private void TryStoreLog(IHoldableObject held, IInteractor interactor)
        {
            if (IsFull || !(held is MonoBehaviour mb)) return;
            if (!IsAccepted(mb.gameObject)) return;

            var slot = logSlots.FirstOrDefault(s => s.childCount == 0);
            if (slot == null) return;

            held.Place(slot.position, slot.rotation);
            mb.transform.SetParent(slot);
            mb.transform.localPosition = Vector3.zero;
            mb.transform.localRotation = Quaternion.identity;

        }

        private bool IsAccepted(GameObject go)
        {
            return string.IsNullOrEmpty(acceptedTag) || go.CompareTag(acceptedTag);
        }
    }
}
