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

    public void CheckBroomCollision()
    {
        if (IsHeld)
        {
            IsHolding = true;
            Debug.Log(gameObject.name + " is am being held!");
        }
    }
}