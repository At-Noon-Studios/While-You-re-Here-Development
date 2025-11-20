using System;
using Interactable;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;

public class RadioInteraction : InteractableBehaviour
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
    private bool isOnClicked;
    private RadioController radioController;
    private bool isDragging;
    private float tuneValue;
    private Vector3 startMousePos;

    // private Interactible interactible;
    void Start()
    {
        // interactible = GetComponent<IInteractible>()
        _isOn = false;
        radioController = GetComponent<RadioController>();
    }

    void Update()
    {
        // Interact();
        HandleMouseInput();
        Debug.Log("is dragging in the update state = "+isDragging);

    }
    private void OnInteract(InputValue inputValue)
    {
            Debug.Log("Pressed");
            isOnClicked = inputValue.isPressed;
        Debug.Log("interacted = "+isOnClicked );
    }

    private void OnClick(InputValue inputValue)
    {
        isDragging = inputValue.isPressed;
        Debug.Log(isDragging);
    }

    public override void Interact()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
     
    }


    private void HandleMouseInput()
    {
        RaycastHit hit;
        Vector3 center = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        Ray ray = cam.ScreenPointToRay(center);
        Debug.DrawRay(ray.origin, ray.direction, Color.red);
        
            if (Physics.Raycast(ray, out  hit, 100, dialLayer))
            {
                Debug.Log("dialouge is hitten");
                
                 if (hit.transform == sliderLocation)
                 {
                     isDragging = true;
                     startMousePos = Mouse.current.position.ReadValue();
                 }
                
            }
        // On mouse drag
        if (isOnClicked && isDragging)
        {
            Vector2 currentMousePos = Mouse.current.position.ReadValue();
            float deltaX = currentMousePos.x - startMousePos.x;
        
            // Convert horizontal drag into tune value
            float sensitivity = 0.002f; // adjust to your liking
            tuneValue = Mathf.Clamp01(tuneValue + deltaX * sensitivity);
        
            startMousePos = currentMousePos; // update for smooth drag
        }
        
        // On mouse up, stop dragging
        if (!isOnClicked)
        {
            isDragging = false;
        }
        
       
        if (Physics.Raycast(ray, out  hit, 100, buttonLayer))
        {
            Debug.Log("button is hitten");
            if (isOnClicked)
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
