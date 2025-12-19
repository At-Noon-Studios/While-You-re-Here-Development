using System.Collections.Generic;
using door;
using gamestate;
using UnityEngine;

public class GlobalTriggerManager : MonoBehaviour
{
    [Header("Doors to lock before end of day")]
    [SerializeField] private List<DoorInteractable> doorsToLock;
    private bool _isListeningForEndDay = false;

    public bool StartListeningForEndDay
    {
        set => _isListeningForEndDay = value;
    }

    private void Update()
    {
        if (_isListeningForEndDay) ListenForEndDay();
    }
    
    private void ListenForEndDay()
    {
        foreach (var door in doorsToLock)
        {
            if (!door.isLocked) return;
        }
        GamestateManager.GetInstance().listOfFlags.Find(flag => flag.name == "DoorsLocked").currentValue = true;
    }
}
