using UnityEngine;

public class Enemy : MonoBehaviour
{
   [SerializeField] private int enemyID;

   private void OnMouseDown()
   {
      QuestEvents.TriggerEnemyKilled(enemyID);
      Destroy(gameObject);
   }
}
