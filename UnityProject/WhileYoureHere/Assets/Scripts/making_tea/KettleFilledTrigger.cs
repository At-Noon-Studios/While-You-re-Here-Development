using chore;
using UnityEngine;

namespace making_tea
{
    public class KettleFilledTrigger : MonoBehaviour
    {
        [Header("Fill Trigger Settings")]
        public KettleFill kettle;
        public float minFill = 0.2f;

        private void Update()
        {
            if (!(kettle.fillAmount >= minFill)) return;
            ChoreEvents.TriggerKettleFilled();
            enabled = false;
        }
    }
}