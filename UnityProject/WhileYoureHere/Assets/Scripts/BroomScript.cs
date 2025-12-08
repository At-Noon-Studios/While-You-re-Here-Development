using Interactable;
using Interactable.Holdable;
using UnityEngine;

public class BroomScript : HoldableObjectBehaviour
{
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
}