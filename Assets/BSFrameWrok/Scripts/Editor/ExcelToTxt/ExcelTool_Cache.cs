using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ExcelTool_Cache : ScriptableObject
{
    internal const string DEFAULT_CACHE_PATH = "Assets/Editor/ExcelTool_Cache.asset";//后缀名必须是asset，否则unity无法识别
    internal static ExcelTool_Cache _cache;

    public static ExcelTool_Cache Cache
    {
        get
        {
            if (_cache == null) GetOrCreatCache();
            return _cache;
        }
    }

    public static void GetOrCreatCache(bool showDialog=false)
    {
        var cache = AssetDatabase.LoadAssetAtPath<ExcelTool_Cache>(DEFAULT_CACHE_PATH);
        if(cache!=null)
        {
            _cache = cache;
            if(showDialog) UnityEditor.EditorUtility.DisplayDialog("尝试重复", $"请检查{DEFAULT_CACHE_PATH}", "确认", "取消");
            return;
        }
        _cache = ScriptableObject.CreateInstance<ExcelTool_Cache>();
        AssetDatabase.CreateAsset(_cache, DEFAULT_CACHE_PATH);
        EditorUtility.SetDirty(_cache);
        AssetDatabase.SaveAssets();
    }

    public static void SaveCache()
    {
        if (_cache == null) return;
        EditorUtility.SetDirty(_cache);//将对象标记为脏，Unity就知道要在AssetDatabase.SaveAssets()中将当前状态保存到磁盘
        AssetDatabase.SaveAssets();
    }

    public string outPath;

    //public List<ExcelPathInfo> ExcelInfo = new List<ExcelPathInfo>();

    //[System.Serializable]
    //public class ExcelPathInfo
    //{
    //    public string outPath;
    //}
}
