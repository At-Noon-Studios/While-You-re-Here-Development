using Interactable;
using Interactable.Concrete.ObjectHolder;
using Interactable.Holdable;
using UnityEngine;
using UnityEngine.InputSystem;

namespace making_tea
{
    public abstract class TablePickup : InteractableBehaviour, ITablePickup
    {
        [Header("Table Settings")]
        [SerializeField] private float liftHeight = 0.8f;

        [Header("Rail Drag")]
        [SerializeField] protected bool enableRailDrag = false;
        [SerializeField] protected Transform railLeft;
        [SerializeField] protected Transform railRight;
        [SerializeField] protected float railSensitivity = 0.005f;

        protected bool IsLifted;
        protected PlayerInteractionController Pic;

        private Rigidbody _rb;

        private float _railT;
        private bool _railInitialized;
        private float _liftedY;

        private Transform[] _allTransforms;
        private int[] _originalLayers;

        private Collider[] _allColliders;
        private bool[] _originalColliderEnabled;
        private bool[] _originalColliderIsTrigger;

        private int _ignoreRaycastLayer;

        protected override void Awake()
        {
            base.Awake();
            _rb = GetComponent<Rigidbody>();

            _ignoreRaycastLayer = LayerMask.NameToLayer("Ignore Raycast");
            if (_ignoreRaycastLayer < 0) _ignoreRaycastLayer = 2;

            CacheOriginalStates();

            var player = GameObject.FindWithTag("Player");
            if (player == null) return;
            
            player.GetComponent<PlayerInteractionController>()?.RegisterTablePickup(this);
        }

        private void CacheOriginalStates()
        {
            _allTransforms = GetComponentsInChildren<Transform>(true);
            _originalLayers = new int[_allTransforms.Length];
            for (var i = 0; i < _allTransforms.Length; i++)
                _originalLayers[i] = _allTransforms[i].gameObject.layer;

            _allColliders = GetComponentsInChildren<Collider>(true);
            _originalColliderEnabled = new bool[_allColliders.Length];
            _originalColliderIsTrigger = new bool[_allColliders.Length];

            for (var i = 0; i < _allColliders.Length; i++)
            {
                _originalColliderEnabled[i] = _allColliders[i].enabled;
                _originalColliderIsTrigger[i] = _allColliders[i].isTrigger;
            }
        }

        private void SetHeldPhysicsState(bool held)
        {
            for (var i = 0; i < _allTransforms.Length; i++)
                _allTransforms[i].gameObject.layer = held ? _ignoreRaycastLayer : _originalLayers[i];

            for (var i = 0; i < _allColliders.Length; i++)
            {
                _allColliders[i].enabled = held || _originalColliderEnabled[i];
                _allColliders[i].isTrigger = held || _originalColliderIsTrigger[i];
            }
        }

        protected virtual void Update()
        {
            HandleRailDrag();
        }
        

        private float ComputeRailTFromCurrentPosition()
        {
            if (railLeft == null || railRight == null) return 0.5f;

            var a = railLeft.position;
            var b = railRight.position;
            var ab = b - a;

            var lenSq = Vector3.Dot(ab, ab);
            if (lenSq < 0.0001f) return 0.5f;

            var t = Vector3.Dot(transform.position - a, ab) / lenSq;
            return Mathf.Clamp01(t);
        }

        private void HandleRailDrag()
        {
            if (!enableRailDrag) return;
            if (!IsLifted || Pic == null || !Pic.IsTableMode) { _railInitialized = false; return; }

            if (Mouse.current == null) return;
            if (railLeft == null || railRight == null) return;

            if (!_railInitialized)
            {
                _railT = ComputeRailTFromCurrentPosition();
                var start = Vector3.Lerp(railLeft.position, railRight.position, _railT);
                start.y = _liftedY;
                transform.position = start;
                _railInitialized = true;
            }

            var dx = Mouse.current.delta.ReadValue().x;
            _railT = Mathf.Clamp01(_railT + dx * railSensitivity);

            var p = Vector3.Lerp(railLeft.position, railRight.position, _railT);
            p.y = _liftedY;
            transform.position = p;
        }

        public bool IsTableHeld => IsLifted;

        public virtual void Pickup(PlayerInteractionController p)
        {
            var holder = GetComponentInParent<ObjectHolderSingle>();
            if (holder != null)
            {
                var holdable = GetComponent<IHoldableObject>() ?? GetComponentInChildren<IHoldableObject>();
                if (holdable != null)
                {
                    holder.ClearHeldObject(holdable);
                    transform.SetParent(null, true);
                }
            }

            IsLifted = true;
            Pic = p;

            if (_rb != null)
            {
                _rb.isKinematic = true;
                _rb.linearVelocity = Vector3.zero;
                _rb.angularVelocity = Vector3.zero;
            }

            SetHeldPhysicsState(true);
            EnableCollider(false);

            transform.position += Vector3.up * liftHeight;
            _liftedY = transform.position.y;
        }

        public virtual void Drop()
        {
            IsLifted = false;
            Pic = null;
            _railInitialized = false;

            SetHeldPhysicsState(false);

            if (_rb != null)
            {
                _rb.isKinematic = false;
                _rb.linearVelocity = Vector3.zero;
                _rb.angularVelocity = Vector3.zero;
            }

            EnableCollider(true);
        }

        public virtual void ForceDropFromTableMode()
        {
            Drop();
        }

        public override string InteractionText(IInteractor interactor) => string.Empty;

        public bool InteractableBy(IInteractor interactor)
        {
            return (interactor as PlayerInteractionController)?.IsTableMode ?? false;
        }

        public override void Interact(IInteractor interactor)
        {
            if (interactor is not PlayerInteractionController { IsTableMode: true } p) return;

            if (!IsLifted) Pickup(p);
            else Drop();
        }

        public new abstract void EnableCollider(bool state);
    }
}
