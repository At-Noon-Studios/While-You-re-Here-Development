using UnityEngine;

namespace component.making_tea
{
    [CreateAssetMenu(fileName="CcCupFilled", menuName="QuestSystem/Components/CupFilled", order=3)]
    public class SoCcCupFilled : SoChoreComponent 
    {
        public float requiredFill = 0.2f;
    }

}