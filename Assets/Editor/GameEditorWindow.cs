using System.Collections.Generic;
using Table;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using static Codice.CM.Common.CmCallContext;
using static UnityEngine.Rendering.DebugUI;

[System.Serializable]
public class QuestDataList
{
    public List<QuestData> QuestList = new();
}

public class GameEditorWindow : EditorWindow
{
    #region Tabs
    private string[] _tabs = { "Quest", "Talk", "Local" };
    private int _currentTab = 0;
    #endregion

    #region Quest
    private QuestDataList _questDataList = new();
    private Vector2 _scroll;
    #endregion

    [MenuItem("Tools/Game Editor Window")]
    public static void Open() => GetWindow<GameEditorWindow>("Game Editor Window");

    private void OnEnable()
    {
        //_graphViewEx = new GraphViewEx();
        //_graphViewEx.StretchToParentSize();
        //rootVisualElement.Add(_graphViewEx);
    }

    void OnGUI()
    {
        _currentTab = GUILayout.Toolbar(_currentTab, _tabs);

        EditorGUILayout.Space(10);

        switch (_currentTab)
        {
            case 0: 
                DrawQuestView();
                break;

            case 1: 
                DrawTalkView();
                break;

            case 2:
                DrawLocalView();
                break;
        }
    }

    private void DrawQuestView()
    {
        EditorGUILayout.Space();
        if (GUILayout.Button("Add Quest", GUILayout.Height(22)))
        {
            _questDataList.QuestList.Add(new QuestData());
        }

        //if (GUILayout.Button("Save to JSON", GUILayout.Height(22)))
        //{
        //    QuestDataIO.Save(questDataList);
        //}

        EditorGUILayout.Space();
        _scroll = EditorGUILayout.BeginScrollView(_scroll);

        for (int i = 0; i < _questDataList?.QuestList?.Count; i++)
        {
            var q = _questDataList?.QuestList[i];
            using (new EditorGUILayout.VerticalScope("box"))
            {
                //q.id = EditorGUILayout.IntField("ID", q.id);
                //q.code = EditorGUILayout.TextField("Code", q.code);
                //q.title = EditorGUILayout.TextField("Title", q.title);
                //q.description = EditorGUILayout.TextArea(q.description, GUILayout.Height(60));

                if (GUILayout.Button("Delete")) { _questDataList.QuestList.RemoveAt(i); break; }
            }
        }

        EditorGUILayout.EndScrollView();
    }

    private void DrawTalkView()
    {
        GUILayout.Label("Text 관련 UI", EditorStyles.boldLabel);
        if (GUILayout.Button("Edit Text")) Debug.Log("텍스트 편집!");
    }

    private void DrawLocalView()
    {
        GUILayout.Label("Text 관련 UI", EditorStyles.boldLabel);
        if (GUILayout.Button("Edit Text")) Debug.Log("텍스트 편집!");
    }
}
