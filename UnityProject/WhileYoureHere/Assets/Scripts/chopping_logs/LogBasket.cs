using System.Linq;
using Interactable;
using Interactable.Holdable;
using UnityEngine;

namespace chopping_logs
{
    public class LogBasket : InteractableBehaviour
    {
        [SerializeField] private Transform[] logSlots;
        [SerializeField] private string acceptedTag = "HalfLog";

        private int CurrentCount => logSlots.Count(s => s != null && s.childCount > 0);
        private bool IsFull => CurrentCount >= logSlots.Length;

        public override bool IsInteractableBy(IInteractor interactor)
        {
            if (blockInteraction) return false;

            var held = interactor.HeldObject;

            if (held == null)
            {
                return CurrentCount > 0;
            }

            if (!TryGetGameObject(held, out var go)) return false;
            if (!IsAccepted(go)) return false;

            return !IsFull;
        }

        public override void Interact(IInteractor interactor)
        {
            var held = interactor.HeldObject;

            if (held != null)
            {
                TryStoreLog(held, interactor);
            }
            else
            {
                TryTakeLog(interactor);
            }
        }

        private void TryStoreLog(IHoldableObject held, IInteractor interactor)
        {
            if (IsFull) return;
            if (!TryGetGameObject(held, out var go)) return;
            if (!IsAccepted(go)) return;

            var slot = logSlots.FirstOrDefault(s => s != null && s.childCount == 0);
            if (slot == null) return;

            held.Place(slot.position, slot.rotation);
            go.transform.SetParent(slot);
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;

            interactor.SetHeldObject(null);
        }

        private void TryTakeLog(IInteractor interactor)
        {
            if (CurrentCount == 0) return;

            var slot = logSlots.LastOrDefault(s => s != null && s.childCount > 0);
            if (slot == null) return;

            var logObject = slot.GetChild(0).gameObject;
            var holdable = logObject.GetComponent<IHoldableObject>();
            if (holdable == null) return;

            if (holdable is HoldableObjectBehaviour hob)
            {
                hob.PickUpByInteractor(interactor);
            }
            else
            {
                logObject.transform.SetParent(null);
                interactor.SetHeldObject(holdable);
            }
        }


        public override string InteractionText(IInteractor interactor)
        {
            var held = interactor.HeldObject;

            if (held != null)
            {
                if (!TryGetGameObject(held, out var go)) return string.Empty;
                if (!IsAccepted(go) || IsFull) return string.Empty;
                return "Put log in crate";
            }

            if (CurrentCount > 0)
            {
                return "Take log from crate";
            }

            return string.Empty;
        }

        private bool TryGetGameObject(IHoldableObject held, out GameObject go)
        {
            go = held is MonoBehaviour mb ? mb.gameObject : null;
            return go != null;
        }

        private bool IsAccepted(GameObject go)
        {
            return string.IsNullOrEmpty(acceptedTag) || go.CompareTag(acceptedTag);
        }
    }
}
