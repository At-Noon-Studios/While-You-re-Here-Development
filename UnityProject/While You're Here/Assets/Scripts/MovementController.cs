using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Animator))]
public class MovementController : MonoBehaviour
{
    private float _moveX;
    private float _moveY;

    public float movementSpeed = 2.2f;

    Animator animator;
    CharacterController controller;

    void Awake()
    {
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
    }

    void OnMove(InputValue value)
    {
        Vector2 movement = value.Get<Vector2>();
        _moveX = movement.x;
        _moveY = movement.y;
    }

    void Update()
    {
        Debug.Log("the value of _moveX is: " + _moveX);
        Debug.Log("the value of _moveY is: " + _moveY);

        bool pressingW = _moveY > 0f;
        bool pressingS = _moveY < 0f;

        bool pressingA = _moveX < 0f;
        bool pressingD = _moveX > 0f;

        bool isIdle = _moveY == 0f;

        if (pressingW)
        {
            animator.SetBool("isWalking", true);
        }
        if (!pressingW)
        {
            animator.SetBool("isWalking", false);
        }
        if (pressingS)
        {
            animator.SetBool("isWalkingBackwards", true);
        }
        if (!pressingS)
        {
            animator.SetBool("isWalkingBackwards", false);
        }
        if (pressingA)
        {
            animator.SetBool("isStrafingLeft", true);
        }
        if (!pressingA)
        {
            animator.SetBool("isStrafingLeft", false);
        }
        if (pressingD)
        {
            animator.SetBool("isStrafingRight", true);
        }
        if (!pressingD)
        {
            animator.SetBool("isStrafingRight", false);
        }
        
        Vector3 movementFinal = new(_moveX, 0.0f, _moveY);
        controller.Move(movementSpeed * Time.deltaTime * movementFinal);
    }
}
