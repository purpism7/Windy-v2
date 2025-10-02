using UnityEditor;
using UnityEngine;

using System.Collections.Generic;
using System.IO;
using System.Linq;

using Newtonsoft.Json;

using Table;
using Common;

[CustomEditor(typeof(Visibility))]
public class VisibilityInspector : Editor
{
    private QuestDataContainer _questDataContainer = null;

    private int _filterQuestGroup = 0;
    private HashSet<int> _questGroupHashSet = new();
    private List<string> _questGroupDisplayedList = new();

    private int _filterQuestStep = 0;
    private HashSet<int> _questStepHashSet = new();
    private List<string> _questStepDisplayedList = new();

    private void OnEnable()
    {
        LoadQuestDatas();

        Initialize();
    }

    private void Initialize()
    {
        _questGroupHashSet?.Clear();
        _questGroupDisplayedList?.Clear();

        _questStepHashSet?.Clear();
        _questStepDisplayedList?.Clear();

        _questGroupHashSet?.Add(0);
        _questStepHashSet?.Add(0);

        for (int i = 0; i < _questDataContainer?.Datas?.Length; ++i)
        {
            var questData = _questDataContainer?.Datas[i];
            if (questData == null)
                continue;

            _questGroupHashSet?.Add(questData.Group);
            _questStepHashSet?.Add(questData.Step);
        }

        _questGroupHashSet = _questGroupHashSet?.OrderBy(_ => _).ToHashSet();
        _questStepHashSet = _questStepHashSet?.OrderBy(_ => _).ToHashSet();

        foreach (var questGroup in _questGroupHashSet)
        {
            var displayed = questGroup.ToString();
            if (questGroup == 0)
                displayed = "None";

            _questGroupDisplayedList?.Add(displayed);
        }

        foreach (var questStep in _questStepHashSet)
        {
            var displayed = questStep.ToString();
            if (questStep == 0)
                displayed = "None";

            _questStepDisplayedList?.Add(displayed);
        }
    }

    private void LoadDatas<T>(string filePath, ref T container) where T : Container
    {
        var settings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
        };

        var fileName = Path.GetFileName(filePath);
        fileName = Path.GetFileNameWithoutExtension(fileName);

        string jsonString = File.ReadAllText(filePath);

        var type = typeof(T);
        var obj = System.Activator.CreateInstance(type);
        container = obj as T;
        container?.Initialize(container, jsonString);
    }

    private void LoadQuestDatas()
    {
        var filePath = "Assets/3_Table/Quest.json";
        LoadDatas(filePath, ref _questDataContainer);
    }

    public override void OnInspectorGUI()
    {
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

                        //EditorGUILayout.PropertyField(element.FindPropertyRelative("QuestGroup"), new GUIContent(contentName), GUILayout.MinWidth(100));
                        var questGroupProp = element.FindPropertyRelative("QuestGroup");
                        questGroupProp.intValue = EditorGUILayout.IntPopup(contentName, questGroupProp.intValue, _questGroupDisplayedList?.ToArray(), _questGroupHashSet?.ToArray(), GUILayout.MinWidth(100));

                        contentName = "Step";
                        if ((VisibilityPhase)visibilityPhase.enumValueIndex == VisibilityPhase.During)
                        {
                            contentName = "From";

                            EditorGUILayout.Space();

                            var toQuestGroupProp = element.FindPropertyRelative("ToQuestGroup");
                            toQuestGroupProp.intValue = EditorGUILayout.IntPopup("To", toQuestGroupProp.intValue, _questGroupDisplayedList?.ToArray(), _questGroupHashSet?.ToArray(), GUILayout.MinWidth(100));
                            //EditorGUILayout.PropertyField(element.FindPropertyRelative("ToQuestGroup"), new GUIContent("To"), GUILayout.MinWidth(100));
                            EditorGUILayout.EndHorizontal();
                            EditorGUILayout.EndVertical();

                            EditorGUILayout.BeginVertical("box");
                            EditorGUILayout.LabelField("Step");
                            EditorGUILayout.BeginHorizontal();
                        }

                        var questStepProp = element.FindPropertyRelative("QuestStep");
                        questStepProp.intValue = EditorGUILayout.IntPopup(contentName, questStepProp.intValue, _questStepDisplayedList?.ToArray(), _questStepHashSet?.ToArray(), GUILayout.MinWidth(100));
                        //EditorGUILayout.PropertyField(element.FindPropertyRelative("QuestStep"), new GUIContent(contentName), GUILayout.MinWidth(100));

                        if((VisibilityPhase)visibilityPhase.enumValueIndex == VisibilityPhase.During)
                        {
                            EditorGUILayout.Space();

                            var toQuestStepProp = element.FindPropertyRelative("ToQuestStep");
                            toQuestStepProp.intValue = EditorGUILayout.IntPopup("To", toQuestStepProp.intValue, _questStepDisplayedList?.ToArray(), _questStepHashSet?.ToArray(), GUILayout.MinWidth(100));
                            //EditorGUILayout.PropertyField(element.FindPropertyRelative("ToQuestStep"), new GUIContent("To"), GUILayout.MinWidth(100));
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
