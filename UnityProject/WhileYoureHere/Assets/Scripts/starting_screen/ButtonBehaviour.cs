using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

namespace starting_screen
{
    public class ButtonBehaviour : MonoBehaviour
    {
        [Header("References")] 
        [SerializeField] private Button button;
        [SerializeField] private TextMeshProUGUI buttonText;

        [Header("Colors")] [SerializeField] private Color normalColor = Color.white;
        [SerializeField] private Color highlightedColor = new Color(1f, 0.5f, 0f);
        [SerializeField] private Color pressedColor = new Color(1f, 0.5f, 0f);

        private void Start()
        {
            if (button == null || buttonText == null)
            {
                Debug.LogError("Button or ButtonText not assigned!");
                return;
            }

            var img = GetComponent<Image>();
            if (img != null)
                button.targetGraphic = img;

            button.transition = Selectable.Transition.None;

            var trigger = button.gameObject.GetComponent<EventTrigger>();
            if (trigger == null)
                trigger = button.gameObject.AddComponent<EventTrigger>();
            else
                trigger.triggers.Clear();

            AddEvent(trigger, EventTriggerType.PointerEnter, () =>
            {
                var c = buttonText.color;
                c.r = highlightedColor.r;
                c.g = highlightedColor.g;
                c.b = highlightedColor.b;
                buttonText.color = c;
            });

            AddEvent(trigger, EventTriggerType.PointerExit, () =>
            {
                var c = buttonText.color;
                c.r = normalColor.r;
                c.g = normalColor.g;
                c.b = normalColor.b;
                buttonText.color = c;
            });

            AddEvent(trigger, EventTriggerType.PointerDown, () =>
            {
                var c = buttonText.color;
                c.r = pressedColor.r;
                c.g = pressedColor.g;
                c.b = pressedColor.b;
                buttonText.color = c;
            });

            AddEvent(trigger, EventTriggerType.PointerUp, () =>
            {
                var c = buttonText.color;
                c.r = highlightedColor.r;
                c.g = highlightedColor.g;
                c.b = highlightedColor.b;
                buttonText.color = c;
            });
        }


        private static void AddEvent(EventTrigger trigger, EventTriggerType type, System.Action action)
        {
            var entry = new EventTrigger.Entry { eventID = type };
            entry.callback.AddListener((_) => action());
            trigger.triggers.Add(entry);
        }
    }
}