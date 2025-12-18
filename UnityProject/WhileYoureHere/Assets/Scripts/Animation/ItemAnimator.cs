using UnityEngine;

public class ItemAnimator : MonoBehaviour
{
    [SerializeField] private Animator animator;

    public void PlayGrab()
    {
        animator.SetTrigger("ToGrab");
    }

    public void PlayDrop()
    {
        animator.SetTrigger("ToDrop");
    }


}
