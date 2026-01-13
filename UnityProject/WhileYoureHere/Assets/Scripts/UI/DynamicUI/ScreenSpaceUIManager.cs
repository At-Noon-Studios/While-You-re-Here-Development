using System.Collections.Generic;
using Interactable;
using Interactable.Holdable;
using UnityEngine;
using UnityEngine.UI;
using gamestate; // Voor GamestateManager
using TaskList; // Voor TaskListUI
using ScriptableObjects.Gamestate; // Voor SoGamestateFlag
using ScriptableObjects.Events; // Voor EventChannel
using UnityEngine.InputSystem; // Voor InputActionReference

namespace UI.DynamicUI
{
    public class ScreenSpaceUIManager : MonoBehaviour
    {
        public enum ActivationMode
        {
            Always,
            HoldableObjectHeld,
            NotebookOpen,     // notebook opgepakt én open
            NotebookClosed,   // notebook opgepakt én gesloten
            EventFlags,       // sprite zichtbaar wanneer alle geselecteerde events/flags true zijn
            EventChannels     // sprite zichtbaar wanneer alle geselecteerde EventChannels zijn getriggerd
        }

        [System.Serializable]
        public class ScreenSpaceUIElement
        {
            [Header("General")]
            [SerializeField] private string elementName;
            [SerializeField] private Sprite sprite;
            [SerializeField] private Vector2 size = new Vector2(128, 128);
            [SerializeField] private ActivationMode activationMode = ActivationMode.Always;

            [Header("Input Settings (optional)")]
            [Tooltip("Koppel een Input Action (uit bijv. PlayerInput.inputactions) aan dit UI-element.")]
            [SerializeField] private InputActionReference inputAction;
            [Tooltip("Hoe vaak de gekoppelde input geactiveerd moet worden voordat dit element verdwijnt. 0 of kleiner = negeren.")]
            [SerializeField] private int requiredInputUses = 0;

            [Header("Holdable Object (optional)")]
            [Tooltip("If set, this UI element will only show when the player holds this specific object.")]
            [SerializeField] private HoldableObjectBehaviour requiredHeldObject;

            [Header("Event Flags (optional)")]
            [Tooltip("Selecteer events/flags die allemaal true moeten zijn om dit element te tonen. Alleen gebruikt wanneer ActivationMode = EventFlags.")]
            [SerializeField] private List<SoGamestateFlag> requiredFlags = new List<SoGamestateFlag>();

            [Header("Event Channels (optional)")]
            [Tooltip("Selecteer EventChannels die allemaal getriggerd moeten zijn om dit element te tonen. Alleen gebruikt wanneer ActivationMode = EventChannels.")]
            [SerializeField] private List<EventChannel> requiredEventChannels = new List<EventChannel>();

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
            public List<SoGamestateFlag> RequiredFlags => requiredFlags;
            public List<EventChannel> RequiredEventChannels => requiredEventChannels;
            public InputActionReference InputAction => inputAction;

            // Runtime state voor input-gebruik
            private int currentInputUses;

            /// <summary>
            /// Geeft aan of dit element verborgen moet blijven omdat het vereiste aantal inputs is gehaald.
            /// </summary>
            public bool IsInputConditionSatisfied =>
                inputAction != null &&
                requiredInputUses > 0 &&
                currentInputUses >= requiredInputUses;

            /// <summary>
            /// Aanroepen wanneer het gekoppelde InputAction een performed-callback krijgt.
            /// </summary>
            public void OnInputPerformed(InputAction.CallbackContext ctx)
            {
                // Alleen tellen als de sprite daadwerkelijk zichtbaar/actief is
                if (uiObject == null || !uiObject.activeInHierarchy)
                    return;

                // Alleen tellen als dit element daadwerkelijk een limiet heeft
                if (requiredInputUses <= 0)
                    return;

                currentInputUses++;
            }

            /// <summary>
            /// Optioneel te gebruiken om de teller te resetten, bijv. bij scene reload of expliciete reset.
            /// </summary>
            public void ResetInputUsage()
            {
                currentInputUses = 0;
            }

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

        [Header("TaskList Reference")]
        [SerializeField] private TaskListUI taskListUI; // zet deze in inspector

        private Canvas screenCanvas;
        private RectTransform screenContainerRect;
        
        // Tracking van getriggerde EventChannels
        private HashSet<EventChannel> triggeredEventChannels = new HashSet<EventChannel>();
        
        // Dictionary om event handlers bij te houden voor correcte unsubscribe
        private Dictionary<EventChannel, System.Action> eventChannelHandlers = new Dictionary<EventChannel, System.Action>();

        private void Awake()
        {
            CreateScreenCanvasAndContainer();
            InitializeScreenSpaceElements();
            SubscribeToInputActions();
            SubscribeToEventChannels();
        }

