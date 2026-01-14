using System.Collections.Generic;
using Interactable;
using Interactable.Holdable;
using UnityEngine;
using UnityEngine.UI;
using gamestate;
using TaskList;
using ScriptableObjects.Gamestate;
using ScriptableObjects.Events;
using UnityEngine.InputSystem;

namespace UI.DynamicUI
{
    public class ScreenSpaceUIManager : MonoBehaviour
    {
        public enum ActivationMode
        {
            Always,
            HoldableObjectHeld,
            NotebookOpen,
            NotebookClosed,
            EventFlags,
            EventChannels
        }

        [System.Serializable]
        public class ScreenSpaceUIElement
        {
            [Header("General")]
            [SerializeField] private string elementName;
            [SerializeField] private Sprite sprite;
            [SerializeField] private Vector2 size = new(128, 128);
            [SerializeField] private ActivationMode activationMode = ActivationMode.Always;

            [Header("Input Settings (optional)")]
            [SerializeField] private InputActionReference inputAction;
            [SerializeField] private int requiredInputUses;

            [Header("Holdable Object (optional)")]
            [SerializeField] private HoldableObjectBehaviour requiredHeldObject;

            [Header("Event Flags (optional)")]
            [SerializeField] private List<SoGamestateFlag> requiredFlags = new();

            [Header("Event Channels (optional)")]
            [SerializeField] private List<EventChannel> requiredEventChannels = new();

            [Header("Base Offset (bottom-left)")]
            [SerializeField] private Vector2 offset;

            [HideInInspector] public GameObject uiObject;
            [HideInInspector] public Image image;
            [HideInInspector] public RectTransform rectTransform;

            public ActivationMode Activation => activationMode;
            public HoldableObjectBehaviour RequiredHeldObject => requiredHeldObject;
            public List<SoGamestateFlag> RequiredFlags => requiredFlags;
            public List<EventChannel> RequiredEventChannels => requiredEventChannels;
            public InputActionReference InputAction => inputAction;
            public Vector2 Offset => offset;

            private int currentInputUses;

            public bool IsInputConditionSatisfied =>
                inputAction != null &&
                requiredInputUses > 0 &&
                currentInputUses >= requiredInputUses;

            public void OnInputPerformed(InputAction.CallbackContext _)
            {
                if (uiObject == null || !uiObject.activeInHierarchy || requiredInputUses <= 0)
                    return;

                currentInputUses++;
            }

            public void UpdateUI(Vector2 finalOffset)
            {
                image.sprite = sprite;
                rectTransform.sizeDelta = size;
                rectTransform.anchoredPosition = finalOffset;
            }
        }

        [Header("Screen Space UI Settings")]
        [SerializeField] private List<ScreenSpaceUIElement> screenSpaceElements;

        [Header("Layout")]
        [SerializeField] private Vector2 elementSpacing = new(140f, 0f);

        [Header("UI Scale")]
        [SerializeField] private float screenScale = 1f;

        [Header("TaskList Reference")]
        [SerializeField] private TaskListUI taskListUI;

        private Canvas screenCanvas;
        private RectTransform container;
        private readonly HashSet<EventChannel> triggeredEventChannels = new();
        private readonly Dictionary<EventChannel, System.Action> eventHandlers = new();

        private void Awake()
        {
            CreateCanvas();
            CreateElements();
            SubscribeInput();
            SubscribeEventChannels();
        }

        private void OnDestroy()
        {
            UnsubscribeInput();
            UnsubscribeEventChannels();
        }

        private void LateUpdate()
        {
            UpdateElements();
            container.localScale = Vector3.one * screenScale;
        }

        #region Setup

        private void CreateCanvas()
        {
            screenCanvas = new GameObject("ScreenSpaceCanvas", typeof(Canvas), typeof(GraphicRaycaster))
                .GetComponent<Canvas>();

            screenCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            screenCanvas.transform.SetParent(transform, false);

            container = new GameObject("ScreenSpaceContainer", typeof(RectTransform))
                .GetComponent<RectTransform>();

            container.SetParent(screenCanvas.transform, false);
            container.anchorMin = container.anchorMax = container.pivot = Vector2.zero;
        }

