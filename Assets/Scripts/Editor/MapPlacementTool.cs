using System.IO;
using Creature;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using Object = Creature.Object;

[InitializeOnLoad]
public static class MapPlacementTool
{
    private static string[] prefabFolders = new[]
    {
        "Assets/2_Resource/Prefabs/Objects",
        "Assets/2_Resource/Prefabs/Characters", 
    };
    
    static MapPlacementTool()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }

    private static void OnSceneGUI(SceneView sceneView)
    {
        Event e = Event.current;

        if (e.type == EventType.ContextClick)
        {
            var prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
            if (prefabStage == null)
                return; // Prefab 모드가 아닐 때 무시

            var region = prefabStage.prefabContentsRoot.GetComponent<Map.Region>();
            if (region == null)
                return;
            
            Vector2 mousePos = e.mousePosition;
            Ray ray = HandleUtility.GUIPointToWorldRay(mousePos);
            Vector3 spawnPosition = Vector3.zero;

            if (Physics.Raycast(ray, out RaycastHit hit))
                spawnPosition = hit.point;
            else
                spawnPosition = ray.origin + ray.direction * 10f; // 디폴트 위치

            spawnPosition.z = 0;

            GenericMenu menu = new GenericMenu();

            // 프리팹 리스트 불러오기
            string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab", prefabFolders);
            if (prefabGuids.Length == 0)
            {
                menu.AddDisabledItem(new GUIContent("No prefabs found in specified folders"));
            }
            else
            {
                foreach (string guid in prefabGuids)
                {
                    string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                    string prefabName = Path.GetFileNameWithoutExtension(assetPath);
                    GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);

                    // 경로 기반으로 서브메뉴 만들기
                    string relativePath = GetRelativePath(assetPath);
                    string menuPath = $"Place Prefab/{relativePath}/{prefabName}";

                    menu.AddItem(new GUIContent(menuPath), false, () =>
                    {
                        PlacePrefab(prefab, spawnPosition);
                    });
                }
            }

            menu.ShowAsContext();
            e.Use(); // 기본 우클릭 메뉴 막기
        }
    }
    
    static void PlacePrefab(GameObject prefab, Vector3 position)
    {
        var prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
        if (prefabStage == null)
            return; 
        
        if (prefab == null) 
            return;

        GameObject gameObj = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
        if (gameObj == null)
            return;
        
        gameObj.transform.SetParent(prefabStage.prefabContentsRoot.transform);
        gameObj.transform.position = position;
        
        var obj = gameObj.GetComponent<Object>();
        if(obj != null)
            obj.SortingOrder(-position.y);
        
        var character = gameObj.GetComponent<Character>();
        if(character != null)
            character.SortingOrder(gameObj.transform.position.y);

        Undo.RegisterCreatedObjectUndo(gameObj, "Place Prefab");
        Selection.activeGameObject = gameObj;
        EditorGUIUtility.PingObject(gameObj);
    }
    
    static string GetRelativePath(string fullAssetPath)
    {
        foreach (string folder in prefabFolders)
        {
            if (fullAssetPath.StartsWith(folder))
            {
                string relative = fullAssetPath.Substring(folder.Length).TrimStart('/');
                string directory = Path.GetDirectoryName(relative);
                return directory?.Replace("\\", "/") ?? "";
            }
        }
            
        return "";
    }
}

