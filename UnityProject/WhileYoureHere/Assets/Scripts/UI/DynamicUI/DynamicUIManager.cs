using System.Collections.Generic;
using chopping_logs;
using door;
using Interactable;
using Interactable.Holdable;
using radio_interaction;
using UnityEngine;
using UnityEngine.UI;

namespace UI.DynamicUI
{
    public class DynamicUIManager : MonoBehaviour
    {
        public enum LookAtTarget { Camera }
        public enum ActivationMode { Always, InteractableHovered }
        public enum DoorState { None, Open, Closed, Locked }
        public enum StumpState { None, CutLog, PlaceLog, MouseUp, MouseDown, GuideLine }
        public enum KettleState { None, Pouring }

        public enum RadioState{ None,Off,On,Tuning }

        [System.Serializable]
        public class WorldSpaceUIElement
        {
            [Header("General")]
            [SerializeField] private string elementName;
            [SerializeField] private Sprite sprite;
            [SerializeField] private Vector2 size = new Vector2(64, 64);

            [Header("Flip Options")]
            public bool flipX;
            public bool flipY;

            [Header("Activation")]
            [SerializeField] private ActivationMode activationMode = ActivationMode.Always;
            public InteractableBehaviour interactableBehaviour;

            [SerializeField] private HoldableObjectBehaviour requiredHeldObject;

            [Header("Required States")]
            [SerializeField] private DoorState requiredDoorState = DoorState.None;
            [SerializeField] private StumpState requiredStumpState = StumpState.None;
            [SerializeField] private KettleState requiredKettleState = KettleState.None;
            [SerializeField] private RadioState requiredRadioState = RadioState.None;

            [Header("Offset")]
            [SerializeField] private Vector3 offset = Vector3.up;

            [Header("Hide After Timer")] [SerializeField]
            private float hideAfterTimer = 0f;

            [Header("Look At")]
            [SerializeField] private LookAtTarget lookAtTarget = LookAtTarget.Camera;

            [HideInInspector] public GameObject uiObject;
            [HideInInspector] public Image image;
            [HideInInspector] public RectTransform rectTransform;
            [HideInInspector] public bool previouslyActive;

            public Vector3 Offset => offset;
            public Sprite Sprite => sprite;
            public Vector2 Size => size;
            public LookAtTarget LookTarget => lookAtTarget;

            public bool IsActive => CheckIsActive();
            public float ElementHideTimer => hideAfterTimer;
            private float activeTimer { get; set; }
            public float ActiveTimer
            {
                get => activeTimer;
                set => activeTimer = value;
            }

            private bool CheckIsActive()
            {
                if (activationMode == ActivationMode.InteractableHovered &&
                    (interactableBehaviour == null ||
                     !interactableBehaviour.IsHovered ||
                     interactableBehaviour.blockInteraction))
                    return false;

                if (interactableBehaviour == null)
                    return false;

                if (interactableBehaviour is DoorInteractable door &&
                    requiredDoorState != DoorState.None)
                    return CheckDoorState(door);

                if(interactableBehaviour is RadioPowerInteraction radioPowerInteraction &&
                   requiredRadioState != RadioState.None)
                    return CheckRadioState(radioPowerInteraction);

                if (interactableBehaviour is Stump stump &&
                    requiredStumpState != StumpState.None)
                    return CheckStumpState(stump);

                if (requiredHeldObject != null)
                {
                    var held = GameObject.FindWithTag("Player")
                        ?.GetComponent<PlayerInteractionController>()?.HeldObject;

                    if (held == null)
                        return false;

                    if (held.GetType() != requiredHeldObject.GetType())
                        return false;
                }

                return true;
            }

            private bool CheckKettleState()
            {
                var kettlePour = GameObject.FindObjectOfType<making_tea.KettlePour>();
                if (kettlePour == null || kettlePour.kettle == null)
                    return false;

                if (requiredKettleState == KettleState.Pouring)
                {
                    bool isFilled = kettlePour.kettle.fillAmount > 0f;
                    bool isHeld =
                        (kettlePour.TryGetComponent<HoldableObjectBehaviour>(out var h) && h.IsCurrentlyHeld) ||
                        (kettlePour.TryGetComponent<making_tea.KettleTablePickup>(out var t) && t.IsTableHeld);

                    return isFilled && isHeld;
                }

                return false;
            }

