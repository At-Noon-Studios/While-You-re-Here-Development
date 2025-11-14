using UnityEngine;
using UnityEngine.InputSystem;

public class AxeController : MonoBehaviour
{

    private Animator _animator;

    [SerializeField] private string swingTrigger;
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnSwing(InputValue value)
    {
        if (value.isPressed && _animator != null)
        {
            _animator.SetTrigger(swingTrigger);
        }
    }
}
