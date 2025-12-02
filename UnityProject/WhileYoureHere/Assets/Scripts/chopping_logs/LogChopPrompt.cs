using UnityEngine;
using UnityEngine.UI;

namespace chopping_logs
{
    public class LogChopPrompt : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private LogChopTarget chopTarget;
        [SerializeField] private Image guideLine;
        [SerializeField] private Image mouseIcon;
        [SerializeField] private Sprite mouseUpSprite;
        [SerializeField] private Sprite mouseDownSprite;

        [Header("Behavior")]
        [SerializeField] private float movementThreshold = 0.1f;
        [SerializeField] private Vector3 lineOffset = Vector3.zero;

        private Stump _stump;
        private ChopPhase _lastPhase = ChopPhase.None;

        private enum ChopPhase { None, Up, Down }

        private void Awake()
        {
            if (chopTarget == null)
                chopTarget = GetComponent<LogChopTarget>();

            _stump = chopTarget != null ? chopTarget.GetComponentInParent<Stump>() : null;
            
            SetVisible(false);
            
            if (guideLine != null && chopTarget != null)
            {
                var t = guideLine.rectTransform;
                t.position = chopTarget.transform.position + lineOffset;
                t.rotation = chopTarget.transform.rotation;
            }
        }

        private void Update()
        {
            if (_stump == null || chopTarget == null)
                return;
            
            if (_stump.MinigameActive && _stump.HasLog)
                SetVisible(true);
            else
            {
                SetVisible(false);
                return;
            }
            
            float mouseY = Input.GetAxis("Mouse Y");

            if (mouseY > movementThreshold && _lastPhase != ChopPhase.Up)
            {
                _lastPhase = ChopPhase.Up;
                SetMouseIcon(mouseUpSprite);
            }
            else if (mouseY < -movementThreshold && _lastPhase != ChopPhase.Down)
            {
                _lastPhase = ChopPhase.Down;
                SetMouseIcon(mouseDownSprite);

                chopTarget.RegisterHit();
            }
        }

        private void SetVisible(bool visible)
        {
            if (guideLine != null) guideLine.enabled = visible;
            if (mouseIcon != null) mouseIcon.enabled = visible;
        }

        private void SetMouseIcon(Sprite sprite)
        {
            if (mouseIcon != null && sprite != null)
                mouseIcon.sprite = sprite;
        }
    }
}