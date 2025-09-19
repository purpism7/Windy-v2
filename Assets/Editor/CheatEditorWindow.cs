using NUnit.Framework.Interfaces;
using System.IO;
using UnityEditor;
using UnityEngine;

using Newtonsoft.Json;

using Table;
using GameSystem.Event;
using Network;
using Network.Api;

public class CheatEditorWindow : EditorWindow, IApiResponse<Network.Api.AddItem.Response>
{
    [MenuItem("Tools/Cheat Editor Window")]
    public static void Open() => GetWindow<CheatEditorWindow>("Cheat Editor Window");

    private string[] _tabs = { "Item", };
    private int _currentTab = 0;

    private Vector2 _scroll;

    private ItemDataContainer _itemDataContainer = null;
    private LocalDataContainer _localDataContainer = null;

    #region Item
    
    private int _itemId = 0;
    private int _itemCount = 0;
    #endregion

    private void OnEnable()
    {
        LoadItemDatas();
        LoadLocalDatas();
    }

    void OnGUI()
    {
        using (new EditorGUILayout.VerticalScope("helpbox"))
        {
            DrawTabs();
            EditorGUILayout.Space(5);

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

    private void DrawItemView()
    {
        EditorGUILayout.BeginVertical("helpbox");

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Add"))
        {
            RequestAddItem(_itemId, _itemCount);
        }

        //

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
        EditorGUILayout.LabelField(resItemName, LabelGUIStyle, GUILayout.Width(300f));

        _itemId = EditorGUILayout.IntField("Item Id", _itemId);
        EditorGUILayout.Space(20);
        _itemCount = EditorGUILayout.IntField("Item Count", _itemCount);

        EditorGUILayout.EndHorizontal();

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
}
