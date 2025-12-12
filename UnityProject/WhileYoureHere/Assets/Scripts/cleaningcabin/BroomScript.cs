using Interactable;
using Interactable.Holdable;
using ScriptableObjects.Interactable;
using UnityEngine;

public class BroomScript : HoldableObjectBehaviour
{
    [SerializeField] Transform holdBroomPoint;

    protected override void Awake()
    {
        base.Awake();
    }

    void Update()
    {
        CheckBroomCollision();
    }

    public bool IsBroomBeingHeld => IsHeld;

    public void CheckBroomCollision()
    {
        if (IsHeld) { Debug.Log(gameObject.name + " is am being held!"); }
    }

    public override void AttachTo(IInteractor interactor)
    {
        base.AttachTo(interactor);
        transform.SetParent(holdBroomPoint);
        Debug.Log("I am inside the attach method and my parent is: " + transform.parent);
    }
}