        private void CreateElements()
        {
            foreach (var e in screenSpaceElements)
            {
                var obj = new GameObject($"SSUI_{e.GetHashCode()}", typeof(Image));
                obj.transform.SetParent(container, false);

                var img = obj.GetComponent<Image>();
                var rect = img.rectTransform;

                rect.anchorMin = rect.anchorMax = rect.pivot = Vector2.zero;
                rect.sizeDelta = e.RequiredHeldObject ? rect.sizeDelta : rect.sizeDelta;

                e.uiObject = obj;
                e.image = img;
                e.rectTransform = rect;

                obj.SetActive(false);
            }
        }

        #endregion

        #region Update

        private void UpdateElements()
        {
            int index = 0;

            foreach (var e in screenSpaceElements)
            {
                bool active = ShouldBeActive(e);
                e.uiObject.SetActive(active);

                if (!active) continue;

                e.UpdateUI(e.Offset + elementSpacing * index);
                index++;
            }
        }

        private bool ShouldBeActive(ScreenSpaceUIElement e)
        {
            var pic = GameObject.FindWithTag("Player")
                ?.GetComponent<PlayerInteractionController>();

            if (pic != null && pic.IsTableMode)
                return false;
            
            if (e.IsInputConditionSatisfied)
                return false;

            return e.Activation switch
            {
                ActivationMode.Always => true,
                ActivationMode.HoldableObjectHeld => IsHolding(e.RequiredHeldObject),
                ActivationMode.NotebookOpen => IsNotebookPickedUp() && !IsNotebookOpen(),
                ActivationMode.NotebookClosed => IsNotebookPickedUp() && IsNotebookOpen(),
                ActivationMode.EventFlags => AllFlagsTrue(e.RequiredFlags),
                ActivationMode.EventChannels => AllChannelsTriggered(e.RequiredEventChannels),
                _ => false
            };
        }

        #endregion

        #region Conditions

        private bool IsHolding(HoldableObjectBehaviour required)
        {
            var controller = GameObject.FindWithTag("Player")
                ?.GetComponent<PlayerInteractionController>();

            var held = controller?.HeldObject as MonoBehaviour;
            var heldObj = held?.GetComponent<HoldableObjectBehaviour>();

            return required == null ? heldObj != null : heldObj == required;
        }

        private bool IsNotebookPickedUp() =>
            GamestateManager.GetInstance()
                ?.listOfFlags
                .Find(f => f.name == "NotebookPickedUpFlag")?.currentValue ?? false;

        private bool IsNotebookOpen() =>
            taskListUI != null && taskListUI.isOpen;

        private static bool AllFlagsTrue(List<SoGamestateFlag> flags)
        {
            if (flags == null || flags.Count == 0) return false;

            var gsm = GamestateManager.GetInstance();
            foreach (var f in flags)
                if (!gsm.listOfFlags.Contains(f) || !f.currentValue)
                    return false;

            return true;
        }

        private bool AllChannelsTriggered(List<EventChannel> channels)
        {
            if (channels == null || channels.Count == 0) return false;
            foreach (var c in channels)
                if (!triggeredEventChannels.Contains(c))
                    return false;
            return true;
        }

        #endregion

        #region Input & Events

        private void SubscribeInput()
        {
            foreach (var e in screenSpaceElements)
                if (e.InputAction?.action != null)
                    e.InputAction.action.performed += e.OnInputPerformed;
        }

        private void UnsubscribeInput()
        {
            foreach (var e in screenSpaceElements)
                if (e.InputAction?.action != null)
                    e.InputAction.action.performed -= e.OnInputPerformed;
        }

        private void SubscribeEventChannels()
        {
            foreach (var e in screenSpaceElements)
            {
                if (e.Activation != ActivationMode.EventChannels) continue;

                foreach (var channel in e.RequiredEventChannels)
                {
                    if (channel == null || eventHandlers.ContainsKey(channel)) continue;

                    System.Action handler = () => triggeredEventChannels.Add(channel);
                    eventHandlers[channel] = handler;
                    channel.OnRaise += handler;
                }
            }
        }

        private void UnsubscribeEventChannels()
        {
            foreach (var kvp in eventHandlers)
                kvp.Key.OnRaise -= kvp.Value;

            eventHandlers.Clear();
        }

        #endregion
    }
}
