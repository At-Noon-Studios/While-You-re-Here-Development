using UnityEngine;

namespace component.making_tea
{
    [CreateAssetMenu(fileName="CcKettleFilled", menuName="QuestSystem/Components/KettleFilled", order=1)]
    public class SoCcKettleFilled : SoChoreComponent
    {
        public float requiredFill = 0.2f;
    }
}