            private bool CheckRadioState(
                RadioPowerInteraction radioPowerInteraction)
            {
                var radioControllerState = radioPowerInteraction.GetRadioController().RadioStateMachine.CurrentState;
                var currentState= RadioState.None;
                Debug.Log("radioController state = "+radioControllerState);
                if(requiredRadioState == RadioState.None) return false;

                switch (radioControllerState)
                {
                    case RadioOnState:
                        currentState = RadioState.On;
                        Debug.Log("requiredRadioState = " + requiredRadioState);
                        break;
                    case RadioOffState:
                        currentState = RadioState.Off;
                        Debug.Log("requiredRadioState = " + requiredRadioState);
                        break;
                    case TuningState:
                        currentState = RadioState.Tuning;
                        Debug.Log("requiredRadioState = " + requiredRadioState);
                        break;
                }

                Debug.Log("requiredRadioState = " + requiredRadioState);
                return currentState == requiredRadioState;
            }

            private bool CheckDoorState(DoorInteractable door)
            {
                if (door.isLocked) return requiredDoorState == DoorState.Locked;
                if (door.isOpen) return requiredDoorState == DoorState.Closed;
                return requiredDoorState == DoorState.Open;
            }

            private bool CheckStumpState(Stump stump)
            {
                if (stump == null) return false;

                if (!stump.IsMinigameActive)
                {
                    var held = GameObject.FindWithTag("Player")
                        ?.GetComponent<PlayerInteractionController>()?.HeldObject;

                    if (requiredStumpState == StumpState.CutLog &&
                        held is HoldableObjectBehaviour h &&
                        h.GetComponentInChildren<AxeHitDetector>() != null &&
                        stump.HasLog)
                        return true;

                    if (requiredStumpState == StumpState.PlaceLog &&
                        held is HoldableObjectBehaviour log &&
                        log.CompareTag("Log"))
                        return true;

                    return false;
                }

                if (requiredStumpState == StumpState.GuideLine)
                    return true;

                if (requiredStumpState == StumpState.MouseUp && ChopUIManager.IsAxeDown)
                    return true;

                if (requiredStumpState == StumpState.MouseDown && !ChopUIManager.IsAxeDown)
                    return true;

                return false;
            }
        }


        [Header("World Space UI Settings")]
        [SerializeField] private List<WorldSpaceUIElement> worldSpaceElements;
        [SerializeField] private float canvasScale = 0.01f;

        [Header("Stacking Settings")]
        [Tooltip("If true, multiple UI elements can be active on the same interactable")]
        [SerializeField] private bool allowMultipleOnSameInteractable = false;

        private Camera mainCamera;
        private Canvas worldCanvas;
        private Transform playerTransform;

        private void Awake()
        {
            mainCamera = Camera.main;
            playerTransform = GameObject.FindWithTag("Player")?.transform;
            CreateWorldCanvas();
            InitializeElements();
        }

        private void LateUpdate()
        {
            UpdateElements();
            UpdateLookDirections();
            UpdateCanvasScale();
        }

        private void CreateWorldCanvas()
        {
            var canvasObj = new GameObject("WorldSpaceCanvas");
            canvasObj.transform.SetParent(transform);
            canvasObj.transform.localPosition = Vector3.zero;
            canvasObj.transform.localRotation = Quaternion.identity;

            worldCanvas = canvasObj.AddComponent<Canvas>();
            worldCanvas.renderMode = RenderMode.WorldSpace;
            worldCanvas.worldCamera = mainCamera;

            var scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.dynamicPixelsPerUnit = 10;

            canvasObj.AddComponent<GraphicRaycaster>();
            canvasObj.transform.localScale = Vector3.one * canvasScale;
        }

        private void InitializeElements()
        {
            foreach (var element in worldSpaceElements)
                SetupUIElement(element);
        }

