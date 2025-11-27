using System.Collections.Generic;
using UnityEngine;

namespace chore
{
    public class ChoreManager : MonoBehaviour
    {
        [Header("Chore Manager")]
        [SerializeField] private List<SoChore> choresToLoad = new List<SoChore>();

        private Dictionary<int, Chore> _chores;

        private void Awake()
        {
            InitializeChores();
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            StartChore(0); // Starting first quest
        }

        private bool StartChore(int id)
        {
            if (!_chores.ContainsKey(id)) return false;
            if (_chores[id].ChoreStatus != ChoreStatus.Inactive) return false;

            _chores[id].Activate();
            _chores[id].OnChoreCompleted += ChoreCompleted;

            Debug.Log($"Chore {_chores[id].ChoreName} has been started");
            return true;
        }

        private void ChoreCompleted(Chore chore)
        {
            Debug.Log($"Chore {chore.ChoreName} has been completed");
            chore.OnChoreCompleted -= ChoreCompleted;

            ChoreEvents.TriggerQuestCompleted(chore);
            
            var nextID = chore.ChoreID + 1;
            if (_chores.ContainsKey(nextID))
                StartChore(nextID);
        }

        private void InitializeChores()
        {
            if (choresToLoad.Count <= 0) return;

            _chores = new Dictionary<int, Chore>();

            for (var i = 0; i < choresToLoad.Count; i++)
            {
                SoChore choreToLoad = choresToLoad[i];
                var chore = new Chore(choreToLoad.choreName, choreToLoad.id, choreToLoad.components);
                _chores.Add(chore.ChoreID, chore);

                Debug.Log($"Chore {chore.ChoreName} has been initialized");
            }
        }
    }
}
