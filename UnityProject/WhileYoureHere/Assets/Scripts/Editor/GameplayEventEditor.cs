using gamestate;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomPropertyDrawer(typeof(GameplayEvent))]
    public class GameplayEventDrawer : PropertyDrawer
    {
        private ISpecialField _eventField;
        private ISpecialField _triggerField;

        private GameplayEventType _cachedEventType;
        private TriggeredBy _cachedTriggerType;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            EnsureFields(property);

            var line = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            var height = line;

            var expanded = property.FindPropertyRelative("_expanded");
            if (!expanded.boolValue)
                return height;

            height += line * 2;
            height += _eventField.GetHeight(property) * line;
            height += _triggerField.GetHeight(property) * line;

            return height;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EnsureFields(property);

            var line = EditorGUIUtility.singleLineHeight;
            var spacing = EditorGUIUtility.standardVerticalSpacing;

            var expanded = property.FindPropertyRelative("_expanded");

            var foldRect = new Rect(position.x, position.y, position.width, line);
            expanded.boolValue = EditorGUI.Foldout(foldRect, expanded.boolValue, label, true);

            if (!expanded.boolValue)
                return;

            EditorGUI.BeginProperty(position, label, property);

            var y = foldRect.y + line + spacing;

            DrawProperty(ref y, position, property, "type");
            DrawProperty(ref y, position, property, "triggeredBy");
            foreach (var f in _eventField.GetFields())
                DrawProperty(ref y, position, property, f);

            foreach (var f in _triggerField.GetFields())
                DrawProperty(ref y, position, property, f);

            EditorGUI.EndProperty();
        }

        private void DrawProperty(ref float y, Rect totalRect, SerializedProperty parent, string propName)
        {
            var sub = parent.FindPropertyRelative(propName);
            var h = EditorGUI.GetPropertyHeight(sub, true);
            var r = new Rect(totalRect.x, y, totalRect.width, h);
            EditorGUI.PropertyField(r, sub, true);
            y += h + EditorGUIUtility.standardVerticalSpacing;
        }

        private void EnsureFields(SerializedProperty property)
        {
            var typeProp = property.FindPropertyRelative("type");
            var trigProp = property.FindPropertyRelative("triggeredBy");

            var currentEventType = (GameplayEventType)typeProp.enumValueIndex;
            var currentTriggerType = (TriggeredBy)trigProp.enumValueIndex;

            if (_eventField == null || _cachedEventType != currentEventType)
            {
                _cachedEventType = currentEventType;
                _eventField = CreateEventField(currentEventType);
            }

            if (_triggerField == null || _cachedTriggerType != currentTriggerType)
            {
                _cachedTriggerType = currentTriggerType;
                _triggerField = CreateTriggerField(currentTriggerType);
            }
        }

        private static ISpecialField CreateEventField(GameplayEventType type)
        {
            return type switch
            {
                GameplayEventType.BooleanChange => new BooleanChangeSpecialField(),
                GameplayEventType.SkyboxChange => new SkyboxChangeSpecialField(),
                GameplayEventType.Cutscene => new CutsceneSpecialField(),
                GameplayEventType.Dialogue => new AudioFragmentSpecialField(),
                GameplayEventType.ProgressToNextActivity => new EmptyField(),
                GameplayEventType.InvokeCustomEvent => new EventToInvokeSpecialField(),
                _ => new EmptyField()
            };
        }

        private static ISpecialField CreateTriggerField(TriggeredBy type)
        {
            return type switch
            {
                TriggeredBy.StartOfActivity => new EmptyField(),
                TriggeredBy.AfterSetTime => new AfterSetTimeSpecialField(),
                TriggeredBy.AfterFinishActivity => new EmptyField(),
                TriggeredBy.OnChoresCompleted => new OnChoresCompletedSpecialField(),
                TriggeredBy.BooleansToTrue => new BooleansToTrueSpecialField(),
                _ => new EmptyField()
            };
        }
    }
}
