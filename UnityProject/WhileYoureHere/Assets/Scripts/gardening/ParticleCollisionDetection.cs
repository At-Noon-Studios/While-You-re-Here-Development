using entity;
using UnityEngine;

namespace gardening
{
   public class ParticleCollisionDetection : MonoBehaviour
   {
      [SerializeField] private PlantObject plant;

      private void OnParticleCollision(GameObject particle)
      {
         if (particle.GetComponentInParent<Watering>() != null)
         {
            plant.StartWatering();
         }
      }
   }
}
