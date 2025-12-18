using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Interactable;

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
            InteractableHovered,
        }

        [System.Serializable]
        public class WorldSpaceUIElement
        {
            [SerializeField] private string elementName;

            [Header("Sprite")]
            [SerializeField] private Sprite sprite;
            [SerializeField] private Vector2 size = new Vector2(64, 64);

            [Header("Flip Options")]
            [SerializeField] private bool flipX = false;
            [SerializeField] private bool flipY = false;

            [Header("Activation")]
            [SerializeField] private ActivationMode activationMode = ActivationMode.Always;
            [SerializeField] private InteractableBehaviour interactableBehaviour;

            [Header("Position Offset")]
            [SerializeField] private Vector3 worldOffset = Vector3.up;

            [Header("Look At")]
            [SerializeField] private LookAtTarget lookAtTarget = LookAtTarget.Camera;

            [HideInInspector] public GameObject uiObject;
            [HideInInspector] public Image image;
            [HideInInspector] public RectTransform rectTransform;

            public string ElementName => elementName;
            public Vector3 Offset => worldOffset;
            public Sprite Sprite => sprite;
            public Vector2 Size => size;
            public LookAtTarget LookTarget => lookAtTarget;
            public bool FlipX => flipX;
            public bool FlipY => flipY;
            public ActivationMode Mode => activationMode;
            public InteractableBehaviour Interactable => interactableBehaviour;

            public bool IsActive
            {
                get
                {
                    switch (activationMode)
                    {
                        case ActivationMode.Always:
                            return true;
                        case ActivationMode.InteractableHovered:
                            return interactableBehaviour != null && interactableBehaviour.IsHovered && !interactableBehaviour.blockInteraction;
                        default:
                            return true;
                    }
                }
            }
        }

        [Header("World Space UI Settings")]
        [SerializeField] private List<WorldSpaceUIElement> worldSpaceElements;

        [SerializeField] private float canvasScale = 0.01f;

        private Camera mainCamera;
        private Canvas worldCanvas;

        private void Awake()
        {
            mainCamera = Camera.main;
            CreateWorldCanvas();
            InitializeElements();
        }

        private void LateUpdate()
        {
            UpdateElementPositions();
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
            if (worldSpaceElements == null) return;

            foreach (var element in worldSpaceElements)
            {
                if (string.IsNullOrEmpty(element.ElementName)) continue;

                GameObject uiObj = new GameObject(element.ElementName);
                uiObj.transform.SetParent(worldCanvas.transform);
                uiObj.transform.localRotation = Quaternion.identity;

                Image img = uiObj.AddComponent<Image>();
                img.sprite = element.Sprite;
                img.enabled = element.IsActive;

                RectTransform rect = img.rectTransform;
                rect.sizeDelta = element.Size;
                rect.pivot = new Vector2(0.5f, 0.5f);
                rect.localScale = new Vector3(element.FlipX ? -1 : 1, element.FlipY ? -1 : 1, 1);

                element.uiObject = uiObj;
                element.image = img;
                element.rectTransform = rect;
            }
        }

        private void UpdateElementPositions()
        {
            foreach (var element in worldSpaceElements)
            {
                if (element.uiObject == null) continue;

                element.uiObject.SetActive(element.IsActive);
                if (element.image != null)
                    element.image.enabled = element.IsActive;

                element.image.sprite = element.Sprite;
                element.rectTransform.sizeDelta = element.Size;
                element.rectTransform.localScale = new Vector3(element.FlipX ? -1 : 1, element.FlipY ? -1 : 1, 1);

                element.uiObject.transform.position = transform.position + element.Offset;
            }
        }

        private void UpdateLookDirections()
        {
            foreach (var element in worldSpaceElements)
            {
                if (element.uiObject == null) continue;

                Transform t = element.uiObject.transform;

                switch (element.LookTarget)
                {
                    case LookAtTarget.Camera:
                        if (mainCamera != null)
                            t.LookAt(mainCamera.transform);
                        break;
                }
            }
        }
        
        private void UpdateCanvasScale()
        {
            if (worldCanvas == null) return;

            float referenceHeight = 1080f;
            float scaleFactor = Screen.height / referenceHeight;
            worldCanvas.transform.localScale = Vector3.one * canvasScale * scaleFactor;
        }
    }
}
