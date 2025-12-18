using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Interactable;
using door;

namespace UI.DynamicUI
{
    public class DynamicUIManager : MonoBehaviour
    {
        public enum LookAtTarget
        {
            Camera
        }

        public enum ActivationMode
        {
            Always,
            InteractableHovered
        }

        public enum DoorState
        {
            None,
            Open,
            Closed,
            Locked
        }

        [System.Serializable]
        public class WorldSpaceUIElement
        {
            [SerializeField] private string elementName;

            [Header("Sprite")]
            [SerializeField] private Sprite sprite;
            [SerializeField] private Vector2 size = new Vector2(64, 64);

            [Header("Flip Options")]
            [SerializeField] public bool flipX;
            [SerializeField] public bool flipY;

            [Header("Activation")]
            [SerializeField] private ActivationMode activationMode = ActivationMode.Always;
            [SerializeField] private InteractableBehaviour interactableBehaviour;

            [Header("Door State")]
            [SerializeField] public DoorInteractable door;
            [SerializeField] public DoorState requiredDoorState = DoorState.None;

            [Header("Position Offset (non-door)")]
            [SerializeField] private Vector3 worldOffset = Vector3.up;

            [Header("Door UI Offset")]
            [SerializeField] private Vector3 doorOffset = new Vector3(0, 1.2f, 1.5f);

            [Header("Look At")]
            [SerializeField] private LookAtTarget lookAtTarget = LookAtTarget.Camera;

            [HideInInspector] public GameObject uiObject;
            [HideInInspector] public Image image;
            [HideInInspector] public RectTransform rectTransform;

            public Vector3 Offset => worldOffset;
            public Vector3 DoorOffset => doorOffset;
            public Sprite Sprite => sprite;
            public Vector2 Size => size;
            public LookAtTarget LookTarget => lookAtTarget;

            public bool IsActive
            {
                get
                {
                    if (activationMode == ActivationMode.InteractableHovered)
                    {
                        if (interactableBehaviour == null ||
                            !interactableBehaviour.IsHovered ||
                            interactableBehaviour.blockInteraction)
                            return false;
                    }

                    if (door == null || requiredDoorState == DoorState.None)
                        return true;

                    if (door.isLocked)
                        return requiredDoorState == DoorState.Locked;

                    if (door.isOpen)
                        return requiredDoorState == DoorState.Closed;

                    return requiredDoorState == DoorState.Open;
                }
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

            var player = GameObject.FindWithTag("Player");
            if (player != null) playerTransform = player.transform;

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
            GameObject canvasObj = new GameObject("WorldSpaceCanvas");
            canvasObj.transform.SetParent(transform);
            canvasObj.transform.localRotation = Quaternion.identity;

            worldCanvas = canvasObj.AddComponent<Canvas>();
            worldCanvas.renderMode = RenderMode.WorldSpace;
            worldCanvas.worldCamera = mainCamera;

            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.dynamicPixelsPerUnit = 10;

            canvasObj.AddComponent<GraphicRaycaster>();
            canvasObj.transform.localScale = Vector3.one * canvasScale;
        }

        private void InitializeElements()
        {
            foreach (var element in worldSpaceElements)
            {
                GameObject uiObj = new GameObject(element.GetHashCode().ToString());
                uiObj.transform.SetParent(worldCanvas.transform);
                uiObj.transform.localRotation = Quaternion.identity;

                Image img = uiObj.AddComponent<Image>();
                img.sprite = element.Sprite;

                RectTransform rect = img.rectTransform;
                rect.sizeDelta = element.Size;
                rect.pivot = Vector2.one * 0.5f;
                rect.localScale = new Vector3(element.flipX ? -1 : 1, element.flipY ? -1 : 1, 1);

                element.uiObject = uiObj;
                element.image = img;
                element.rectTransform = rect;
            }
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

                element.image.sprite = element.Sprite;
                element.rectTransform.sizeDelta = element.Size;

                Vector3 targetPos = transform.position + element.Offset;

                if (element.door != null && playerTransform != null)
                {
                    Vector3 toPlayer = (playerTransform.position - element.door.transform.position).normalized;
                    Vector3 doorRight = element.door.transform.right;
                    Vector3 worldUp = Vector3.up;

                    targetPos = element.door.transform.position
                                + doorRight * element.DoorOffset.x
                                + worldUp * element.DoorOffset.y
                                + toPlayer * element.DoorOffset.z;
                }

                element.uiObject.transform.position = targetPos;
            }
        }

        private void UpdateLookDirections()
        {
            foreach (var element in worldSpaceElements)
            {
                if (element.uiObject == null || !element.uiObject.activeSelf) continue;

                if (element.LookTarget == LookAtTarget.Camera && mainCamera != null)
                    element.uiObject.transform.LookAt(mainCamera.transform);
            }
        }

        private void UpdateCanvasScale()
        {
            float referenceHeight = 1080f;
            float scaleFactor = Screen.height / referenceHeight;
            worldCanvas.transform.localScale = Vector3.one * canvasScale * scaleFactor;
        }
    }
}
