using System;
using Interactable;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;

public class RadioInteraction : InteractableBehaviour//, IInteractible
{
    /* TODO:: add a raycast that detects when objects are hitting when E is pressed
        add a layer mask var in both unity and code
        add script to player if already exists
        
    */
    
    [SerializeField] private LayerMask dialLayer;
    [SerializeField] private LayerMask buttonLayer;
    [SerializeField] private LayerMask radioLayerMask;
    [SerializeField] private Camera cam;
    [SerializeField] private float lookDistance = 100f;
    [SerializeField] private Transform startButtonLocation;
    [SerializeField] private Transform sliderLocation;
    // [SerializeField] private GameObject onSwitch;
    // [SerializeField] private GameObject offSwitch;
    [SerializeField] private GameObject canvas;
    private bool _isOn;
    private bool interacted;
    private RadioController radioController;
    private bool isDragging;
    private float tuneValue;
    private Vector3 startMousePos;

    // private Interactible interactible;
    void Start()
    {
        // interactible = GetComponent<IInteractible>()
        _isOn = false;
    }

    void Update()
    {
        // Interact();
        HandleMouseInput();

    }
    private void OnInteract(InputValue inputValue)
    {
        // Interact();
        interacted = true;
    }

    public override void Interact()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
     
    }
    
    private void HandleMouseInput()
    {
        Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (interacted)
        {
            if (Physics.Raycast(ray, out RaycastHit hit, 100, dialLayer))
            {
                if (hit.transform == sliderLocation)
                {
                    isDragging = true;
                    startMousePos = Mouse.current.position.ReadValue();
                }
            }
        }
        // On mouse drag
        if (interacted && isDragging)
        {
            Vector2 currentMousePos = Mouse.current.position.ReadValue();
            float deltaX = currentMousePos.x - startMousePos.x;

            // Convert horizontal drag into tune value
            float sensitivity = 0.002f; // adjust to your liking
            tuneValue = Mathf.Clamp01(tuneValue + deltaX * sensitivity);

            startMousePos = currentMousePos; // update for smooth drag
        }

        // On mouse up, stop dragging
        if (!interacted)
        {
            isDragging = false;
        }
        
       
        if (Physics.Raycast(ray, out RaycastHit hitc, 100, buttonLayer))
        {
            if (interacted)
            {
                radioController.TurnRadioOn();
                Debug.Log("Radio turned ON");
            }
        }
        

    }

    
    
    // private void handleOnOffSwitch()
    // {
    //     Renderer OnRenderer = onSwitch.GetComponent<Renderer>();
    //     Renderer OffRenderer = offSwitch.GetComponent<Renderer>();
    //
    //     if (_isOn)
    //     {
    //         OffRenderer.material.color = Color.silver;
    //         OnRenderer.material.SetColor("_Color", Color.green);
    //         OnRenderer.material.color = Color.green;
    //         Debug.Log("on switch is on");
    //     }
    //     else if (!_isOn)
    //     {
    //         OnRenderer.material.color = Color.silver;
    //         OffRenderer.material.SetColor("_Color", Color.red);
    //         OffRenderer.material.color = Color.red;
    //         Debug.Log("off switch is off");
    //     }
    // }
    
    

}
