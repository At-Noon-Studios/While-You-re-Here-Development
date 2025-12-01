using UnityEditor;
using UnityEngine;

namespace ScriptableObjects.Interactable
{
    [CreateAssetMenu(fileName = "HoldableObject",  menuName = "ScriptableObjects/HoldableObject")]
    public class HoldableObjectData : ScriptableObject
    {
        [Range(0, 100)]
        [SerializeField] private float weight = 50f;
        [SerializeField] private Vector3 offset = Vector3.zero;
        [SerializeField] private Vector3 rotation = Vector3.zero;
        [SerializeField] private float droppingForce = 400f;
        [SerializeField, Layer] private int holdLayer;

        public float Weight => weight;
        public Vector3 Offset => offset;
        public Vector3 Rotation => rotation;
        public float DroppingForce => droppingForce;
        public int HoldLayer => holdLayer;
    }
}

public class LayerAttribute : PropertyAttribute { }

[CustomPropertyDrawer(typeof(LayerAttribute))]
public class LayerDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.propertyType == SerializedPropertyType.Integer)
        {
            property.intValue = EditorGUI.LayerField(position, label, property.intValue);
        }
        else
        {
            EditorGUI.LabelField(position, label.text, "Use [Layer] with int.");
        }
    }
}




