using chopping_logs;
using UnityEngine;
using UnityEngine.UI;

namespace chopping_logs
{
    public class MouseChopController : MonoBehaviour
    {
        private enum ChopPhase
        {
            None,
            Up,
            Down
        }

        private ChopPhase _lastPhase = ChopPhase.None;
        private Stump _stump;

        [Header("Chop Settings")] public float movementThreshold = 0.1f;

        [Header("UI")] public Image mouseIcon;
        public Sprite mouseUpSprite;
        public Sprite mouseDownSprite;

        [Header("Chop Target")] [SerializeField]
        private LogChopTarget chopTarget;

        private void Update()
        {
            float mouseY = Input.GetAxis("Mouse Y");

            if (mouseY > movementThreshold && _lastPhase != ChopPhase.Up)
            {
                _lastPhase = ChopPhase.Up;
                OnMouseUp();
            }
            else if (mouseY < -movementThreshold && _lastPhase != ChopPhase.Down)
            {
                _lastPhase = ChopPhase.Down;
                OnMouseDown();
            }
        }

        private void OnMouseUp()
        {
            if (mouseIcon != null && mouseUpSprite != null)
                mouseIcon.sprite = mouseUpSprite;
        }

        private void OnMouseDown()
        {
            if (mouseIcon != null && mouseDownSprite != null)
                mouseIcon.sprite = mouseDownSprite;

            if (chopTarget != null && _stump.IsReadyForChop())
            {
                chopTarget.RegisterHit();
            }
        }
    }
}