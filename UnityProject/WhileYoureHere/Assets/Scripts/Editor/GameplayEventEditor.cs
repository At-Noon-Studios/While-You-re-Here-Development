using System;
using gamestate;
using UnityEditor;
using UnityEngine;

namespace Editor
{
	[CustomPropertyDrawer(typeof(GameplayEvent))]
	public class GameplayEventDrawer : PropertyDrawer
	{
		private float _x, _y, _width, _height;
		private ISpecialField _eventField; 
		private ISpecialField _triggerField;
		
		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			_height = base.GetPropertyHeight(property, label);
			var rows = 1;
			if (!property.FindPropertyRelative("_expanded").boolValue) return _height * rows;
			rows += 4;
			if (_eventField == null || _triggerField == null) return _height * rows;
			rows += _eventField.GetHeight(property);
			rows += _triggerField.GetHeight(property);

			return _height * rows;
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			SetEvent((GameplayEventType)property.FindPropertyRelative("type").enumValueIndex);
			SetTrigger((TriggeredBy)property.FindPropertyRelative("triggeredBy").enumValueIndex);

			var pExpanded = property.FindPropertyRelative("_expanded");
			_height = base.GetPropertyHeight(property, label);
			position.height = _height;
			pExpanded.boolValue = EditorGUI.Foldout(position, pExpanded.boolValue, label);
			if (!pExpanded.boolValue) return;
			position.y += _height;
			position.height = GetPropertyHeight(property, label) - _height;
			EditorGUI.BeginProperty(position, label, property);

			_x = position.x;
			_y = position.y;
			_width = position.width;

			AddField("type", property);
			AddField("triggeredBy", property);
			_y += _height;
			
			foreach (var field in _eventField.GetFields())
			{
				AddField(field, property);
			}
			if (_eventField.GetType() ==  typeof(EventToInvokeSpecialField)) _y += _height * 5;
			foreach (var field in _triggerField.GetFields())
			{
				AddField(field, property);
			}

			EditorGUI.EndProperty();
		}

		private void SetEvent(GameplayEventType gameplayEventType)
		{
			_eventField = gameplayEventType switch
			{
				GameplayEventType.BooleanChange => new BooleanChangeSpecialField(),
				GameplayEventType.SkyboxChange => new SkyboxChangeSpecialField(),
				GameplayEventType.Cutscene => new CutsceneSpecialField(),
				GameplayEventType.Dialogue => new AudioFragmentSpecialField(),
				GameplayEventType.ProgressToNextActivity => new EmptyField(),
				GameplayEventType.InvokeCustomEvent => new EventToInvokeSpecialField(),
				_ => throw new ArgumentOutOfRangeException(nameof(gameplayEventType), gameplayEventType, null)
			};
		}

		private void SetTrigger(TriggeredBy triggeredBy)
		{
			_triggerField = triggeredBy switch
			{
				TriggeredBy.StartOfActivity => new EmptyField(),
				TriggeredBy.AfterSetTime => new AfterSetTimeSpecialField(),
				TriggeredBy.AfterFinishActivity => new EmptyField(),
				TriggeredBy.OnChoresCompleted => new OnChoresCompletedSpecialField(),
				TriggeredBy.BooleansToTrue => new BooleansToTrueSpecialField(),
				_ => throw new ArgumentOutOfRangeException(nameof(triggeredBy), triggeredBy, null)
			};
		}

		private void AddField(string propertyName, SerializedProperty property)
		{
			_y += _height;
			var rect = new Rect(_x, _y, _width, _height);
			var serializedProperty = property.FindPropertyRelative(propertyName);
			EditorGUI.PropertyField(rect, serializedProperty, new GUIContent(serializedProperty.displayName));
		}
	}
}
