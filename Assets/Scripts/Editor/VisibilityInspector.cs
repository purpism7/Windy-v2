using Common;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Visibility))]
public class VisibilityInspector : Editor
{
    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();

        //var visibility = target as Visibility;
        //if (visibility == null)
        //    return;

        EditorGUILayout.BeginVertical();
        serializedObject.Update();

        SerializedProperty conditionsProp = serializedObject.FindProperty("conditions");
        //EditorGUILayout.PropertyField(conditionsProp, true);


        conditionsProp.arraySize = EditorGUILayout.IntField("Conditions Size", conditionsProp.arraySize);
        EditorGUILayout.Space();

        for (int i = 0; i < conditionsProp.arraySize; i++)
        {
            //EditorGUILayout.BeginVertical("box");
            var element = conditionsProp.GetArrayElementAtIndex(i);
            var visibilityCondition = element.FindPropertyRelative("VisibilityCondition");
            
            EditorGUILayout.PropertyField(visibilityCondition);

            if ((VisibilityCondition)visibilityCondition.enumValueIndex != VisibilityCondition.None)
            {
                var visibility = element.FindPropertyRelative("Visibility");
                var visibilityPhase = element.FindPropertyRelative("VisibilityPhase");

                EditorGUILayout.PropertyField(visibility);
                EditorGUILayout.PropertyField(visibilityPhase);
            }

            switch ((VisibilityCondition)visibilityCondition.enumValueIndex)
            {
                case VisibilityCondition.Quest:
                    {
                        EditorGUILayout.PropertyField(element.FindPropertyRelative("QuestGroup"));
                        EditorGUILayout.PropertyField(element.FindPropertyRelative("QuestStep"));

                        break;
                    }

                case VisibilityCondition.Weather:
                    {
                        EditorGUILayout.PropertyField(element.FindPropertyRelative("Weather"));

                        break;
                    }
                  
                case VisibilityCondition.TimeOfDay:
                    {
                        EditorGUILayout.PropertyField(element.FindPropertyRelative("TimeOfDay"));

                        break;
                    }
            }

            //EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
        }

        serializedObject.ApplyModifiedProperties();


        EditorGUILayout.EndVertical();

    }
}
