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
        [Header("Interaction UI")]
        [SerializeField] private Canvas interactionCanvas;

        [Header("Holdable Data")]
        [SerializeField] private HoldableObjectData data;

        private Rigidbody _rigidbody;
        private int _originalLayer;
        [CanBeNull] private IInteractor _holder;
        [CanBeNull] private GameObject _heldVersion;

        private Transform _playerCamera;

        public float Weight => data.Weight;
        
        public bool IsPlaced { get; private set; }

        private const int HoldLayer = 3;

        private GameObject player;

        protected override void Awake()
        {
            base.Awake();
            _rigidbody = GetComponent<Rigidbody>();
            Renderers = GetComponentsInChildren<Renderer>();

            if (interactionCanvas != null)
                interactionCanvas.gameObject.SetActive(false);

            player = GameObject.FindWithTag("Player");
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
            // var player = GameObject.FindWithTag("Player");
            var pic = player.GetComponent<PlayerInteractionController>();
            if (pic.HeldObject != null)
                return;
            
            PickUp(interactor);

            if (interactionCanvas != null)
                interactionCanvas.gameObject.SetActive(false);
        }
        
        private void PickUp(IInteractor interactor)
        {
            if (_heldVersion) SetHeldVisual(true, _heldVersion);
            if (TryGetComponent<PickUpSound>(out var sound)) sound.PlayPickUpSound();
            _holder = interactor;
            var heldObject = this;
            interactor.HeldObject?.Drop();
            interactor.SetHeldObject(heldObject);
            AttachTo(interactor);
            EnableCollider(false);
            IsPlaced = false;
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
            IsPlaced = false;
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
            IsPlaced = true;
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

            bool canInteract = _holder == null;

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
