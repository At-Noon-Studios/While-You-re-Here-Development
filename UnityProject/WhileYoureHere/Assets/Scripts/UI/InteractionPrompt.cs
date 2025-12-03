using System.Collections;
using ScriptableObjects.UI;
using TMPro;
using UnityEngine;

namespace UI
{
    [RequireComponent(typeof(Canvas))]
    public class InteractionPrompt : MonoBehaviour
    {
        [SerializeField] private InteractionPromptData  data;
        private Canvas _canvas;
        [SerializeField] private TextMeshProUGUI  textMesh;
        
        private float _originalAlpha;
        private Color _originalColor;
        private Vector3 _originalScale;
        
        private Coroutine _currentRoutine;
        
        private void Awake()
        {
            _canvas = GetComponent<Canvas>();
        }
        
        public void PulseInteractPrompt()
        {
            if (_currentRoutine != null)
            {
                StopCoroutine(_currentRoutine);
                ResetPrompt();
            }
            _currentRoutine = StartCoroutine(PulseRoutine());
        }
        
        public void ShowInteractPrompt(string text, bool allowed)
        {
            gameObject.SetActive(true);
            textMesh.color = allowed ? data.InteractionAllowedColor : data.InteractionNotAllowedColor;
            textMesh.alpha = allowed ? data.InteractionAllowedAlpha : data.InteractionNotAllowedAlpha;
            textMesh.text = text;
        }

        public void HideInteractPrompt()
        {
            gameObject.SetActive(false);
        }
        
        private IEnumerator PulseRoutine()
        {
            _originalScale = textMesh.transform.localScale;
            _originalAlpha = textMesh.alpha;
            _originalColor = textMesh.color;
            var elapsed = 0f;
            var targetScale = _originalScale * data.PulseRoutineTargetScale;
            while (elapsed < data.PulseRoutineDuration)
            {
                var t = elapsed / data.PulseRoutineDuration;
                var pulse = Mathf.Sin(t * Mathf.PI);
                textMesh.transform.localScale = Vector3.Lerp(_originalScale, targetScale, pulse);
                var color = Color.Lerp(_originalColor, data.PulseRoutineTargetColor, pulse);
                textMesh.color = new Color(
                    color.r,
                    color.g,
                    color.b,
                    Mathf.Lerp(_originalAlpha, data.PulseRoutineTargetAlpha, pulse)
                );
                elapsed += Time.deltaTime;
                yield return null;
            }
            ResetPrompt();
        }

        private void ResetPrompt()
        {
            textMesh.transform.localScale = _originalScale;
            textMesh.alpha = _originalAlpha;
            textMesh.color = _originalColor;
        }
    }
}