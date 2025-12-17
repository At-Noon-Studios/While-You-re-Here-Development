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
    [DisallowMultipleComponent]
    public class PlayerInteractionController : MonoBehaviour, IInteractor
    {
        [Header("Interaction Settings")]
        [SerializeField] private PlayerInteractionData data;

        [Header("Camera")]
        [SerializeField] private Camera playerCamera;

        [Header("Input Events")]
        [SerializeField] private EventChannel interact;
        [SerializeField] private EventChannel clickInteractEvent;

        [Header("Holding")]
        [SerializeField] private Transform holdPoint;

        [CanBeNull] private IInteractable _currentTarget;
        private UIManager _uiManager;
        private MovementController _movementController;
        private ChairInteractable _sittingChair;

        public bool IsTableMode { get; private set; }
        public Camera PlayerCamera => playerCamera;

        private const int InteractableRaycastAllocation = 16;

        #region Unity

        private void Awake()
        {
            _movementController = GetComponent<MovementController>();
        }

        private void Start()
        {
            if (_uiManager == null)
                _uiManager = UIManager.Instance ?? FindObjectOfType<UIManager>();
        }

        private void Update()
        {
            RefreshCurrentTarget();
        }

        private void OnEnable()
        {
            if (interact != null) interact.OnRaise += Interact;
            if (clickInteractEvent != null) clickInteractEvent.OnRaise += ClickInteract;
        }

        private void OnDisable()
        {
            if (interact != null) interact.OnRaise -= Interact;
            if (clickInteractEvent != null) clickInteractEvent.OnRaise -= ClickInteract;
        }

        #endregion

        #region Interface - Holding

        public Transform HoldPoint => holdPoint;

        [CanBeNull] public IHoldableObject HeldObject { get; private set; }

        public void SetHeldObject([CanBeNull] IHoldableObject holdableObject)
        {
            HeldObject = holdableObject;
            UpdateMovementSpeed(holdableObject);
        }

        #endregion

        #region Chair helpers

        public void SetSittingChair(ChairInteractable chair) => _sittingChair = chair;

        public void ClearSittingChair() => _sittingChair = null;

        #endregion

        #region Interaction Core

        private void Interact()
        {
            if (IsTableMode)
            {
                if (HeldObject == null &&
                    _currentTarget is not ObjectHolder &&
                    CanDropTablePickup())
                    return;

                if (NoTarget)
                {
                    if (_sittingChair != null)
                        _sittingChair.ForceStandUp();

                    return;
                }

                if (TargetInteractable)
                {
                    InteractWithTarget();
                }
                else
                {
                    _uiManager?.PulseInteractPrompt();
                }

                return;
            }

            if (NoTarget)
            {
                HeldObject?.Drop();
                return;
            }

            if (TargetInteractable)
            {
                InteractWithTarget();
            }
            else
            {
                _uiManager?.PulseInteractPrompt();
            }
        }


        private void ClickInteract()
        {
            if (NoTarget)
            {
                HeldObject?.Drop();
                return;
            }

            if (_currentTarget is IClickInteractable)
            {
                ClickInteractWithTarget();
            }
            else
            {
                _uiManager?.PulseInteractPrompt();
            }
        }


        #endregion

        #region Target detection

        private void RefreshCurrentTarget()
        {
            var hits = new RaycastHit[InteractableRaycastAllocation];
            var hitCount = LookForHits(hits);

            IInteractable bestTarget = null;
            var closestDistance = float.MaxValue;

            for (var i = 0; i < hitCount; i++)
            {
                if (hits[i].collider.TryGetComponent<IHoldableObject>(out _) &&
                    HeldObject != null)
                    continue;

                UpdateBestTarget(hits[i], ref closestDistance, ref bestTarget, IsTableMode);
            }

            if (bestTarget == _currentTarget) return;

            SetCurrentTarget(bestTarget);
        }

        private int LookForHits(RaycastHit[] result)
        {
            if (playerCamera == null || data == null)
                return 0;

            Ray ray = playerCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            return Physics.SphereCastNonAlloc(ray,
                data.InteractionAssistRadius,
                result,
                data.InteractionReach);
        }

        private void UpdateBestTarget(
            RaycastHit candidate,
            ref float closestDistance,
            ref IInteractable bestTarget,
            bool tableMode)
        {
            if (candidate.distance >= closestDistance)
                return;

            if (!TryGetBestInteractable(candidate.collider, tableMode, out var interactable))
                return;

            bestTarget = interactable;
            closestDistance = candidate.distance;
        }
        
        private bool TryGetBestInteractable(
            Collider collider,
            bool tableMode,
            out IInteractable interactable)
        {
            interactable = null;

            if (tableMode)
            {
                if (collider.TryGetComponent<ITablePickup>(out var tablePickup) &&
                    tablePickup.IsDetectableBy(this))
                {
                    interactable = tablePickup;
                    return true;
                }

                if (collider.TryGetComponent<ObjectHolder>(out var objectHolder) &&
                    objectHolder.IsDetectableBy(this))
                {
                    interactable = objectHolder;
                    return true;
                }
            }
            else
            {
                if (collider.TryGetComponent<Placeable>(out var placeable) &&
                    placeable.IsDetectableBy(this))
                {
                    interactable = placeable;
                    return true;
                }
            }

            if (collider.TryGetComponent<IInteractable>(out var fallback) &&
                fallback.IsDetectableBy(this))
            {
                interactable = fallback;
                return true;
            }

            return false;
        }


        private void SetCurrentTarget(IInteractable newTarget)
        {
            OnHoverExit(_currentTarget);
            _currentTarget = newTarget;
            OnHoverEnter(_currentTarget);
        }

        private void OnHoverEnter(IInteractable target)
        {
            if (target == null) return;
            _uiManager.ShowInteractPrompt(target.InteractionText(this), target.IsInteractableBy(this));
            target.OnHoverEnter(this);
        }

        private void OnHoverExit(IInteractable target)
        {
            _uiManager?.HideInteractPrompt();
            target?.OnHoverExit(this);
        }

        private bool NoTarget => _currentTarget == null;
        
        private bool TargetInteractable => _currentTarget != null && _currentTarget.IsInteractableBy(this);
        
        private void InteractWithTarget()
        {
            _currentTarget?.Interact(this);
            OnHoverEnter(_currentTarget); // Refresh
        }

        private void ClickInteractWithTarget()
        {
            _currentTarget?.ClickInteract(this);
            OnHoverExit(_currentTarget);
        }

        private void UpdateMovementSpeed([CanBeNull] IHoldableObject holdableObject)
        {
            if (_movementController == null) return;

            if (holdableObject == null)
            {
                _movementController.SetMovementModifier(1f);
                return;
            }

            float weight = Mathf.Clamp01(holdableObject.Weight / 100f);
            float modifier = Mathf.Max(1f - weight, 0.4f);

            _movementController.SetMovementModifier(modifier);
        }
        
        #endregion

        #region Tablemode

        public void EnableTableMode(bool enable)
        {
            IsTableMode = enable;

            Cursor.lockState = enable ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = enable;
        }

        private static bool CanDropTablePickup()
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
