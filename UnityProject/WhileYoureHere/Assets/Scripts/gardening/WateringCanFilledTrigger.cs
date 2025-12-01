using chore;
using making_tea;
using UnityEngine;

namespace gardening
{
    public class WateringCanFilledTrigger : MonoBehaviour
    {
        [SerializeField] private int wateringCanID;
        [SerializeField] private KettleFill wateringCan;
        [SerializeField] private float threshold = 0.2f;

        void Update()
        {
            if (wateringCan.fillAmount >= threshold)
            {
                ChoreEvents.TriggerWateringCanFilled(wateringCanID);
                enabled = false;
            }
        }
    }
}