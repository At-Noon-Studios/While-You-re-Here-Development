using System.Linq;
using Interactable;
using Interactable.Holdable;
using UnityEngine;

namespace chopping_logs
{
    public class LogBasket : InteractableBehaviour
    {
        [Header("Basket Settings")]
        [SerializeField] private Transform[] logSlots;
        [SerializeField] private string acceptedTag = "HalfLog";

        private HoldableObjectBehaviour _basketHoldable;

        private int CurrentCount =>
            logSlots.Count(slot => slot != null && slot.childCount > 0);

        private bool IsFull =>
            logSlots == null || CurrentCount >= logSlots.Length;

        private bool IsEmpty =>
            logSlots == null || logSlots.All(slot => slot == null || slot.childCount == 0);

        protected override void Awake()
        {
            base.Awake();
            _basketHoldable = GetComponent<HoldableObjectBehaviour>();
        }

        public override bool IsInteractableBy(IInteractor interactor)
        {
            if (blockInteraction) return false;

            var held = interactor.HeldObject;

            // Iets in hand → proberen op te slaan
            if (held != null)
            {
                if (!TryGetHeldGameObject(held, out var go))
                    return false;

                if (!IsAccepted(go))
                    return false;

                return !IsFull;
            }

            // Niets in hand → altijd interactie mogelijk
            // (log pakken OF basket oppakken)
            return true;
        }

        public override void Interact(IInteractor interactor)
        {
            var held = interactor.HeldObject;

            // LOG IN BASKET LEGGEN
            if (held != null)
            {
                TryStoreLog(held, interactor);
                return;
            }

            // LOG UIT BASKET HALEN
            if (!IsEmpty)
            {
                TakeLog(interactor);
                return;
            }

            // BASKET OPPAKKEN (alleen als leeg)
            TryPickUpBasket(interactor);
        }

        private void TryStoreLog(IHoldableObject held, IInteractor interactor)
        {
            if (IsFull) return;
            if (!TryGetHeldGameObject(held, out var go)) return;
            if (!IsAccepted(go)) return;

            var freeSlot = GetFirstFreeSlot();
            if (freeSlot == null) return;

            held.Place(freeSlot.position, freeSlot.rotation);

            go.transform.SetParent(freeSlot);
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;

            interactor.SetHeldObject(null);
        }

        private Transform GetFirstFreeSlot()
        {
            foreach (var slot in logSlots)
            {
                if (slot == null) continue;
                if (slot.childCount == 0)
                    return slot;
            }
            return null;
        }

        private void TakeLog(IInteractor interactor)
        {
            if (interactor is not PlayerInteractionController player) return;
            if (player.HeldObject != null) return;

            for (int i = logSlots.Length - 1; i >= 0; i--)
            {
                var slot = logSlots[i];
                if (slot == null || slot.childCount == 0) continue;

                var log = slot.GetComponentInChildren<HoldableObjectBehaviour>();
                if (log == null) continue;

                log.PickUpByInteractor(player);
                return;
            }
        }

        private void TryPickUpBasket(IInteractor interactor)
        {
            if (_basketHoldable == null) return;
            if (interactor is not PlayerInteractionController player) return;
            if (player.HeldObject != null) return;

            _basketHoldable.PickUpByInteractor(player);
        }

        public override string InteractionText(IInteractor interactor)
        {
            var held = interactor.HeldObject;

            if (held == null)
            {
                if (!IsEmpty)
                    return "Pak een houtblok uit de mand";

                return _basketHoldable != null
                    ? "Pak de mand op"
                    : string.Empty;
            }

            if (!TryGetHeldGameObject(held, out var go))
                return string.Empty;

            if (!IsFull && IsAccepted(go))
                return "Leg het hout in de mand";

            return string.Empty;
        }

        // =========================
        // Helpers (CRASH-PROOF)
        // =========================

        private bool TryGetHeldGameObject(IHoldableObject held, out GameObject go)
        {
            if (held is MonoBehaviour mb)
            {
                go = mb.gameObject;
                return true;
            }

            go = null;
            return false;
        }

        private bool IsAccepted(GameObject go)
        {
            if (string.IsNullOrEmpty(acceptedTag))
                return true;

            // SAFE: crasht nooit
            return go.tag == acceptedTag;
        }
    }
}
