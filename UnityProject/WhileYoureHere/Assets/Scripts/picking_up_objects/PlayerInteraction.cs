// using UnityEngine;
// using UnityEngine.InputSystem;
//
// namespace picking_up_objects
// {
//     public class PlayerInteraction : MonoBehaviour
//     {
//         [SerializeField] private Camera playerCamera;
//         // [SerializeField] private Transform holdPoint;
//         [SerializeField] private Transform placeObjectPoint;
//         
//         private Pickable _pickAbleObject;
//         private PlayerController _playerController;
//         
//         private void Awake()
//         {
//             _playerController = GetComponent<PlayerController>();
//         }
//         
//         void OnHold(InputValue input)
//         {
//           //  TryPickup();
//         }
//         
//         void OnDrop(InputValue input)
//         {
//
//         }
//         
//         void OnPlace(InputValue input)
//         {
//
//         }
//
//         void Update()
//         {
//             if (_pickAbleObject && Mouse.current.rightButton.wasPressedThisFrame)
//             {
//                 _pickAbleObject = null;
//                 _playerController.SetHeldObject(null);
//             }
//         }
//         
//     }
// }