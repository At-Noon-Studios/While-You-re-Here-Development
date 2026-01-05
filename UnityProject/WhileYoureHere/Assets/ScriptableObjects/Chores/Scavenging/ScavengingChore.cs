using UnityEngine;

namespace ScriptableObjects.Chores
{
    [CreateAssetMenu(fileName = "Scavenging_Chore", menuName = "ScriptableObjects/ChoreSystem/ScavengingChore")]
    public class ScavengingChore : ScriptableObject
    {
        [Header("Sound which plays after collecting a plant")]
        [SerializeField] AudioClip pickupPlantsSound;
        public AudioClip PickupPlants => pickupPlantsSound;
    }
}
