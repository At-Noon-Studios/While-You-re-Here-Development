using Interactable.Holdable;
using JetBrains.Annotations;
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
        [Header("Listen to")] [SerializeField] private EventChannel interact;
        [SerializeField] private EventChannel clickInteractEvent;
        [SerializeField] private Transform holdPoint;

        [CanBeNull] private IInteractable _currentTarget;
        private UIManager _uiManager;
        private MovementController _movementController;

        private const int InteractableRaycastAllocation = 16;

        #region Unity event functions {

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
            clickInteractEvent.OnRaise += clickInteract;
        }

        private void OnDisable()
        {
            interact.OnRaise -= Interact;
            clickInteractEvent.OnRaise -= clickInteract;
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

        #region Private methods

        private void Interact()
        {
            if (NoTarget) HeldObject?.Drop();
            else if (TargetInteractable)
            {
                if (_currentTarget is not IEInteractable || interact.OnRaise == null) return;
                InteractWithTarget();
            }
            else _uiManager.PulseInteractPrompt(); // Target is interactable, but interaction is not allowed
        }

        private void clickInteract()
        {
            if (NoTarget) HeldObject?.Drop();
            else if (_currentTarget is IClickInteractable && clickInteractEvent.OnRaise != null)
            {
                ClickInteractWithTarget();
            }
            else _uiManager.PulseInteractPrompt(); // Target is interactable, but interaction is not allowed
        }

        private void RefreshCurrentTarget()
        {
            var hits = new RaycastHit[InteractableRaycastAllocation];
            var hitCount = LookForHits(hits);
            IInteractable bestTarget = null;
            var closestDistance = float.MaxValue;
            for (var i = 0; i < hitCount; i++)
            {
                if (hits[i].collider.TryGetComponent<IHoldableObject>(out var holdable) && HeldObject != null) break;
                UpdateBestTarget(hits[i], ref closestDistance, ref bestTarget);
            }

            if (bestTarget == _currentTarget) return;
            SetCurrentTarget(bestTarget);
        }

        private int LookForHits(RaycastHit[] result)
        {
            var ray = playerCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            return Physics.SphereCastNonAlloc(ray, data.InteractionAssistRadius, result, data.InteractionReach);
        }

        private static void UpdateBestTarget(RaycastHit candidate, ref float closestDistance,
            ref IInteractable bestTarget)
        {
            if (candidate.distance >= closestDistance ||
                !candidate.collider.TryGetComponent<IInteractable>(out var interactable)) return;
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
            if (target == null)
                return;

            if (_uiManager != null)
                _uiManager.ShowInteractPrompt(
                    target.InteractionText(this),
                    target.InteractableBy(this)
                );

            target.OnHoverEnter(this);
        }

        private void OnHoverExit(IInteractable target)
        {
            if (_uiManager != null)
                _uiManager.HideInteractPrompt();

            target?.OnHoverExit(this);
        }


        private bool NoTarget => _currentTarget == null;

        private bool TargetInteractable => _currentTarget != null && _currentTarget.InteractableBy(this);

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

            var weight = Mathf.Clamp01(holdableObject.Weight / 100f);
            var modifier = Mathf.Max(1f - weight, 0.4f);
            _movementController.SetMovementModifier(modifier);
        }

        #endregion
    }
}