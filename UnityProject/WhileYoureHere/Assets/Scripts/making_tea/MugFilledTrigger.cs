using chore;
using UnityEngine;

namespace making_tea
{
    public class MugFilledTrigger : MonoBehaviour
    {
        [Header("Fill Trigger Settings")]
        public KettleFill cup;
        public float minFill = 0.2f;

        private void Update()
        {
            if (!(cup.fillAmount >= minFill)) return;

            ChoreEvents.TriggerCupFilled();
            enabled = false;
        }
    }
}