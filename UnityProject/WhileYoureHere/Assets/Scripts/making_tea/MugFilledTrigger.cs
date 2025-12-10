using chore;
using UnityEngine;

namespace making_tea
{
    public class MugFilledTrigger : MonoBehaviour
    {
        public KettleFill cup;
        public float threshold = 0.2f;

        private void Update()
        {
            if (!(cup.fillAmount >= threshold)) return;

            ChoreEvents.TriggerCupFilled();
            enabled = false;
        }
    }
}