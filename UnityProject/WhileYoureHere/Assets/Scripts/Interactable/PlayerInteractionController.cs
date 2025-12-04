using Interactable.Concrete.ObjectHolder;
using Interactable.Holdable;
using JetBrains.Annotations;
using making_tea;
using player_controls;
using ScriptableObjects.Events;
using ScriptableObjects.Interactable;
using UI;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Interactable
{
    public class PlayerInteractionController : MonoBehaviour, IInteractor
    {
        [SerializeField] private PlayerInteractionData data;
        [SerializeField] private Camera playerCamera;
        [Header("Listen to")]
        [SerializeField] private EventChannel interact;
        [SerializeField] private Transform holdPoint;
        
        [CanBeNull] private IInteractable _currentTarget;
        private UIManager _uiManager;
        private MovementController _movementController;

        private ChairInteractable _sittingChair;
        
        public bool TableMode { get; private set; }
        public Camera PlayerCamera => GetComponentInChildren<Camera>();
        
        private const int InteractableRaycastAllocation = 16;

        #region Unity event functions

        private void Awake()
        {
            _movementController = GetComponent<MovementController>();
        }
        
        private void Start()
        {
            _uiManager = UIManager.Instance;
        }

        private void Update()
        {
            RefreshCurrentTarget();
        }

        private void OnEnable()
        {
            interact.OnRaise += Interact;
        }

        private void OnDisable()
        {
            interact.OnRaise -= Interact;
        }
        
        #endregion
        
        #region Interface implementation
        
        public Transform HoldPoint => holdPoint;
        
        [CanBeNull] public IHoldableObject HeldObject { get; private set; }
        
        public void SetHeldObject([CanBeNull] IHoldableObject holdableObject)
        {
            HeldObject = holdableObject;
            UpdateMovementSpeed(holdableObject);
        }
        
        #endregion
        
        #region Public chair helpers (vom Chair benutzt)

        public void SetSittingChair(ChairInteractable chair)
        {
            _sittingChair = chair;
        }

        public void ClearSittingChair()
        {
            _sittingChair = null;
        }

        #endregion

        #region Private methods
        
        private void Interact()
        {
            if (TableMode)
            {
                if (HeldObject == null && TryDropTablePickup())
                    return;

                if (_currentTarget != null)
                {
                    _currentTarget.Interact(this);
                    return;
                }

                if (_sittingChair == null) return;
                _sittingChair.ForceStandUp();
           
            }
            
            if (NoTarget) HeldObject?.Drop();
            else if (TargetInteractable) InteractWithTarget();
            else _uiManager.PulseInteractPrompt();
        }

        private void RefreshCurrentTarget()
        {
            var hits = new RaycastHit[InteractableRaycastAllocation];
            var hitCount = LookForHits(hits);
            IInteractable bestTarget = null;
            var closestDistance = float.MaxValue;
            for (var i = 0; i < hitCount; i++)
            {
                UpdateBestTarget(hits[i], ref closestDistance, ref bestTarget, TableMode);
            }

            if (bestTarget == _currentTarget) return;
            SetCurrentTarget(bestTarget);
        }
        
        private int LookForHits(RaycastHit[] result)
        {
            var ray = playerCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            return Physics.SphereCastNonAlloc(ray, data.InteractionAssistRadius, result, data.InteractionReach);
        }

        private static void UpdateBestTarget(
            RaycastHit candidate,
            ref float closestDistance,
            ref IInteractable bestTarget,
            bool tableMode)
        {
            if (candidate.distance >= closestDistance) return;

            var col = candidate.collider;

            switch (tableMode)
            {
                case true when col.TryGetComponent<ITablePickup>(out var tablePickup):
                    bestTarget = tablePickup;
                    closestDistance = candidate.distance;
                    return;
                case false when col.TryGetComponent<Placeable>(out var placeable):
                    bestTarget = placeable;
                    closestDistance = candidate.distance;
                    return;
            }

            if (!col.TryGetComponent<IInteractable>(out var interactable)) return;
            bestTarget = interactable;
            closestDistance = candidate.distance;
        }

        private void SetCurrentTarget(IInteractable newTarget)
        {
            OnHoverExit(_currentTarget);
            _currentTarget = newTarget;
            OnHoverEnter(newTarget);
        }

        private void OnHoverEnter(IInteractable target)
        {
            if (target == null) return;
            _uiManager.ShowInteractPrompt(target.InteractionText(this), target.InteractableBy(this));
            target.OnHoverEnter(this);
        }

        private void OnHoverExit(IInteractable target)
        {
            _uiManager.HideInteractPrompt();
            target?.OnHoverExit(this);
        }
        
        private bool NoTarget => _currentTarget == null;
        
        private bool TargetInteractable => _currentTarget != null && _currentTarget.InteractableBy(this);
        
        private void InteractWithTarget()
        {
            _currentTarget?.Interact(this);
            OnHoverEnter(_currentTarget);
        }
        
        private void UpdateMovementSpeed([CanBeNull] IHoldableObject holdableObject)
        {
            if (_movementController == null) return;
            if (holdableObject == null)
            {
                _movementController.SetMovementModifier(1f);
                return;
            }
            var weight = Mathf.Clamp01(holdableObject.Weight / 100f);
            var modifier = Mathf.Max(1f - weight, 0.4f);
            _movementController.SetMovementModifier(modifier);
        }
        
        public void EnableTableMode(bool enable)
        {
            TableMode = enable;

            if (enable)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
        
        private static bool TryDropTablePickup()
        {
            var pickups = FindObjectsByType<TablePickup>(FindObjectsSortMode.None);

            foreach (var p in pickups)
            {
                if (!p.IsTableHeld) continue;
                p.Drop();
                return true;
            }
            return false;
        }
        
        #endregion
    }
}
