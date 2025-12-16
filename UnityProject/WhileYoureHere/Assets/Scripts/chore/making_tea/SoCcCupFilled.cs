using ScriptableObjects.chores;
using UnityEngine;

namespace chore.making_tea
{
    [CreateAssetMenu(fileName="CcCupFilled", menuName="QuestSystem/Components/CupFilled", order=4)]
    public class SoCcCupFilled : SoChoreComponent 
    {
        public float requiredFill = 0.2f;
    }

}