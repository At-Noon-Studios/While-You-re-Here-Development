using Interactable;
using Interactable.Holdable;
using UnityEngine;

public class CrateHoldable : HoldableObjectBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private Animator animator;
    [SerializeField] private AnimatorStateInfo animatorStateInfo;
    

    private void Awake()
    {
        base.Awake();
        animator = player.GetComponentInChildren<Animator>();
    }
    
    public override void Interact(IInteractor interactor)
    {
        base.Interact(interactor);
        animator.SetTrigger("PickUpCrate");
    }

    public override void Drop()
    {
        Debug.Log("Am I being triggered ???");
        base.Drop();
        animator.SetTrigger("DropCrate");
        // animator.SetBool("IsWalking");
    }
}