        private void OnDestroy()
        {
            UnsubscribeFromInputActions();
            UnsubscribeFromEventChannels();
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
            // Als de gekoppelde input genoeg gebruikt is, tonen we dit element nooit meer
            if (e.IsInputConditionSatisfied)
                return false;

            switch (e.Activation)
            {
                case ActivationMode.Always:
                    return true;

                case ActivationMode.HoldableObjectHeld:
                    return IsPlayerHolding(e.RequiredHeldObject);

                case ActivationMode.NotebookOpen:
                    return IsTaskListPickedUp() && !IsTaskListOpen();

                case ActivationMode.NotebookClosed:
                    return IsTaskListPickedUp() && IsTaskListOpen();

                case ActivationMode.EventFlags:
                    return AreAllFlagsTrue(e.RequiredFlags);

                case ActivationMode.EventChannels:
                    return AreAllEventChannelsTriggered(e.RequiredEventChannels);

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

        private bool IsTaskListOpen()
        {
            if (taskListUI == null) return false;
            return taskListUI.isOpen; // public property in TaskListUI
        }

        private bool AreAllFlagsTrue(List<SoGamestateFlag> flags)
        {
            if (flags == null || flags.Count == 0)
                return false; // Geen flags geselecteerd = niet actief

            var gamestateManager = GamestateManager.GetInstance();
            if (gamestateManager == null)
                return false;

            // Check of alle geselecteerde flags true zijn
            foreach (var flag in flags)
            {
                if (flag == null) continue; // Skip null entries

                // Zoek de flag in de gamestate manager
                var foundFlag = gamestateManager.listOfFlags.Find(f => f == flag);
                if (foundFlag == null || !foundFlag.currentValue)
                    return false; // Flag niet gevonden of niet true
            }

            return true; // Alle flags zijn true
        }

        private bool AreAllEventChannelsTriggered(List<EventChannel> eventChannels)
        {
            if (eventChannels == null || eventChannels.Count == 0)
                return false; // Geen event channels geselecteerd = niet actief

            // Check of alle geselecteerde event channels zijn getriggerd
            foreach (var eventChannel in eventChannels)
            {
                if (eventChannel == null) continue; // Skip null entries

                if (!triggeredEventChannels.Contains(eventChannel))
                    return false; // Event channel nog niet getriggerd
            }

            return true; // Alle event channels zijn getriggerd
        }

        private void SubscribeToEventChannels()
        {
            // Verzamel alle unieke EventChannels van alle elementen
            HashSet<EventChannel> allEventChannels = new HashSet<EventChannel>();
            
            foreach (var element in screenSpaceElements)
            {
                if (element.Activation == ActivationMode.EventChannels && element.RequiredEventChannels != null)
                {
                    foreach (var eventChannel in element.RequiredEventChannels)
                    {
                        if (eventChannel != null)
                            allEventChannels.Add(eventChannel);
                    }
                }
            }

            // Abonneer op alle EventChannels
            foreach (var eventChannel in allEventChannels)
            {
                // Maak een handler en bewaar deze voor correcte unsubscribe
                if (!eventChannelHandlers.ContainsKey(eventChannel))
                {
                    System.Action handler = () => OnEventChannelTriggered(eventChannel);
                    eventChannelHandlers[eventChannel] = handler;
                    eventChannel.OnRaise += handler;
                }
            }
        }

        #region Input Action Subscriptions

        private void SubscribeToInputActions()
        {
            foreach (var element in screenSpaceElements)
            {
                var inputRef = element.InputAction;
                if (inputRef == null)
                    continue;

                var action = inputRef.action;
                if (action == null)
                    continue;

                action.performed += element.OnInputPerformed;
            }
        }

        private void UnsubscribeFromInputActions()
        {
            foreach (var element in screenSpaceElements)
            {
                var inputRef = element.InputAction;
                if (inputRef == null)
                    continue;

                var action = inputRef.action;
                if (action == null)
                    continue;

                action.performed -= element.OnInputPerformed;
            }
        }

        #endregion

        private void UnsubscribeFromEventChannels()
        {
            // Deabonneer van alle EventChannels met de opgeslagen handlers
            foreach (var kvp in eventChannelHandlers)
            {
                if (kvp.Key != null)
                {
                    kvp.Key.OnRaise -= kvp.Value;
                }
            }
            
            eventChannelHandlers.Clear();
        }

        private void OnEventChannelTriggered(EventChannel eventChannel)
        {
            if (eventChannel == null) return;

            // Markeer dit event channel als getriggerd
            triggeredEventChannels.Add(eventChannel);
        }

        private void UpdateCanvasScale()
        {
            if (screenContainerRect != null)
                screenContainerRect.localScale = Vector3.one * screenScale;
        }

        #endregion
    }
}
