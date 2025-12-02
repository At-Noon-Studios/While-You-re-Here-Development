using System;
using UnityEditor;
using UnityEngine;


namespace gamestate
{ 
    public interface ISpecialField
    {
        public string[] GetFields();
        public int GetHeight(SerializedProperty property);
    }

    public class BooleanChangeSpecialField : ISpecialField
    {

        public string[] GetFields()
        {
            var returnFields = new string[2];
            returnFields[0] = "booleanToChange";
            returnFields[1] = "newValue";
            return returnFields;
        }

        public int GetHeight(SerializedProperty property)
        {
            return 3;
        }
    }
    
    public class SkyboxChangeSpecialField : ISpecialField
    {
        public string[] GetFields()
        {
            var returnFields = new string[1];
            returnFields[0] = "hourOfDay";
            return returnFields;
        }

        public int GetHeight(SerializedProperty property)
        {
            return 2;
        }
    }

    public class CutsceneSpecialField : ISpecialField
    {
        public string[] GetFields()
        {
            var returnFields = new string[1];
            returnFields[0] = "cutsceneToPlay";
            return returnFields;
        }

        public int GetHeight(SerializedProperty property)
        {
            return 2;
        }
    }

    public class AudioFragmentSpecialField : ISpecialField
    {
        public string[] GetFields()
        {
            var returnFields = new string[1];
            returnFields[0] = "audioToPlay";
            return returnFields;        
        }

        public int GetHeight(SerializedProperty property)
        {
            return 2;
        }
    }

    public class EmptyField : ISpecialField
    {
        public string[] GetFields()
        {
            return Array.Empty<string>();
        }

        public int GetHeight(SerializedProperty property)
        {
            return 0;
        }
    }

    public class EventToInvokeSpecialField : ISpecialField
    { 
        public string[] GetFields()
        {
            var returnFields = new string[1];
            returnFields[0] = "eventToInvoke";
            return returnFields;
        }

        public int GetHeight(SerializedProperty property)
        {
            var height = 7;
            var serializedProperty = property.FindPropertyRelative("eventToInvoke.m_PersistentCalls.m_Calls");
            height += Mathf.Max(serializedProperty.arraySize - 1, 0) * 3;
            return height;
        }
    }

    public class AfterSetTimeSpecialField : ISpecialField
    {
        public string[] GetFields()
        {
            var returnFields = new string[1];
            returnFields[0] = "triggerAfterSeconds";
            return returnFields;
            
        }

        public int GetHeight(SerializedProperty property)
        {
            return 1;
        }
    }

    public class OnChoresCompletedSpecialField : ISpecialField
    {
        public string[] GetFields()
        {
            var returnFields = new string[1];
            returnFields[0] = "choresToComplete";
            return returnFields;
        }

        public int GetHeight(SerializedProperty property)
        {
            var height = 1;
            var serializedProperty = property.FindPropertyRelative("choresToComplete");
            if (serializedProperty.isExpanded) height += Mathf.Max(2 + serializedProperty.arraySize, 0);
            return height;
        }
    }

    public class BooleansToTrueSpecialField : ISpecialField
    {
        public string[] GetFields()
        {
            var returnFields = new string[1];
            returnFields[0] = "booleansToBeTrue";
            return returnFields;
        }

        public int GetHeight(SerializedProperty property)
        {
            var height = 1;
            var serializedProperty = property.FindPropertyRelative("booleansToBeTrue");
            if (serializedProperty.isExpanded) height += Mathf.Max(2 + serializedProperty.arraySize, 0);
            return height;
        }
    }
}