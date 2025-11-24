using picking_up_objects;
using ScriptableObjects.picking_up_objects;
using UnityEngine;
using UnityEngine.InputSystem;

namespace chopping_logs
{
    public class ChopController : MonoBehaviour
    {
        
        [SerializeField] private HeldObjectController _heldObjectController;
        [SerializeField] private Stump stump;
        [SerializeField] private Animator axeAnimator;
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip placeSound, impactSound, swingSound, gruntSound, chopSound;

        private Vector2 _swipeStart;
        private bool _swipeReady;
        
        private LogBehaviour _logBehaviour;
        
        private void Start()
        {
            _swipeReady = false;
        }
        
        
        private void Update()
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                _swipeStart = Mouse.current.position.ReadValue();
                _swipeReady = true;
            }

            if (Mouse.current.leftButton.wasReleasedThisFrame && _swipeReady)
            {
                var swipeEnd = Mouse.current.position.ReadValue();
                var verticalSwipe = swipeEnd.y - _swipeStart.y;
                
                // if (verticalSwipe < -100f && _heldObjectController.HasAxe && stump.HasLog)
                // {
                //     ChopLog();
                // }
            }
        }

        private void ChopLog()
        {
            axeAnimator.SetTrigger("Chop");
            audioSource.PlayOneShot(chopSound);
            _logBehaviour.SplitLog(stump.GetLog());
            stump.ClearLog();
        }
        
    }
}