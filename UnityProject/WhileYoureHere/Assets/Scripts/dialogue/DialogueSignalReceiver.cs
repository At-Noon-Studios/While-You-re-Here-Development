using ScriptableObjects.dialogue;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace dialogue
{
    [System.Serializable]
    public class DialogueSignalMapping
    {
        public SignalAsset signal;
        public DialogueInteractionConfig config;
    }

    public class DialogueSignalReceiver : MonoBehaviour, INotificationReceiver
    {
        [SerializeField] private DialogueLoader loader;
        [SerializeField] private DialogueSignalMapping[] dialogueSignals;
        
        public void Awake(){
            loader.gameObject.SetActive(true);
        }
        public void OnNotify(
            UnityEngine.Playables.Playable origin,
            INotification notification,
            object context)
        {
            if (notification is SignalEmitter emitter)
            {
                foreach (var map in dialogueSignals)
                {
                    if (map.signal == emitter.asset)
                    {
                        Debug.Log("Dialogue started");
                        loader.StartDialogue(map.config);
                        return;
                    }
                }
            }
        }
    }
}