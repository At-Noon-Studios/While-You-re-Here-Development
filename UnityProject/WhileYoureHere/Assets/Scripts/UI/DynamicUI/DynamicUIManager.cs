using System.Collections.Generic;
using chopping_logs;
using door;
using Interactable;
using Interactable.Holdable;
using UnityEngine;
using UnityEngine.UI;

namespace UI.DynamicUI
{
    public class DynamicUIManager : MonoBehaviour
    {
        public enum LookAtTarget { Camera }
        public enum ActivationMode { Always, InteractableHovered }
        public enum DoorState { None, Open, Closed, Locked }
        public enum StumpState { None, CutLog, PlaceLog }

        [System.Serializable]
        public class WorldSpaceUIElement
        {
            [Header("General")]
            [SerializeField] private string elementName;
            [SerializeField] private Sprite sprite;
            [SerializeField] private Vector2 size = new Vector2(64, 64);

            [Header("Flip Options")]
            [SerializeField] public bool flipX;
            [SerializeField] public bool flipY;

            [Header("Activation")]
            [SerializeField] private ActivationMode activationMode = ActivationMode.Always;
            [SerializeField] public InteractableBehaviour interactableBehaviour;

            [Header("Required States")]
            [SerializeField] private DoorState requiredDoorState = DoorState.None;
            [SerializeField] private StumpState requiredStumpState = StumpState.None;

            [Header("Offset")]
            [SerializeField] private Vector3 offset = Vector3.up;

            [Header("Look At")]
            [SerializeField] private LookAtTarget lookAtTarget = LookAtTarget.Camera;

            [HideInInspector] public GameObject uiObject;
            [HideInInspector] public Image image;
            [HideInInspector] public RectTransform rectTransform;

            public Vector3 Offset => offset;
            public Sprite Sprite => sprite;
            public Vector2 Size => size;
            public LookAtTarget LookTarget => lookAtTarget;

            public bool IsActive => CheckIsActive();

            private bool CheckIsActive()
            {
                if (activationMode == ActivationMode.InteractableHovered && 
                    (interactableBehaviour == null || !interactableBehaviour.IsHovered || interactableBehaviour.blockInteraction))
                    return false;

                if (interactableBehaviour == null) return false;

                if (interactableBehaviour is DoorInteractable door && requiredDoorState != DoorState.None)
                    return CheckDoorState(door);

                if (interactableBehaviour is Stump stump && requiredStumpState != StumpState.None)
                    return CheckStumpState(stump);

                return true;
            }

            private bool CheckDoorState(DoorInteractable door)
            {
                if (door.isLocked) return requiredDoorState == DoorState.Locked;
                if (door.isOpen) return requiredDoorState == DoorState.Closed;
                return requiredDoorState == DoorState.Open;
            }

            private bool CheckStumpState(Stump stump)
            {
                if (!interactableBehaviour.IsHovered) return false;
                return GetStumpState(stump) == requiredStumpState;
            }

            private StumpState GetStumpState(Stump stump)
            {
                if (stump == null) return StumpState.None;

                var held = GameObject.FindWithTag("Player")?
                    .GetComponent<PlayerInteractionController>()?.HeldObject;

                if (held is HoldableObjectBehaviour h && h.GetComponentInChildren<AxeHitDetector>() != null)
                    return StumpState.CutLog;

                if (held is HoldableObjectBehaviour pickableLog && pickableLog.CompareTag("Log"))
                    return StumpState.PlaceLog;

                return StumpState.None;
            }
        }

        [Header("World Space UI Settings")]
        [SerializeField] private List<WorldSpaceUIElement> worldSpaceElements;
        [SerializeField] private float canvasScale = 0.01f;

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
            canvasObj.transform.localRotation = Quaternion.identity;
            canvasObj.transform.localPosition = Vector3.zero;

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
            {
                SetupUIElement(element);
            }
        }

        private void SetupUIElement(WorldSpaceUIElement element)
        {
            var uiObj = new GameObject(element.GetHashCode().ToString());
            uiObj.transform.SetParent(worldCanvas.transform);
            uiObj.transform.localRotation = Quaternion.identity;
            uiObj.transform.localPosition = Vector3.zero;

            var img = uiObj.AddComponent<Image>();
            img.sprite = element.Sprite;

            var rect = img.rectTransform;
            rect.sizeDelta = element.Size;
            rect.pivot = Vector2.one * 0.5f;
            rect.localScale = new Vector3(element.flipX ? -1 : 1, element.flipY ? -1 : 1, 1);

            element.uiObject = uiObj;
            element.image = img;
            element.rectTransform = rect;
        }

        private void UpdateElements()
        {
            foreach (var element in worldSpaceElements)
            {
                if (element.uiObject == null) continue;

                bool active = element.IsActive;
                element.uiObject.SetActive(active);
                element.image.enabled = active;
                if (!active) continue;

                UpdateUIElementSpriteAndSize(element);
                UpdateUIElementPosition(element);
            }
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

            if (element.interactableBehaviour is DoorInteractable && playerTransform != null)
                targetPos = CalculateDoorUIPosition(element);

            element.uiObject.transform.position = targetPos;
        }

        private Vector3 CalculateDoorUIPosition(WorldSpaceUIElement element)
        {
            Vector3 toPlayer = (playerTransform.position - element.interactableBehaviour.transform.position).normalized;
            Vector3 right = element.interactableBehaviour.transform.right;
            Vector3 up = Vector3.up;
            return element.interactableBehaviour.transform.position
                   + right * element.Offset.x
                   + up * element.Offset.y
                   + toPlayer * element.Offset.z;
        }

        private void UpdateLookDirections()
        {
            if (mainCamera == null) return;

            foreach (var element in worldSpaceElements)
            {
                if (element.uiObject == null || !element.uiObject.activeSelf) continue;
                if (element.LookTarget == LookAtTarget.Camera)
                    RotateUIElementTowardsCamera(element);
            }
        }

        private void RotateUIElementTowardsCamera(WorldSpaceUIElement element)
        {
            Vector3 direction = element.uiObject.transform.position - mainCamera.transform.position;
            direction.y = 0;
            if (direction.sqrMagnitude > 0.001f)
                element.uiObject.transform.rotation = Quaternion.LookRotation(direction);
        }

        private void UpdateCanvasScale()
        {
            float scaleFactor = Screen.height / 1080f;
            worldCanvas.transform.localScale = Vector3.one * canvasScale * scaleFactor;
        }
    }
}
