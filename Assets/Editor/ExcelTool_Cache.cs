using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ExcelTool_Cache : ScriptableObject
{
    internal const string DEFAULT_CACHE_PATH = "Assets/Editor/ExcelTool_Cache.asset";//��׺��������asset������unity�޷�ʶ��
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
            if(showDialog) UnityEditor.EditorUtility.DisplayDialog("�����ظ�", $"����{DEFAULT_CACHE_PATH}", "ȷ��", "ȡ��");
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
        EditorUtility.SetDirty(_cache);//��������Ϊ�࣬Unity��֪��Ҫ��AssetDatabase.SaveAssets()�н���ǰ״̬���浽����
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
