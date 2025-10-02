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

        var isAutoStartProp = serializedObject.FindProperty("isAutoStart");
        EditorGUILayout.PropertyField(isAutoStartProp);

        var targetTmProp = serializedObject.FindProperty("targetTm");
        EditorGUILayout.PropertyField(targetTmProp);

        var visibilityTypeProp = serializedObject.FindProperty("visibilityType");
        EditorGUILayout.PropertyField(visibilityTypeProp);

        EditorGUILayout.Space();

        conditionsProp.arraySize = EditorGUILayout.IntField("Conditions Size", conditionsProp.arraySize);

        for (int i = 0; i < conditionsProp.arraySize; i++)
        {
            //EditorGUILayout.BeginVertical("box");
            var element = conditionsProp.GetArrayElementAtIndex(i);
            var visibilityCondition = element.FindPropertyRelative("VisibilityCondition");
            
            EditorGUILayout.PropertyField(visibilityCondition);

            SerializedProperty visibilityPhase = null;
            if ((VisibilityCondition)visibilityCondition.enumValueIndex != VisibilityCondition.None)
            {
                visibilityPhase = element.FindPropertyRelative("VisibilityPhase");
                EditorGUILayout.PropertyField(visibilityPhase);
            }

            switch ((VisibilityCondition)visibilityCondition.enumValueIndex)
            {
                case VisibilityCondition.Quest:
                    {
                        var contentName = "Group";

                        EditorGUILayout.BeginVertical("box");
                        if ((VisibilityPhase)visibilityPhase.enumValueIndex == VisibilityPhase.During)
                        {
                            contentName = "From";
                            EditorGUILayout.LabelField("Group");
                            EditorGUILayout.BeginHorizontal();
                        }

                        EditorGUILayout.PropertyField(element.FindPropertyRelative("QuestGroup"), new GUIContent(contentName), GUILayout.MinWidth(100));

                        contentName = "Step";
                        if ((VisibilityPhase)visibilityPhase.enumValueIndex == VisibilityPhase.During)
                        {
                            contentName = "From";

                            EditorGUILayout.Space();
                            EditorGUILayout.PropertyField(element.FindPropertyRelative("ToQuestGroup"), new GUIContent("To"), GUILayout.MinWidth(100));
                            EditorGUILayout.EndHorizontal();
                            EditorGUILayout.EndVertical();

                            EditorGUILayout.BeginVertical("box");
                            EditorGUILayout.LabelField("Step");
                            EditorGUILayout.BeginHorizontal();
                        }

                        EditorGUILayout.PropertyField(element.FindPropertyRelative("QuestStep"), new GUIContent(contentName), GUILayout.MinWidth(100));

                        if((VisibilityPhase)visibilityPhase.enumValueIndex == VisibilityPhase.During)
                        {
                            EditorGUILayout.Space();
                            EditorGUILayout.PropertyField(element.FindPropertyRelative("ToQuestStep"), new GUIContent("To"), GUILayout.MinWidth(100));
                            EditorGUILayout.EndHorizontal();
                        }

                        EditorGUILayout.EndVertical();

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
            //EditorGUILayout.Space();
        }

        serializedObject.ApplyModifiedProperties();


        EditorGUILayout.EndVertical();

    }
}
