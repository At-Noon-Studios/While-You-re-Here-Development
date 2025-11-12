using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueNode", menuName = "Scriptable Objects/DialogueNode")]
public class DialogueNode : ScriptableObject
{
    public string nodeID;
    public string speakerName;
    [TextArea] public string dialogueText;
    public List<DialogueChoice> choices;
}
