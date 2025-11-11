using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Animator))]
public class MovementController : MonoBehaviour
{
    [Header("Movement Variables")]
    private float _moveX;
    private float _moveY;

    [Header("Movement Speed")]
    [SerializeField] private float movementSpeed = 2.2f;

    [Header("Components for animation & character controller")]
    Animator _animator;
    CharacterController _controller;

    void Awake()
    {
        _animator = GetComponent<Animator>();
        _controller = GetComponent<CharacterController>();
    }

    void OnMove(InputValue value)
    {
        Vector2 movement = value.Get<Vector2>();
        _moveX = movement.x;
        _moveY = movement.y;
    }

    void Update()
    {
        _animator.SetBool("isWalking", _moveY > 0);
        _animator.SetBool("isWalkingBackwards", _moveY < 0);
        _animator.SetBool("isStrafingLeft", _moveX < 0);
        _animator.SetBool("isStrafingRight", _moveX > 0);
        
        Vector3 movementFinal = new(_moveX, 0.0f, _moveY);
        _controller.Move(movementSpeed * Time.deltaTime * movementFinal);
    }
}
