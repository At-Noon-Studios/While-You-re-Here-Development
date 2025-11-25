using UnityEngine;

[CreateAssetMenu(fileName = "Scavenging_Chore", menuName = "Scriptable Objects/ScavengingChore")]
public class ScavengingChore : ScriptableObject
{
    [Header("Sound which plays after collecting a plant")]
    [SerializeField] AudioClip pickupPlants;
    public AudioClip PickupPlants => pickupPlants;
}
