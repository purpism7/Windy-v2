using System;
using GameSystem.Mission;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Quest))]
public class QuestInspector : Editor
{
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
    }
}
