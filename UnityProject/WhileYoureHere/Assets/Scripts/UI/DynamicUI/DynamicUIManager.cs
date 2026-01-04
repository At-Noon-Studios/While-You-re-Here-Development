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
        public enum StumpState { None, CutLog, PlaceLog, MouseUp, MouseDown, GuideLine }

        public enum KettleState
        {
            None,
            Pouring
        }

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

            [SerializeField] private HoldableObjectBehaviour requiredHeldObject;
            
            [Header("Required States")]
            [SerializeField] private DoorState requiredDoorState = DoorState.None;
            [SerializeField] private StumpState requiredStumpState = StumpState.None;
            [SerializeField] private KettleState requiredKettleState = KettleState.None;

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
                // Bestaande checks
                if (activationMode == ActivationMode.InteractableHovered &&
                    (interactableBehaviour == null || !interactableBehaviour.IsHovered || interactableBehaviour.blockInteraction))
                    return false;

                if (interactableBehaviour == null) return false;

                // Door check
                if (interactableBehaviour is DoorInteractable door && requiredDoorState != DoorState.None)
                    return CheckDoorState(door);

                // Stump check
                if (interactableBehaviour is Stump stump && requiredStumpState != StumpState.None)
                    return CheckStumpState(stump);

                // 🫖 Kettle check
                if (requiredKettleState != KettleState.None)
                    return CheckKettleState();

// Check of er een requiredHeldObject is toegewezen
                if (requiredHeldObject != null)
                {
                    // Zoek het object dat de speler momenteel vasthoudt
                    var held = GameObject.FindWithTag("Player")?.GetComponent<PlayerInteractionController>()?.HeldObject;

                    // Als de speler het juiste object niet vasthoudt, geen UI tonen
                    if (held != requiredHeldObject) 
                        return false;
                }

// 🫖 Kettle check
                if (requiredKettleState != KettleState.None)
                    return CheckKettleState();

                return true;

            }

            private bool CheckKettleState()
            {
                // Zoek de KettlePour in de scene
                var kettlePour = GameObject.FindObjectOfType<making_tea.KettlePour>();
                if (kettlePour == null || kettlePour.kettle == null) return false;

                // Voor Pouring state: moet gevuld zijn en vastgehouden
                if (requiredKettleState == KettleState.Pouring)
                {
                    bool isFilled = kettlePour.kettle.fillAmount > 0f;
                    bool isHeld = (kettlePour.TryGetComponent<Interactable.Holdable.HoldableObjectBehaviour>(out var h) && h.IsCurrentlyHeld)
                                  || (kettlePour.TryGetComponent<making_tea.KettleTablePickup>(out var t) && t.IsTableHeld);

                    return isFilled && isHeld;
                }

                return false;
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

                    if (requiredStumpState == StumpState.CutLog 
                        && held is HoldableObjectBehaviour h 
                        && h.GetComponentInChildren<AxeHitDetector>() != null
                        && stump.HasLog)
                        return true;

                    if (requiredStumpState == StumpState.PlaceLog 
                        && held is HoldableObjectBehaviour log 
                        && log.CompareTag("Log"))
                        return true;

                    if (!interactableBehaviour.IsHovered)
                        return false;

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
            Transform t = element.interactableBehaviour.transform;

            return t.position
                   + t.right   * element.Offset.x
                   + t.up      * element.Offset.y
                   + t.forward * element.Offset.z;
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
