using System;
using System.Linq;
using JetBrains.Annotations;
using ScriptableObjects.Interactable;
using UnityEngine;

namespace Interactable.Holdable
{
    [RequireComponent(typeof(Rigidbody))]
    public class HoldableObjectBehaviour : InteractableBehaviour, IHoldableObject
    {
        [SerializeField] private HoldableObjectData data;
        private Rigidbody _rigidbody;
        private int _originalLayer;
        [CanBeNull] private IInteractor _holder;
        [CanBeNull] private GameObject _heldVersion;
        
        public bool IsCurrentlyHeld => _holder != null;
        public float Weight => data.Weight;

        private const int HoldLayer = 3;

        protected override void Awake()
        {
            base.Awake();
            _rigidbody = GetComponent<Rigidbody>();
            Renderers = GetComponentsInChildren<Renderer>();
        }
        
        protected void Start()
        {
            _originalLayer = gameObject.layer;
            InitializeHeldVersion();
        }
        
        public override void Interact(IInteractor interactor)
        {
            PickUp(interactor);
        }

        private void PickUp(IInteractor interactor)
        {
            if (_heldVersion) SetHeldVisual(true, _heldVersion);
            _holder = interactor;
            var heldObject = this;
            interactor.HeldObject?.Drop();
            interactor.SetHeldObject(heldObject);
            AttachTo(interactor);
            EnableCollider(false);
        }
        
        private void SetHeldVisual(bool state, GameObject heldVisual) {
            heldVisual.SetActive(state);
            foreach (var r in Renderers)
            {
                r.enabled = !state;
            }
        }
        
        public virtual void Drop()
        {
            if (_holder == null) throw new Exception("Tried to drop an item that wasn't being held");
            if (_heldVersion) SetHeldVisual(false, _heldVersion);
            _holder.SetHeldObject(null);
            _holder = null;
            Detach();
            EnableCollider(true);
        }

        public virtual void Place(Vector3 position, Quaternion? rotation = null)
        {
            if (_heldVersion) SetHeldVisual(false, _heldVersion);
            _holder?.SetHeldObject(null);
            _holder = null;
            _rigidbody.isKinematic = true;
            transform.SetParent(null);
            gameObject.layer = _originalLayer;
            transform.position = position;
            transform.rotation = rotation ?? Quaternion.identity;
            EnableCollider(true);
        }
        
        private void AttachTo(IInteractor interactor)
        {
            _rigidbody.isKinematic = true;
            transform.SetParent(interactor.HoldPoint);
            transform.localRotation = Quaternion.Euler(data.HoldingRotation);
            transform.localPosition = data.HoldingOffset;
            gameObject.layer = HoldLayer;
        }

        private void Detach()
        {
            _rigidbody.isKinematic = false;
            _rigidbody.AddForce((transform.parent?.forward ?? Vector3.zero) * data.DroppingForce);
            transform.SetParent(null);
            gameObject.layer = _originalLayer;
        }
        
        private void InitializeHeldVersion()
        {
            if (!data.HoldingPrefab) return;
            _heldVersion = Instantiate(data.HoldingPrefab, transform, true);
            _heldVersion!.transform.localPosition = data.HoldingPrefab.transform.position;
            _heldVersion.SetActive(false);
            DisableHeldVersionColliders();
        }

        private void DisableHeldVersionColliders()
        {
            var heldVersionColliders = _heldVersion?.GetComponents<Collider>();
            heldVersionColliders?.ToList().ForEach((col) => col.enabled = false);
            if (heldVersionColliders is { Length: > 0 }) Debug.LogError("Held prefab has colliders. They have been disabled.");
        }
    }
}