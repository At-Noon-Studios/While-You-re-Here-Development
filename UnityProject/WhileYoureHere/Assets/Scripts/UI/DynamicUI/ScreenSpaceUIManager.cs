using System.Collections.Generic;
using Interactable;
using Interactable.Holdable;
using UnityEngine;
using UnityEngine.UI;
using gamestate; // Voor GamestateManager

namespace UI.DynamicUI
{
    public class ScreenSpaceUIManager : MonoBehaviour
    {
        public enum ActivationMode
        {
            Always,
            HoldableObjectHeld,
            TaskListPickedUp // Alleen kijken of notebook opgepakt is
        }

        [System.Serializable]
        public class ScreenSpaceUIElement
        {
            [Header("General")]
            [SerializeField] private string elementName;
            [SerializeField] private Sprite sprite;
            [SerializeField] private Vector2 size = new Vector2(128, 128);
            [SerializeField] private ActivationMode activationMode = ActivationMode.Always;

            [Header("Holdable Object (optional)")]
            [Tooltip("If set, this UI element will only show when the player holds this specific object.")]
            [SerializeField] private HoldableObjectBehaviour requiredHeldObject;

            [Header("Base Offset (bottom-left)")]
            [SerializeField] private Vector2 offset = Vector2.zero;

            [HideInInspector] public GameObject uiObject;
            [HideInInspector] public Image image;
            [HideInInspector] public RectTransform rectTransform;

            public ActivationMode Activation => activationMode;
            public Sprite Sprite => sprite;
            public Vector2 Size => size;
            public Vector2 Offset => offset;
            public HoldableObjectBehaviour RequiredHeldObject => requiredHeldObject;

            public void UpdateUI(Vector2 finalOffset)
            {
                if (image != null)
                    image.sprite = sprite;

                if (rectTransform != null)
                {
                    rectTransform.sizeDelta = size;
                    rectTransform.anchoredPosition = finalOffset;
                }
            }
        }

        [Header("Screen Space UI Settings")]
        [SerializeField] private List<ScreenSpaceUIElement> screenSpaceElements;

        [Header("Layout")]
        [Tooltip("Spacing between active UI elements (X = horizontal, Y = vertical)")]
        [SerializeField] private Vector2 elementSpacing = new Vector2(140f, 0f);

        [Header("UI Scale")]
        [SerializeField] private float screenScale = 1f;

        private Canvas screenCanvas;
        private RectTransform screenContainerRect;

        private void Awake()
        {
            CreateScreenCanvasAndContainer();
            InitializeScreenSpaceElements();
        }

        private void LateUpdate()
        {
            UpdateScreenSpaceElements();
            UpdateCanvasScale();
        }

        #region Canvas & Container

        private void CreateScreenCanvasAndContainer()
        {
            var canvasObj = new GameObject("ScreenSpaceCanvas");
            canvasObj.transform.SetParent(transform, false);

            screenCanvas = canvasObj.AddComponent<Canvas>();
            screenCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<GraphicRaycaster>();

            var containerObj = new GameObject("ScreenSpaceContainer");
            containerObj.transform.SetParent(canvasObj.transform, false);

            screenContainerRect = containerObj.AddComponent<RectTransform>();
            screenContainerRect.anchorMin = Vector2.zero;
            screenContainerRect.anchorMax = Vector2.zero;
            screenContainerRect.pivot = Vector2.zero;
            screenContainerRect.anchoredPosition = Vector2.zero;
            screenContainerRect.localScale = Vector3.one * screenScale;
        }

        #endregion

        #region Initialization

        private void InitializeScreenSpaceElements()
        {
            foreach (var e in screenSpaceElements)
            {
                var obj = new GameObject("SSUI_" + e.GetHashCode());
                obj.transform.SetParent(screenContainerRect, false);

                var img = obj.AddComponent<Image>();
                img.sprite = e.Sprite;

                var rect = img.rectTransform;
                rect.anchorMin = Vector2.zero;
                rect.anchorMax = Vector2.zero;
                rect.pivot = Vector2.zero;
                rect.sizeDelta = e.Size;

                e.uiObject = obj;
                e.image = img;
                e.rectTransform = rect;

                obj.SetActive(false);
            }
        }

        #endregion

        #region Update Logic

        private void UpdateScreenSpaceElements()
        {
            int activeIndex = 0;

            foreach (var e in screenSpaceElements)
            {
                if (e.uiObject == null) continue;

                bool shouldBeActive = ShouldElementBeActive(e);

                e.uiObject.SetActive(shouldBeActive);

                if (!shouldBeActive)
                    continue;

                Vector2 spacedOffset = e.Offset + elementSpacing * activeIndex;
                e.UpdateUI(spacedOffset);
                activeIndex++;
            }
        }

        private bool ShouldElementBeActive(ScreenSpaceUIElement e)
        {
            switch (e.Activation)
            {
                case ActivationMode.Always:
                    return true;

                case ActivationMode.HoldableObjectHeld:
                    return IsPlayerHolding(e.RequiredHeldObject);

                case ActivationMode.TaskListPickedUp:
                    return IsTaskListPickedUp();

                default:
                    return false;
            }
        }

        #endregion

        #region Helper Methods

        private bool IsPlayerHolding(HoldableObjectBehaviour requiredObject)
        {
            var player = GameObject.FindWithTag("Player");
            if (player == null) return false;

            var controller = player.GetComponent<PlayerInteractionController>();
            if (controller == null) return false;

            var held = controller.HeldObject as MonoBehaviour;
            if (held == null) return false;

            var heldObj = held.GetComponent<HoldableObjectBehaviour>();

            if (requiredObject == null)
                return heldObj != null;

            return heldObj == requiredObject;
        }

        private bool IsTaskListPickedUp()
        {
            var flag = GamestateManager.GetInstance()
                .listOfFlags.Find(f => f.name == "NotebookPickedUpFlag");

            return flag != null && flag.currentValue;
        }

        private void UpdateCanvasScale()
        {
            if (screenContainerRect != null)
                screenContainerRect.localScale = Vector3.one * screenScale;
        }

        #endregion
    }
}
