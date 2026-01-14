using System;
using System.Collections.Generic;
using System.Linq;
using Interactable.Holdable;
using JetBrains.Annotations;
using making_tea;
using UnityEngine;

namespace Interactable.Concrete.ObjectHolder
{
    public class ObjectHolderSingle : InteractableBehaviour
    {
        [Header("Placement")]
        [SerializeField] private Transform placePoint;
        [SerializeField] private Vector3 placedObjectRotation;

        [Header("Tablemode")] 
        [SerializeField] private bool allowTableMode = true;
        [SerializeField] private float tableDetectHeight = 0.75f;
        [SerializeField] private float tableDetectRadius = 0.18f;
        [SerializeField] private LayerMask tableDetectMask = ~0;

        [CanBeNull] private IHoldableObject _heldObject;

        private ITablePickup _tableCandidate;
        
        private static readonly HashSet<ObjectHolderSingle> All = new();
        private static readonly RaycastHit[] Hits = new RaycastHit[12];

        public event Action<IHoldableObject> OnPlaced;
        public event Action<IHoldableObject> OnRemoved;
        
        public bool HasHeldObject => _heldObject != null;
        private void OnEnable()
        {
            All.Add(this);
        }

        private void OnDisable()
        {
            All.Remove(this);
        }

        protected override void Awake()
        {
            base.Awake();
        }

        private void Update()
        {
            UpdateTableCandidate();
        }

        private void UpdateTableCandidate()
        {
            if (!allowTableMode || _heldObject != null)
            {
                _tableCandidate = null;
                return;
            }

            var origin = placePoint.position + Vector3.up * 0.02f;
            var hitCount = Physics.SphereCastNonAlloc(
                origin,
                tableDetectRadius,
                Vector3.up,
                Hits,
                tableDetectHeight,
                tableDetectMask,
                QueryTriggerInteraction.Collide
            );

            ITablePickup best = null;
            var bestDist = float.MaxValue;

            for (var i = 0; i < hitCount; i++)
            {
                var col = Hits[i].collider;
                if (col == null) continue;

                var tp = col.GetComponentInParent<ITablePickup>();
                if (tp == null) continue;
                if (!tp.IsTableHeld) continue;

                var d = Hits[i].distance;
                if (!(d < bestDist)) continue;
                bestDist = d;
                best = tp;
            }

            _tableCandidate = best;
        }

        public static bool TryPlaceHeldTablePickup(PlayerInteractionController pic)
        {
            if (pic == null) return false;

            var held = pic.GetHeldTablePickup();
            if (held == null) return false;

            foreach (var holder in from holder in All where holder != null where holder.allowTableMode where holder._heldObject == null where holder._tableCandidate != null where ReferenceEquals(holder._tableCandidate, held) where holder._tableCandidate.IsTableHeld select holder)
            {
                holder.PlaceFromTableCandidate(held);
                return true;
            }

            return false;
        }

        private void PlaceFromTableCandidate(ITablePickup candidate)
        {
            if (_heldObject != null) return;
            if (candidate is not { IsTableHeld: true }) return;

            var pickup = candidate as TablePickup;
            if (pickup == null) return;

            var rootObj = pickup.gameObject;

            var holdable = rootObj.GetComponent<IHoldableObject>() ?? rootObj.GetComponentInChildren<IHoldableObject>();

            if (holdable == null)
                return;

            pickup.ForceDropFromTableMode();

            var rbs = rootObj.GetComponentsInChildren<Rigidbody>(true);
            
            foreach (var t in rbs)
            {
                t.isKinematic = true;
                t.linearVelocity = Vector3.zero;
                t.angularVelocity = Vector3.zero;
            }

            _heldObject = holdable;

            _heldObject.Place(placePoint.position, Quaternion.Euler(placedObjectRotation), this);

            rootObj.transform.SetParent(placePoint);
            rootObj.transform.localPosition = Vector3.zero;
            rootObj.transform.localRotation = Quaternion.Euler(placedObjectRotation);

            _tableCandidate = null;

            OnPlaced?.Invoke(_heldObject);
            
        }
        
        public bool CanPlaceHeldTablePickup(PlayerInteractionController pic)
        {
            if (pic == null) return false;
            if (!allowTableMode) return false;
            if (_heldObject != null) return false;

            var held = pic.GetHeldTablePickup();
            if (held == null) return false;

            return _tableCandidate != null &&
                   ReferenceEquals(_tableCandidate, held) &&
                   held.IsTableHeld;
        }


        public override void Interact(IInteractor interactor)
        {
            var pic = interactor as PlayerInteractionController;

            if (pic != null && pic.IsTableMode)
            {
                if (!allowTableMode) return;
                if (_heldObject != null) return;

                if (_tableCandidate is { IsTableHeld: true })
                    PlaceFromTableCandidate(_tableCandidate);

                return;
            }

            if (_heldObject != null)
                return;

            if (interactor.HeldObject == null)
                return;

            _heldObject = interactor.HeldObject;

            _heldObject.Place(
                placePoint.position,
                Quaternion.Euler(placedObjectRotation),
                this
            );

            var go = ((MonoBehaviour)_heldObject).gameObject;
            go.transform.SetParent(placePoint);
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.Euler(placedObjectRotation);

            OnPlaced?.Invoke(_heldObject);
        }

        public void ClearHeldObject(IHoldableObject obj)
        {
            if (_heldObject == null) return;

            if (ReferenceEquals(_heldObject, obj))
            {
                OnRemoved?.Invoke(obj);
                _heldObject = null;
                return;
            }

            var heldMb = _heldObject as MonoBehaviour;
            var objMb = obj as MonoBehaviour;

            if (heldMb == null || objMb == null) return;
            if (!heldMb.transform.IsChildOf(objMb.transform) && !objMb.transform.IsChildOf(heldMb.transform)) return;
            OnRemoved?.Invoke(_heldObject);
            _heldObject = null;
        }

        public override bool IsInteractableBy(IInteractor interactor)
        {
            if (interactor is not PlayerInteractionController { IsTableMode: true })
                return _heldObject == null && interactor.HeldObject is IPlaceable;
            
            if (!allowTableMode) return false;
            return _heldObject == null && _tableCandidate is { IsTableHeld: true };
        }
        
    }
}