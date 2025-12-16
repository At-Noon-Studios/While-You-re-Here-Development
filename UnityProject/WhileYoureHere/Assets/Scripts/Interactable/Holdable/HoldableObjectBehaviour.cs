using System;
using System.Linq;
using ScriptableObjects.Interactable.Holdable;
using UnityEngine;
using Interactable.Concrete.ObjectHolder;
using JetBrains.Annotations;

namespace Interactable.Holdable
{
    [RequireComponent(typeof(Rigidbody))]
    public class HoldableObjectBehaviour : InteractableBehaviour, IHoldableObject
    {
        [Header("Interaction UI")]
        [SerializeField] private Canvas interactionCanvas;

        [Header("Holdable Data")]
        [SerializeField] private HoldableObjectData data;

        private Rigidbody _rigidbody;
        private int _originalLayer;
        [CanBeNull] private IInteractor _holder;
        [CanBeNull] private GameObject _heldVersion;

        private Transform _playerCamera;

        private ObjectHolder _currentHolder;
        
        private const int HoldLayer = 3;

        public bool IsCurrentlyHeld => _holder != null;
        public float Weight => data.Weight;

        protected override void Awake()
        {
            base.Awake();
            _rigidbody = GetComponent<Rigidbody>();
            Renderers = GetComponentsInChildren<Renderer>();

            if (interactionCanvas != null)
                interactionCanvas.gameObject.SetActive(false);

            var player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                var cam = player.GetComponentInChildren<Camera>();
                if (cam != null)
                    _playerCamera = cam.transform;
            }
        }

        protected void Start()
        {
            _originalLayer = gameObject.layer;
            InitializeHeldVersion();
        }

        private void Update()
        {
            if (!interactionCanvas ||
                !interactionCanvas.gameObject.activeSelf ||
                !_playerCamera) return;
            interactionCanvas.transform.LookAt(_playerCamera);
            interactionCanvas.transform.Rotate(0f, 180f, 0f);
        }

        public override void Interact(IInteractor interactor)
        {
            if (interactor is PlayerInteractionController { IsTableMode: true })
                return;

            PickUp(interactor);

            if (interactionCanvas != null)
                interactionCanvas.gameObject.SetActive(false);
        }
        
        private void PickUp(IInteractor interactor)
        {
            if (_heldVersion) SetHeldVisual(true, _heldVersion);
            if (TryGetComponent<PickUpSound>(out var sound)) sound.PlayPickUpSound();
            if (_currentHolder != null)
            {
                _currentHolder.ClearHeldObject(this);
                _currentHolder = null;
            }
            _holder = interactor;
            interactor.HeldObject?.Drop();
            interactor.SetHeldObject(this);
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

        public void Place(Vector3 position, Quaternion? rotation = null, ObjectHolder holder = null)
        {
            _currentHolder = holder;

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

        public override void OnHoverEnter(IInteractor interactor)
        {
            base.OnHoverEnter(interactor);

            var canInteract = _holder == null;

            if (interactionCanvas)
                interactionCanvas.gameObject.SetActive(canInteract);
        }

        public override void OnHoverExit(IInteractor interactor)
        {
            base.OnHoverExit(interactor);

            if (interactionCanvas)
                interactionCanvas.gameObject.SetActive(false);
        }

        public override string InteractionText(IInteractor interactor)
        {
            return string.Empty;
        }
    }
}
