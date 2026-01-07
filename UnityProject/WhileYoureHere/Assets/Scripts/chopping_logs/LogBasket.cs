using System.Linq;
using Interactable;
using Interactable.Holdable;
using UnityEngine;

namespace chopping_logs
{
    [RequireComponent(typeof(Rigidbody))]
    public class LogBasket : InteractableBehaviour
    {
        [SerializeField] private Transform[] logSlots;
        [SerializeField] private string acceptedTag = "HalfLog";
        [SerializeField] private Vector3 carryOffsetPosition;
        [SerializeField] private Vector3 carryOffsetEulerAngles;

        private IInteractor _currentCarrier;
        private Rigidbody _rb;

        private int CurrentCount => logSlots.Count(s => s != null && s.childCount > 0);
        private bool IsFull => CurrentCount >= logSlots.Length;
        private bool IsCarried => _currentCarrier != null;

        protected override void Awake()
        {
            base.Awake();
            _rb = GetComponent<Rigidbody>();
        }

        public override bool IsInteractableBy(IInteractor interactor)
        {
            if (blockInteraction) return false;

            var held = interactor.HeldObject;

            if (held != null)
            {
                if (!TryGetGameObject(held, out var go)) return false;
                if (!IsAccepted(go)) return false;

                return !IsFull;
            }

            if (!IsCarried) return true;
            return _currentCarrier == interactor;
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
                ToggleCarry(interactor);
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

        private void ToggleCarry(IInteractor interactor)
        {
            if (!IsCarried)
            {
                _currentCarrier = interactor;

                if (_rb != null)
                    _rb.isKinematic = true;

                var holdPoint = interactor.HoldPoint;
                if (holdPoint != null)
                {
                    transform.SetParent(holdPoint);
                    transform.localPosition = carryOffsetPosition;
                    transform.localRotation = Quaternion.Euler(carryOffsetEulerAngles);
                }

                return;
            }

            if (_currentCarrier == interactor)
            {
                transform.SetParent(null, true);

                if (_rb != null)
                    _rb.isKinematic = false;

                _currentCarrier = null;
            }
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

        public bool IsCarriedBy(IInteractor interactor)
        {
            return IsCarried && _currentCarrier == interactor;
        }

        public void DropIfCarriedBy(IInteractor interactor)
        {
            if (IsCarriedBy(interactor))
            {
                transform.SetParent(null, true);

                if (_rb != null)
                    _rb.isKinematic = false;

                _currentCarrier = null;
            }
        }
    }
}