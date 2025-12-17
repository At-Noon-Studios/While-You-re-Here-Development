using UnityEngine;

namespace ScriptableObjects.Gamestate
{
    [CreateAssetMenu(fileName = "gamestateFlag", menuName = "ScriptableObjects/GamestateFlag", order = 0)]

    public class SoGamestateFlag : ScriptableObject
    {
        public bool defaultValue;
        public bool currentValue;
    }
}
