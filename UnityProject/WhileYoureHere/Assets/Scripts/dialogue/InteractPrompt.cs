// using UnityEngine;
//
// namespace Dialogue
// {
//     public class InteractPrompt : MonoBehaviour
//     {
//         [Header("Player Settings")]
//         [SerializeField] private Transform player;
//
//         [Header("Interaction Settings")]
//         [SerializeField] private float interactDistance;
//
//         [Header("UI Settings")]
//         [SerializeField] private GameObject promptUI;
//
//         private bool _isInteracting;
//         private bool _isVisible;
//
//         private void Update()
//         {
//             if (_isInteracting) 
//                 return;
//
//             var distance = Vector3.Distance(player.position, transform.position);
//
//             if (distance <= interactDistance && !_isVisible)
//             {
//                 ShowPrompt();
//             }
//             else if (distance > interactDistance && _isVisible)
//             {
//                 HidePrompt();
//             }
//         }
//
//         public void BeginInteraction()
//         {
//             _isInteracting = true;
//             HidePrompt();
//         }
//
//         public void EndInteraction()
//         {
//             _isInteracting = false;
//         }
//
//         private void ShowPrompt()
//         {
//             promptUI.SetActive(true);
//             _isVisible = true;
//         }
//
//         private void HidePrompt()
//         {
//             promptUI.SetActive(false);
//             _isVisible = false;
//         }
//     }
// }