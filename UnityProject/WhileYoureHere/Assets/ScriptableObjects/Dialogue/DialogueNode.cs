using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueNode", menuName = "Dialogue/Node")]
public class DialogueNode : ScriptableObject
{
    public string nodeID;
    public string speakerName;
    [TextArea(3, 10)]
    public string dialogueText;
    public List<DialogueChoice> choices;
}