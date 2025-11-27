using UnityEngine;

namespace component
{
    [CreateAssetMenu(fileName="CcCupFilled", menuName="QuestSystem/Components/CupFilled", order=4)]
    public class SoCcCupFilled : SoChoreComponent 
    {
        public float requiredFill = 0.2f;
    }

}