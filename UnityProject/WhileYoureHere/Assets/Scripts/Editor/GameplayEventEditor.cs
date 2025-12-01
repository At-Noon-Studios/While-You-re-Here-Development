using gamestate;
using UnityEditor;
using UnityEngine;

namespace Editor
{
	[CustomPropertyDrawer(typeof(GameplayEvent))]
	public class GameplayEventDrawer : PropertyDrawer
	{
		private float x, y, w, h;
		private int _skipRowsForEvent;
		
		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			float h = base.GetPropertyHeight(property, label);
			int rows = 1;
			if (property.FindPropertyRelative("_expanded").boolValue)
			{
				rows += 4;
				GameplayEventType gameplayEventType = (GameplayEventType)property.FindPropertyRelative("type").enumValueIndex;
				if (gameplayEventType == GameplayEventType.BooleanChange) rows += 3;
				else if (gameplayEventType is GameplayEventType.SkyboxChange or GameplayEventType.Cutscene or GameplayEventType.Dialogue) rows += 2;
				else if (gameplayEventType is GameplayEventType.InvokeCustomEvent)
				{
					rows += 7;
					_skipRowsForEvent = 5;
					var serializedProperty = property.FindPropertyRelative("eventToInvoke.m_PersistentCalls.m_Calls");
					rows += Mathf.Max(serializedProperty.arraySize -1, 0) * 3;
					_skipRowsForEvent += Mathf.Max(serializedProperty.arraySize -1, 0) * 3;	
				}
				
				TriggeredBy trigger =  (TriggeredBy)property.FindPropertyRelative("triggeredBy").enumValueIndex;
				if (trigger is TriggeredBy.AfterSetTime) rows += 1;
				else if (trigger is TriggeredBy.OnChoresCompleted)
				{
					rows += 1;
					var serializedProperty = property.FindPropertyRelative("choresToComplete");
					if (serializedProperty.isExpanded) rows += Mathf.Max(2 + serializedProperty.arraySize, 0);
				} else if (trigger is TriggeredBy.BooleansToTrue)
				{
					rows += 1;
					var serializedProperty = property.FindPropertyRelative("booleansToBeTrue");
					if (serializedProperty.isExpanded) rows += Mathf.Max(2 + serializedProperty.arraySize, 0);
				}
			}

			return h * rows;
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var pExpanded = property.FindPropertyRelative("_expanded");
			h = base.GetPropertyHeight(property, label);
			position.height = h;
			pExpanded.boolValue = EditorGUI.Foldout(position, pExpanded.boolValue, label);
			if (!pExpanded.boolValue) return;
			position.y += h;
			position.height = GetPropertyHeight(property, label) - h;
			EditorGUI.BeginProperty(position, label, property);
			
			x = position.x;
			y = position.y;
			w = position.width;

			AddField("type", property);
			AddField("triggeredBy", property);
			y += h;

			var type = property.FindPropertyRelative("type");
			var triggeredBy = property.FindPropertyRelative("triggeredBy");
			
			var gameplayEventType = (GameplayEventType)type.enumValueIndex;
			switch (gameplayEventType)
			{
				case GameplayEventType.BooleanChange:
					AddField("booleanToChange", property); 
					AddField("newValue", property);
					break;
				case GameplayEventType.SkyboxChange:
					AddField("hourOfDay", property);
					break;
				case GameplayEventType.InvokeCustomEvent:
					AddField("eventToInvoke", property);
					y += h * _skipRowsForEvent;
					break;
				case GameplayEventType.Dialogue:
					AddField("dialogueToPlay", property);
					break;
				case GameplayEventType.Cutscene:
					AddField("cutsceneToPlay", property);
					break;
			}
			var trigger = (TriggeredBy)triggeredBy.enumValueIndex;
			switch (trigger)
			{
				case TriggeredBy.AfterSetTime:
					AddField("triggerAfterSeconds", property);
					break;
				case TriggeredBy.OnChoresCompleted:
					AddField("choresToComplete", property);
					break;
				case TriggeredBy.BooleansToTrue:
					AddField("booleansToBeTrue", property);
					break;
			}
			
			EditorGUI.EndProperty();
		}

		private void AddField(string propertyName, SerializedProperty property)
		{
			y += h;
			var rect = new Rect(x, y, w, h);
			var serializedProperty = property.FindPropertyRelative(propertyName);

			EditorGUI.PropertyField(rect, serializedProperty, new GUIContent(serializedProperty.displayName));
		}
	}
}
