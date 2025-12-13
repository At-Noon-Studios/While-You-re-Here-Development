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

            _uiManager = UIManager.Instance ?? FindObjectOfType<UIManager>();
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

                if (_currentTarget != null)
                {
                    if (_currentTarget.InteractableBy(this))
                        _currentTarget.Interact(this);
                    else
                        _uiManager?.PulseInteractPrompt();
                    return;
                }

                if (_sittingChair != null)
                    _sittingChair.ForceStandUp();

                return;
            }

            if (_currentTarget == null)
            {
                HeldObject?.Drop();
                return;
            }

            if (_currentTarget.InteractableBy(this))
            {
                _currentTarget.Interact(this);
                return;
            }

            _uiManager?.PulseInteractPrompt();
        }

        private void ClickInteract()
        {
            if (_currentTarget == null)
            {
                HeldObject?.Drop();
                return;
            }

            if (_currentTarget is IClickInteractable)
            {
                _currentTarget.ClickInteract(this);
                OnHoverExit(_currentTarget);
                return;
            }

            _uiManager?.PulseInteractPrompt();
        }

        #endregion

        #region Target detection

        private void RefreshCurrentTarget()
        {
            var hits = new RaycastHit[InteractableRaycastAllocation];
            int hitCount = LookForHits(hits);

            IInteractable bestTarget = null;
            float closest = float.MaxValue;

            for (int i = 0; i < hitCount; i++)
            {
                if (hits[i].collider.TryGetComponent<IHoldableObject>(out _) &&
                    HeldObject != null)
                    continue;

                UpdateBestTarget(hits[i], ref closest, ref bestTarget, IsTableMode);
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

        private static void UpdateBestTarget(
            RaycastHit hit,
            ref float closest,
            ref IInteractable best,
            bool tableMode)
        {
            if (hit.distance >= closest)
                return;

            var col = hit.collider;

            if (tableMode)
            {
                if (col.TryGetComponent<ITablePickup>(out var tp))
                {
                    best = tp;
                    closest = hit.distance;
                    return;
                }

                if (col.TryGetComponent<ObjectHolder>(out var oh))
                {
                    best = oh;
                    closest = hit.distance;
                    return;
                }
            }
            else
            {
                if (col.TryGetComponent<Placeable>(out var placeable))
                {
                    best = placeable;
                    closest = hit.distance;
                    return;
                }
            }

            if (col.TryGetComponent<IInteractable>(out var interactable))
            {
                best = interactable;
                closest = hit.distance;
            }
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

            _uiManager?.ShowInteractPrompt(target.InteractionText(this), target.InteractableBy(this));
            target.OnHoverEnter(this);
        }

        private void OnHoverExit(IInteractable target)
        {
            _uiManager?.HideInteractPrompt();
            target?.OnHoverExit(this);
        }

        #endregion

        #region Utility

        private void UpdateMovementSpeed([CanBeNull] IHoldableObject obj)
        {
            if (_movementController == null) return;

            if (obj == null)
            {
                _movementController.SetMovementModifier(1f);
                return;
            }

            float weight = Mathf.Clamp01(obj.Weight / 100f);
            float modifier = Mathf.Max(1f - weight, 0.4f);

            _movementController.SetMovementModifier(modifier);
        }

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
