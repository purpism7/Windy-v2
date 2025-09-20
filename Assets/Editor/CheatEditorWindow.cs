using NUnit.Framework.Interfaces;
using System.IO;
using GameSystem;
using UnityEditor;
using UnityEngine;

using Newtonsoft.Json;

using Table;
using GameSystem.Event;
using Network;
using Network.Api;
using Network.Api.Quest;

using Quest = GameSystem.Mission.Quest;

public class CheatEditorWindow : EditorWindow, IApiResponse<Network.Api.AddItem.Response>
{
    [MenuItem("Tools/Cheat Editor Window")]
    public static void Open() => GetWindow<CheatEditorWindow>("Cheat Editor Window");

    private string[] _tabs = { "Item", };
    private int _currentTab = 0;

    private Vector2 _scroll;

    private ItemDataContainer _itemDataContainer = null;
    private LocalDataContainer _localDataContainer = null;

    #region Quest

    private static bool _initializeQuest = false;
    private int _questGroup = 0;
    private int _questStep = 0;
    #endregion
    
    #region Item
    
    private int _itemId = 0;
    private int _itemCount = 0;
    #endregion

    private void OnEnable()
    {
        LoadItemDatas();
        LoadLocalDatas();
    }

    [InitializeOnEnterPlayMode]
    static void OnEnterPlayMode()
    {
        // 여기에 값을 초기화하거나 로직 추가
        Debug.Log("▶ 에디터에서 Play 모드 진입! 값 갱신 실행됨");
        
        GameSystem.Event.EventDispatcher.Register<GameSystem.Event.ChangeQuest>(OnChanged);
        
        _initializeQuest = false;
        
    }

    void OnGUI()
    {
        using (new EditorGUILayout.VerticalScope("helpbox"))
        {
            // DrawTabs();
            // EditorGUILayout.Space(5);
            DrawQuestView();
            DrawItemView();
        }
        EditorGUILayout.Space(2);
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

    private void LoadItemDatas()
    {
        var filePath = "Assets/3_Table/Item.json";
        LoadDatas(filePath, ref _itemDataContainer);
    }

    private void LoadLocalDatas()
    {
        var filePath = "Assets/3_Table/Local.json";
        LoadDatas(filePath, ref _localDataContainer);
    }

    private GUIStyle LabelGUIStyle
    {
        get
        {
            GUIStyle wrapStyle = new GUIStyle(EditorStyles.label);
            wrapStyle.richText = true;
            wrapStyle.wordWrap = true;

            return wrapStyle;
        }
    }

    private void DrawTabs()
    {
        var tab = GUILayout.Toolbar(_currentTab, _tabs, GUILayout.Height(30f));
        if (_currentTab != tab)
        {
            _currentTab = tab;

            _scroll.y = 0;
        }
    }
    #region Quest

    private void DrawQuestView()
    {
        if (!_initializeQuest)
        {
            var currentQuestData = Manager.Get<IMission>()?.Quest?.CurrentQuestData;
            if (currentQuestData != null)
            {
                _questGroup = currentQuestData.Group;
                _questStep = currentQuestData.Step;

                _initializeQuest = true;
            }
        }
        
        EditorGUILayout.BeginVertical("helpbox");
        
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Change"))
            {
                var questData = QuestDataContainer.Instance?.GetData(_questGroup, _questStep);
                if (questData == null)
                {
                    ShowNotification(new GUIContent("No quest data found"));
                    return;
                }

                var quest = Manager.Get<IMission>()?.Quest;
                if (quest != null)
                {
                    quest.SetNextQuest(_questGroup, _questStep);
                    RequestSaveQuest(quest.CurrentQuestData.Group, quest.CurrentQuestData.Step);
                }
            }
            
            EditorGUILayout.Space(20f);
            _questGroup = EditorGUILayout.IntField("Quest Group", _questGroup);
            EditorGUILayout.Space(20f);
            _questStep = EditorGUILayout.IntField("Quest Step", _questStep);
            
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(3f);
            
        }
        
        EditorGUILayout.EndVertical();
    }
    
    private void RequestSaveQuest(int questGroup, int questStep)
    {
        var request = SaveQuest.CreateRequest<SaveQuest>(
            new SaveQuest.Request
            {
                QuestGroup = questGroup,
                QuestStep = questStep,
            });
                    
        ApiClient.Instance?.RequestPost(request);
    }

    private static void OnChanged(GameSystem.Event.ChangeQuest changeQuest)
    {
        _initializeQuest = false;
    }
    #endregion

    #region Item
    private void DrawItemView()
    {
        EditorGUILayout.BeginVertical("helpbox");

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Add"))
        {
            RequestAddItem(_itemId, _itemCount);
        }

        string resItemName = string.Empty;
        var itemData = _itemDataContainer?.GetData(_itemId);
        if (itemData != null)
        {
            var itemName = _localDataContainer?.GetLocalization(itemData.LocalKey);
            resItemName = $"<color=green>{itemName}</color>";
        }
        else
        {
            if(_itemId > 0)
                resItemName = "<color=red>Not Found Item</color>";
        }

        EditorGUILayout.Space(20f);
        EditorGUILayout.LabelField(resItemName, LabelGUIStyle);

        _itemId = EditorGUILayout.IntField("Item Id", _itemId);
        EditorGUILayout.Space(20);
        _itemCount = EditorGUILayout.IntField("Item Count", _itemCount);

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space(3f);

        EditorGUILayout.EndVertical();
    }

    private void RequestAddItem(int itemId, int itemCount)
    {
        if (itemId <= 0)
            return;

        if (itemCount <= 0)
            return;

        var request = Network.Api.AddItem.CreateRequest<Network.Api.AddItem>(
            new Network.Api.AddItem.Request
            {
                ItemId = itemId,
                ItemCount = itemCount,
            });

        ApiClient.Instance?.RequestPost(request, this);
    }

    void IApiResponse<Network.Api.AddItem.Response>.OnResponse(Network.Api.AddItem.Response data, bool isSuccess, string errorMessage)
    {
        if (isSuccess)
            GameSystem.Event.EventDispatcher.Dispatch<Item>(new GameSystem.Event.AddItem(data.ItemId, data.ItemCount));

        _itemId = 0;
        _itemCount = 0;
    }
    #endregion
}
