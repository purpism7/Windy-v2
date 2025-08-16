using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

using Cysharp.Threading.Tasks;
using Spine;
using Spine.Unity;

using Table;
using GameSystem;

public static class Extensions
{
    public static T AddOrGetComponent<T>(this Component component) where T : Component
    {
        if (!component)
            return default;
            
        var t = component.GetComponent<T>();
        if (t == null)
            t = component.gameObject.AddComponent<T>();

        return t;
    }

    public static void SetActive(this Component component, bool active)
    {
        if (component == null)
            return;

        SetActiveAsync(component, active).Forget();
    }

    private static async UniTask SetActiveAsync(this Component component, bool active)
    {
        await UniTask.Yield();
            
        component.gameObject.SetActive(active);
    }
        
    public static List<T> AddList<T, V>(this V[] arrays) where T : class
    {
        if (arrays == null)
            return null;
            
        var list = new List<T>();
        list.Clear();
            
        foreach (V t in arrays)
        {
            if(t == null)
                continue;
                
            list.Add(t as T);
        }

        return list;
    }

    public static bool IsNullOrEmpty<T>(this List<T> list)
    {
        if (list == null)
            return true;

        if (list.Count <= 0)
            return true;

        return false;
    }
    
    public static bool IsNullOrEmpty<T>(this T[] array)
    {
        if (array == null)
            return true;
        
        if (array.Length <= 0)
            return true;

        return false;
    }
    
    public static bool IsNullOrEmpty<T>(this HashSet<T> hashSet)
    {
        if (hashSet == null)
            return true;

        if (hashSet.Count <= 0)
            return true;

        return false;
    }
    
    public static bool IsNullOrEmpty<T, V>(this Dictionary<T, V> dic)
    {
        if (dic == null)
            return true;

        if (dic.Count <= 0)
            return true;

        return false;
    }
    
    public static void RemoveAllChild(this Transform tm)
    {
        if (!tm)
            return;

        for (int i = tm.childCount - 1; i >= 0; --i)
        {
            Transform child = tm.GetChild(i);
            child.Remove();
        }
    }

    public static void Remove(this Transform tm)
    {
        if (!tm)
            return;
        
        if (Application.isPlaying)
            GameObject.Destroy(tm.gameObject); // 안전한 런타임 제거
        else
            GameObject.DestroyImmediate(tm.gameObject); // 에디터에서 즉시 제거
    }

    public static void Initialize(this Transform tm)
    {
        if (!tm)
            return;
        
        tm.position = Vector3.zero;
        tm.rotation = Quaternion.identity;
        tm.localScale = Vector3.one;
    }
    
    #region Localization

    public static string GetLocalization(this int localId)
    {
        var localData = LocalDataContainer.Instance.GetData(localId);
        if (localData == null)
            return string.Empty;

        return localData.Ko;
    }

    public static string GetLocalization(this string key)
    {
        return LocalDataContainer.Instance?.GetLocalization(key);
    }
    #endregion
    
    #region Spine Animation

    public static void SetAnimation(this SkeletonAnimation skeletonAnimation, string animationName, bool loop, System.Action<TrackEntry> completeAction = null)
    {
        if (skeletonAnimation == null)
            return;
            
        var animationState = skeletonAnimation.AnimationState;
        if (animationState == null)
            return;
            
        var animation = skeletonAnimation.skeletonDataAsset?.GetSkeletonData(true)?.Animations?
            .Find(animation => animation.Name.Contains(animationName));
        if (animation == null)
            return;
        
        var trackEntry = animationState.SetAnimation(0, animationName, loop);
        if (trackEntry == null)
            return;

        trackEntry.Complete += trackEntry => { completeAction?.Invoke(trackEntry); };
    }
    #endregion
    
    #region File
    public static void CreateLocalFile(string filePath, string fileName)
    {
        if (string.IsNullOrEmpty(filePath))
            return;

        if (!Directory.Exists(filePath))
            Directory.CreateDirectory(filePath);
            
        var fullFilePath = Path.Combine(filePath, fileName);
        if (File.Exists(fullFilePath))
            return;
            
        Debug.Log("Local fullPath = " + fullFilePath);
        File.Create(fullFilePath);
    }
    #endregion
    
    #region UI

    public static void ScreenPointToLocalPointInRectangle(this RectTransform rectTm, Transform targetTm)
    {
        if (!rectTm)
            return;

        if (!targetTm)
            return;
        
        var parent = rectTm.parent as RectTransform;
        if (!parent)
            return;

        var targetWorldPos = targetTm.position;
        Vector3 localPos = parent.InverseTransformPoint(targetWorldPos);
        //rectTm.localPosition = localPos;
        rectTm.anchoredPosition = localPos;
        // Vector2 localPoint;
        // RectTransformUtility.ScreenPointToLocalPointInRectangle(
        //     parent,
        //     RectTransformUtility.WorldToScreenPoint(null, targetTm.position),
        //     null,
        //     out localPoint
        // );
        //
        // rectTm.anchoredPosition = localPoint;
    }
    #endregion
}

