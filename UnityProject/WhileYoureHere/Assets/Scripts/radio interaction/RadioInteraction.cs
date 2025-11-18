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
    [SerializeField] private LayerMask radioLayerMask;
    [SerializeField] private Camera cam;
    [SerializeField] private float lookDistance = 100f;
    // [SerializeField] private GameObject onSwitch;
    // [SerializeField] private GameObject offSwitch;
    [SerializeField] private GameObject canvas;
    private bool _isOn;
    private bool interacted;
    // private Interactible interactible;
    void Start()
    {
        // interactible = GetComponent<IInteractible>()
        _isOn = false;
    }

    void Update()
    {
        // Interact();
        
    }
    private void OnInteract(InputValue inputValue)
    {
        // Interact();
    }

    public override void Interact()
    {
        //wat gebeudrd er wanneer je e drukt op een radio
        
        var mousePos = Mouse.current.position.ReadValue(); 
        Ray ray = cam.ScreenPointToRay(mousePos);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, lookDistance,radioLayerMask))
        {

            Debug.Log(hit.collider.gameObject.name + " was hit");
            canvas.SetActive(true);
            // if (hit.collider.GetComponent<IInteractible>())
            // {
            // if (hit.collider.gameObject == onSwitch)
            // {
            //         _isOn = true;
            //         
            //         // optional add a light gameObject and toggle it on and off 
            //         
            // }
            // if (hit.collider.gameObject == offSwitch)
            // {
            //     _isOn = false;
            //     Debug.Log("Object" + hit.collider.gameObject.name + "is hitten");
            // }

            // handleOnOffSwitch();
            // }
        }
        else canvas.SetActive(false);
        
        
        Debug.DrawRay(ray.origin, ray.direction*100f, Color.red);
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