        private void SetupUIElement(WorldSpaceUIElement element)
        {
            var uiObj = new GameObject(element.GetHashCode().ToString());
            uiObj.transform.SetParent(worldCanvas.transform);
            uiObj.transform.localPosition = Vector3.zero;
            uiObj.transform.localRotation = Quaternion.identity;

            var img = uiObj.AddComponent<Image>();
            img.sprite = element.Sprite;

            var rect = img.rectTransform;
            rect.sizeDelta = element.Size;
            rect.pivot = Vector2.one * 0.5f;
            rect.localScale = new Vector3(
                element.flipX ? -1 : 1,
                element.flipY ? -1 : 1,
                1);

            element.uiObject = uiObj;
            element.image = img;
            element.rectTransform = rect;
        }

        private void UpdateElements()
        {
            Dictionary<InteractableBehaviour, bool> hasExclusiveActive =
                new Dictionary<InteractableBehaviour, bool>();

            foreach (var element in worldSpaceElements)
            {
                if (element.uiObject == null)
                    continue;

                bool active = false;
                var interactable = element.interactableBehaviour;

                bool isCurrentlyActive = element.IsActive;

                if (isCurrentlyActive && !element.previouslyActive)
                {
                    element.ActiveTimer = 0f;
                }

                element.previouslyActive = isCurrentlyActive;

                if (isCurrentlyActive)
                {
                    if (element.ElementHideTimer > 0f)
                    {
                        element.ActiveTimer += Time.deltaTime;
                    }
                }

                if (interactable != null)
                {
                    if (!hasExclusiveActive.ContainsKey(interactable))
                        hasExclusiveActive.Add(interactable, false);

                    if (isCurrentlyActive)
                    {
                        bool timerExpired = element.ElementHideTimer > 0f && element.ActiveTimer >= element.ElementHideTimer;

                        if (!timerExpired)
                        {
                            if (allowMultipleOnSameInteractable || !hasExclusiveActive[interactable])
                            {
                                active = true;

                                if (!allowMultipleOnSameInteractable)
                                {
                                    hasExclusiveActive[interactable] = true;
                                }
                            }
                        }
                    }
                }

                SetElementActive(element, active);

                if (active)
                {
                    UpdateUIElementSpriteAndSize(element);
                    UpdateUIElementPosition(element);
                }
            }
        }

        private void SetElementActive(WorldSpaceUIElement element, bool active)
        {
            element.uiObject.SetActive(active);
            element.image.enabled = active;
        }

        private void UpdateUIElementSpriteAndSize(WorldSpaceUIElement element)
        {
            element.image.sprite = element.Sprite;
            element.rectTransform.sizeDelta = element.Size;
        }

        private void UpdateUIElementPosition(WorldSpaceUIElement element)
        {
            Vector3 targetPos = element.Offset;

            if (element.interactableBehaviour != null)
                targetPos += element.interactableBehaviour.transform.position;

            if (element.interactableBehaviour is DoorInteractable)
                targetPos = CalculateDoorUIPosition(element);

            element.uiObject.transform.position = targetPos;
        }

        private Vector3 CalculateDoorUIPosition(WorldSpaceUIElement element)
        {
            Transform t = element.interactableBehaviour.transform;

            return t.position
                   + t.right * element.Offset.x
                   + t.up * element.Offset.y
                   + t.forward * element.Offset.z;
        }

        private void UpdateLookDirections()
        {
            if (mainCamera == null) return;

            foreach (var element in worldSpaceElements)
            {
                if (element.uiObject == null || !element.uiObject.activeSelf)
                    continue;

                if (element.LookTarget == LookAtTarget.Camera)
                {
                    Vector3 dir = element.uiObject.transform.position - mainCamera.transform.position;
                    if (dir.sqrMagnitude > 0.001f)
                        element.uiObject.transform.rotation = Quaternion.LookRotation(dir);
                }
            }
        }

        private void UpdateCanvasScale()
        {
            float scaleFactor = Screen.height / 1080f;
            worldCanvas.transform.localScale = Vector3.one * canvasScale * scaleFactor;
        }
    }
}
