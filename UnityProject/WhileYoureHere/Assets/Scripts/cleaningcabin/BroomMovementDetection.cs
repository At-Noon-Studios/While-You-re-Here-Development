using UnityEngine;
using UnityEngine.InputSystem;

namespace cleaningcabin
{
    public class BroomMovementDetection : MonoBehaviour
    {

        void Update()
        {
        }

        void OnClean(InputValue inputValue)
        {
            var broom = inputValue.Get<Vector2>();

            Debug.Log("I am the X value of the broom input: " + broom.x);
            Debug.Log("I am the Y value of the broom input: " + broom.y);
        }
    }
}