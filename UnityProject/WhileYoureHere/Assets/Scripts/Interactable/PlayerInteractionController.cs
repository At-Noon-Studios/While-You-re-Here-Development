using System.Collections.Generic;
using System.Linq;
using chopping_logs;
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
        [SerializeField] private EventChannel dropEvent;
        [SerializeField] private EventChannel standUpEvent;

        [Header("Holding")]
        [SerializeField] private Transform holdPoint;

        [Header("Table Selection Filter")]
        [SerializeField] private float tableSelectionMaxDistance = 2.5f;
        [SerializeField] private float tableSelectionScreenMargin = 80f;

        [CanBeNull] private IInteractable _currentTarget;
        private UIManager _uiManager;
        private MovementController _movementController;
        private ChairInteractable _sittingChair;

        public bool IsTableMode { get; private set; }
        public ITablePickup CurrentTableSelection { get; private set; }

        public Camera PlayerCamera => playerCamera;

        private const int InteractableRaycastAllocation = 16;
        
        private readonly List<ITablePickup> _tablePickupsAll = new();
        private readonly List<ITablePickup> _tablePickupsSelectable = new();
        private int _tablePickupIndex;

        #region Unity

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
            if (IsTableMode)
            {
                if (IsAnyTablePickupHeld())
                {
                    RefreshCurrentTarget();
                }
                else
                {
                    if (_currentTarget != null)
                        SetCurrentTarget(null);

                    RebuildSelectableTablePickups(true);
                    HandleTableSelection();
                }
            }
            else
            {
                RefreshCurrentTarget();
            }
        }

        private void OnEnable()
        {
            interact.OnRaise += Interact;
            clickInteractEvent.OnRaise += ClickInteract;
            dropEvent.OnRaise += DropObject;
            standUpEvent.OnRaise += StandUp;
        }

        private void OnDisable()
        {
            interact.OnRaise -= Interact;
            clickInteractEvent.OnRaise -= ClickInteract;
            dropEvent.OnRaise -= DropObject;
            standUpEvent.OnRaise -= StandUp;
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
        
        #region Tablemode
        
                public void RegisterTablePickup(ITablePickup pickup)
        {
            if (pickup == null) return;
            if (!_tablePickupsAll.Contains(pickup))
                _tablePickupsAll.Add(pickup);
        }

        private static bool IsUnityDestroyed(ITablePickup p)
        {
            return p is Object uo && uo == null;
        }

        private void CleanupDestroyedTablePickups()
        {
            for (var i = _tablePickupsAll.Count - 1; i >= 0; i--)
            {
                var p = _tablePickupsAll[i];
                if (p == null || IsUnityDestroyed(p))
                    _tablePickupsAll.RemoveAt(i);
            }

            for (var i = _tablePickupsSelectable.Count - 1; i >= 0; i--)
            {
                var p = _tablePickupsSelectable[i];
                if (p == null || IsUnityDestroyed(p))
                    _tablePickupsSelectable.RemoveAt(i);
            }

            _tablePickupIndex = _tablePickupsSelectable.Count == 0 ? 0 : Mathf.Clamp(_tablePickupIndex, 0, _tablePickupsSelectable.Count - 1);
        }

        public void UnregisterTablePickup(ITablePickup pickup)
        {
            if (pickup == null) return;

            bool wasSelected =
                IsTableMode &&
                _tablePickupsSelectable.Count > 0 &&
                _tablePickupIndex >= 0 &&
                _tablePickupIndex < _tablePickupsSelectable.Count &&
                ReferenceEquals(_tablePickupsSelectable[_tablePickupIndex], pickup);

            for (int i = _tablePickupsAll.Count - 1; i >= 0; i--)
                if (ReferenceEquals(_tablePickupsAll[i], pickup))
                    _tablePickupsAll.RemoveAt(i);

            for (int i = _tablePickupsSelectable.Count - 1; i >= 0; i--)
                if (ReferenceEquals(_tablePickupsSelectable[i], pickup))
                    _tablePickupsSelectable.RemoveAt(i);

            if (!IsTableMode) return;
            if (IsAnyTablePickupHeld()) return;

            if (_tablePickupsSelectable.Count == 0)
            {
                _tablePickupIndex = 0;
                return;
            }

            if (_tablePickupIndex >= _tablePickupsSelectable.Count)
                _tablePickupIndex = _tablePickupsSelectable.Count - 1;

            if (wasSelected)
            {
                ClearTableSelection();
                HighlightCurrentTablePickup();
            }
        }

        private bool IsAnyTablePickupHeld()
        {
            CleanupDestroyedTablePickups();

            return _tablePickupsAll.Where(p => p != null && !IsUnityDestroyed(p)).Any(p => p.IsTableHeld);
        }

        public bool HasAnyTablePickupHeld() => IsAnyTablePickupHeld();

        public TablePickup GetHeldTablePickup()
        {
            CleanupDestroyedTablePickups();

            foreach (var p in _tablePickupsAll.Where(p => p != null && !IsUnityDestroyed(p)))
            {
                if (p is TablePickup tp && tp != null && tp.IsTableHeld)
                    return tp;
            }

            return null;
        }
        
        private void HandleTableSelection()
        {
            if (_tablePickupsSelectable.Count == 0)
                return;

            if (Keyboard.current == null)
                return;

            if (Keyboard.current.aKey.wasPressedThisFrame)
                ChangeTableSelection(-1);

            if (Keyboard.current.dKey.wasPressedThisFrame)
                ChangeTableSelection(1);
        }

        private void ChangeTableSelection(int dir)
        {
            RebuildSelectableTablePickups(true);

            ClearTableSelection();

            if (_tablePickupsSelectable.Count == 0)
                return;

            _tablePickupIndex =
                (_tablePickupIndex + dir + _tablePickupsSelectable.Count) % _tablePickupsSelectable.Count;

            HighlightCurrentTablePickup();
        }

        private void HighlightCurrentTablePickup()
        {
            if (_tablePickupsSelectable.Count == 0) return;

            var selected = _tablePickupsSelectable[_tablePickupIndex];
            CurrentTableSelection = selected;

            if (selected is InteractableBehaviour ib)
                ib.OnHoverEnter(this);
        }

        private void ClearTableSelection()
        {
            foreach (var t in _tablePickupsSelectable)
                if (t is InteractableBehaviour ib)
                    ib.OnHoverExit(this);

            CurrentTableSelection = null;
        }

        private void RebuildSelectableTablePickups(bool preserveSelection)
        {
            CleanupDestroyedTablePickups();

            ITablePickup previous = null;
            if (preserveSelection &&
                _tablePickupsSelectable.Count > 0 &&
                _tablePickupIndex >= 0 &&
                _tablePickupIndex < _tablePickupsSelectable.Count)
            {
                previous = _tablePickupsSelectable[_tablePickupIndex];
                if (previous == null || IsUnityDestroyed(previous))
                    previous = null;
            }

            _tablePickupsSelectable.Clear();
            
            foreach (var p in _tablePickupsAll)
            {
                if (p == null || IsUnityDestroyed(p)) continue;
                if (!IsSelectableTablePickup(p)) continue;
                _tablePickupsSelectable.Add(p);
            }

            if (_tablePickupsSelectable.Count is 0 or 1)
            {
                _tablePickupIndex = 0;
                return;
            }

            _tablePickupsSelectable.Sort((a, b) => GetPickupScreenX(a).CompareTo(GetPickupScreenX(b)));

            if (previous != null)
            {
                int idx = _tablePickupsSelectable.IndexOf(previous);
                _tablePickupIndex = idx >= 0 ? idx : FindClosestToScreenCenterIndex();
            }
            else
            {
                _tablePickupIndex = FindClosestToScreenCenterIndex();
            }

            _tablePickupIndex = Mathf.Clamp(_tablePickupIndex, 0, _tablePickupsSelectable.Count - 1);
        }

        private bool IsSelectableTablePickup(ITablePickup pickup)
        {
            if (pickup == null || IsUnityDestroyed(pickup)) return false;
            if (pickup is not MonoBehaviour mb || mb == null) return false;
            if (playerCamera == null) return false;

            var anchor = _sittingChair != null ? _sittingChair.transform.position : transform.position;
            if (tableSelectionMaxDistance > 0f && Vector3.Distance(mb.transform.position, anchor) > tableSelectionMaxDistance)
                return false;

            var sp = playerCamera.WorldToScreenPoint(mb.transform.position);
            if (sp.z < 0.01f) return false;

            float m = Mathf.Max(0f, tableSelectionScreenMargin);
            if (sp.x < -m || sp.x > Screen.width + m) return false;
            if (sp.y < -m || sp.y > Screen.height + m) return false;

            return true;
        }

        private float GetPickupScreenX(ITablePickup pickup)
        {
            if (pickup == null || IsUnityDestroyed(pickup) || playerCamera == null)
                return float.MaxValue;

            if (pickup is not MonoBehaviour mb || mb == null)
                return float.MaxValue;

            var sp = playerCamera.WorldToScreenPoint(mb.transform.position);
            if (sp.z < 0.01f)
                return float.MaxValue;

            return sp.x;
        }

        private int FindClosestToScreenCenterIndex()
        {
            if (_tablePickupsSelectable.Count == 0 || playerCamera == null)
                return 0;

            float center = Screen.width * 0.5f;
            int bestIndex = 0;
            float bestDist = float.MaxValue;

            for (int i = 0; i < _tablePickupsSelectable.Count; i++)
            {
                float x = GetPickupScreenX(_tablePickupsSelectable[i]);
                float d = Mathf.Abs(x - center);
                if (d < bestDist)
                {
                    bestDist = d;
                    bestIndex = i;
                }
            }

            return bestIndex;
        }
        
        public void EnableTableMode(bool enable)
        {
            IsTableMode = enable;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            if (enable)
            {
                RebuildSelectableTablePickups(false);
                ClearTableSelection();
                HighlightCurrentTablePickup();
            }
            else
            {
                ClearTableSelection();
                if (_currentTarget != null)
                    SetCurrentTarget(null);
            }
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
                if (IsAnyTablePickupHeld())
                {
                    if (ObjectHolderSingle.TryPlaceHeldTablePickup(this))
                        return;

                    if (TargetInteractable && _currentTarget is ObjectHolderSingle)
                    {
                        InteractWithTarget();
                        return;
                    }

                    GetHeldTablePickup()?.Drop();
                    return;
                }

                if (_tablePickupsSelectable.Count <= 0) return;
                var selected = _tablePickupsSelectable[_tablePickupIndex];
                selected?.Interact(this);
                return;
            }

            if (NoTarget)
            {
                DropObject();
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
            if (IsTableMode)
                return;

            if (NoTarget)
            {
                DropObject();
                return;
            }

            if (_currentTarget is IClickInteractable && clickInteractEvent.OnRaise != null)
            {
                ClickInteractWithTarget();
            }
            else
            {
                _uiManager?.PulseInteractPrompt();
            }
        }

        private void DropObject()
        {
            if (IsTableMode)
            {
                if (IsAnyTablePickupHeld())
                {
                    GetHeldTablePickup()?.Drop();
                    return;
                }

                if (_sittingChair != null)
                    _sittingChair.StandUp();

                return;
            }

            HeldObject?.Drop();
        }

        private void StandUp()
        {
            _sittingChair?.StandUp();
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
                if (HeldObject != null &&
                    hits[i].collider.TryGetComponent<IHoldableObject>(out _) &&
                    !hits[i].collider.TryGetComponent<LogBasket>(out _))
                {
                    continue;
                }

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

                if (collider.TryGetComponent<ObjectHolderSingle>(out var objectHolderSingle) &&
                    objectHolderSingle.IsDetectableBy(this))
                {
                    interactable = objectHolderSingle;
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
            OnHoverEnter(newTarget);
        }

        private void OnHoverEnter(IInteractable target)
        {
            if (target == null) return;
            _uiManager.ShowInteractPrompt(target.InteractionText(this), target.IsInteractableBy(this));
            target.OnHoverEnter(this);
        }

        private void OnHoverExit(IInteractable target)
        {
            _uiManager.HideInteractPrompt();
            target?.OnHoverExit(this);
        }

        private bool NoTarget => _currentTarget == null;
        
        private bool TargetInteractable => _currentTarget != null && _currentTarget.IsInteractableBy(this);
        
        private void InteractWithTarget()
        {
            _currentTarget?.Interact(this);
            OnHoverEnter(_currentTarget);
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
