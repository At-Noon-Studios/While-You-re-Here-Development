using chore;
using UnityEngine;

namespace making_tea
{
    public class KettleFilledTrigger : MonoBehaviour
    {
        public KettleFill kettle;
        public float threshold = 0.2f;

        void Update()
        {
            if (kettle.fillAmount >= threshold)
            {
                ChoreEvents.TriggerKettleFilled();
                enabled = false;
            }
        }
    }
}