using Interactable;
using Interactable.Holdable;
using UnityEngine;

public class BroomScript : HoldableObjectBehaviour
{
    public bool IsHolding { get; private set; }

    CharacterController characterController;
    protected override void Awake()
    {
        base.Awake();
        characterController = GetComponentInParent<CharacterController>();
    }

    void Update()
    {
        CheckBroomCollision();
    }

    // private void OnCollisionStay(Collision other)
    // {
    //     Debug.Log("I am now touching: " + other.gameObject.name);
    //     if (other.gameObject.CompareTag("Player"))
    //     {
    //         IsHolding = true;
    //     }
    //     // IsHolding = other.gameObject.CompareTag("Player");
    // }

    // private void OnCollisionExit(Collision other)
    // {
    //     if (!other.gameObject.CompareTag("Player"))
    //     {
    //         IsHolding = false;
    //     }
    //     // IsHolding = other.gameObject.CompareTag("Player");
    //     Debug.Log("I am now touching: " + other.gameObject.name);
    // }

    public void CheckBroomCollision()
    {
        if (this.IsHeld)
        {
            Debug.Log("Bombo!");
        }
    }
}