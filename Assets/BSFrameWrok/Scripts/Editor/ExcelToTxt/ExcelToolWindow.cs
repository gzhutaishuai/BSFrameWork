using System.Collections;
using System.Collections.Generic;
using Excel;
using System.Data;
using System.IO;
using UnityEngine;
using UnityEditor;

public class ExcelToolWindow : EditorWindow
{
    static ExcelToolWindow _thisInstance;

    [MenuItem("工具/Excel转Txt",false,109)]
    public static void ShowWindow()
    {

        ExcelTool_Cache.GetOrCreatCache();
        if(_thisInstance==null)
        {
            _thisInstance = EditorWindow.GetWindow(typeof(ExcelToolWindow), false,"Excel转Txt",true) as ExcelToolWindow;
            _thisInstance.minSize = new Vector2(800, 600);
        }
        _thisInstance.Show();
    }

    string pathInfo;

    public static void CloseWindow()
    {
        _thisInstance?.Close();
    }

    private void CreateGUI()
    {
        pathInfo = GetExcelPathInfo();
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Excel路径：", GUILayout.Width(65));
        pathInfo = GUILayout.TextField(pathInfo, GUILayout.Width(400));
        if (GUILayout.Button("选择Excel表目录", GUILayout.Width(150)))
        {
            pathInfo = EditorUtility.OpenFolderPanel("选择Excel表目录", pathInfo, "");
            if(!string.IsNullOrEmpty(pathInfo))
            {
                ExcelTool_Cache.Cache.outPath = pathInfo;
                ExcelTool_Cache.SaveCache();
            }
            else
            {
                pathInfo = GetExcelPathInfo();
            }
        }
        if(GUILayout.Button("保存路径",EditorStyles.miniButton))
        {
            ExcelTool_Cache.Cache.outPath = pathInfo;
            ExcelTool_Cache.SaveCache();
        }
        EditorGUILayout.EndHorizontal();

        if(GUILayout.Button("转换成Txt",GUILayout.Width(200),GUILayout.Height(50)))
        {
            ExcelConvertTxt(pathInfo);
        }
        
    }

    public static void ExcelConvertTxt(string path)
    {
        bool isExistXlsx = false;
        string[] files = Directory.GetFiles(path, "*.xlsx");
        for(int i=0;i<files.Length;i++)
        {
            isExistXlsx = true;
            //确保在任何平台上文件路径都能被正确解析
            files[i] = files[i].Replace("\\", "/");//反斜杠替换成正斜杠，.Net中用\\表示一个反斜杠\
            //通过文件流读取文件
            using (FileStream fs = File.Open(files[i],FileMode.Open,FileAccess.Read))
            {
                //文件流转换为Excel对象
                var excelDataReader = ExcelReaderFactory.CreateOpenXmlReader(fs);
                //获取excel数据
                DataSet dataSet = excelDataReader.AsDataSet();

                //检查是否有一个xlsx表中是否有Sheet
                if(dataSet.Tables.Count<1)
                {
                    Debug.LogError($"There is no Sheet in {path},Plaes Check Again");
                    return;
                }

                //读取excel第一张表
                DataTable table = dataSet.Tables[0];


                // 检查一个Sheet表中是否有数据
                if(table.Rows.Count<1)
                {
                    Debug.LogError($"There is no message in {path},Plaes Check Again");
                    return;
                }

                //将表中内容 读取后 存储到对应的txt文件
                ReadTableToTxt(files[i], table);
            }
        }
        if (!isExistXlsx)
        {
            Debug.LogError($"There is no Excel in {path},Plaes Check Again");
            return;
        }
        //刷新编辑器
        AssetDatabase.Refresh();
    }

    public static void ReadTableToTxt(string filePath,DataTable table)
    {
        //获得文件名(不需要文件后缀 生成与之名字相同的txt文件)
        string fileName = Path.GetFileNameWithoutExtension(filePath);
        //txt文件存储的路径
        string uPath = Application.dataPath + "/Resources/Text/" + fileName + ".txt";
        //判断该路径下是否已经存在对应的txt文件，如果是，直接删除
        if(File.Exists(uPath))
        {
            File.Delete(uPath);
        }


        //文件流创建txt文件
        using (FileStream fs = new FileStream(uPath,FileMode.Create))
        {
            //文件流写入流方便写入字符串
            using(StreamWriter sw=new StreamWriter(fs))
            {
                //遍历table
                for(int row=0;row<table.Rows.Count;++row)
                {
                    DataRow dataRow = table.Rows[row];
                    string str = "";
                    //遍历列
                    for(int col=0;col<table.Columns.Count;++col)
                    {
                        string val = dataRow[col].ToString();
                        str = str + val + "\t";
                    }

                    //写入
                    sw.Write(str);

                    //如果不是最后一行
                    if(row!=table.Rows.Count-1)
                    {
                        sw.WriteLine();
                    }

                }
            }
        }
    }

    public string GetExcelPathInfo()
    {
        var path= ExcelTool_Cache.Cache.outPath;

        var result = $"{path}";

        return result;
    }

}
