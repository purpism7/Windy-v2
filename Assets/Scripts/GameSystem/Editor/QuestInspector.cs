using System;
using GameSystem.Mission;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Quest))]
public class QuestInspector : Editor
{
    private int _questGroup = 0;
    private int _questSetp = 0;
    
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI(); 
        
        var quest = target as Quest;
        var currentQuestData = quest?.CurrentQuestData;
        if (currentQuestData == null)
            return;

        EditorGUILayout.BeginHorizontal("box");
        EditorGUILayout.LabelField($"Group = {currentQuestData.Group}");
        EditorGUILayout.LabelField($"Step = {currentQuestData.Step}");
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.BeginHorizontal("box");
        _questGroup = EditorGUILayout.IntField($"Group", _questGroup);
        _questSetp = EditorGUILayout.IntField($"Step", _questSetp);
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("Change Quest"))
        {
            quest.SetNextQuest(_questGroup, _questSetp);
            quest.RequestSaveQuest(quest.CurrentQuestData.Group, quest.CurrentQuestData.Step);
        }
    }
}
