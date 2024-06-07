using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;

public class TestRead : MonoBehaviour
{
    public TextAsset testTxt;

    private void Start()
    {
        string path = Application.streamingAssetsPath + "/test01.txt";
        using(FileStream fs=File.OpenRead(path))
        {
            byte[] bytes = new byte[fs.Length];
            fs.Read(bytes, 0, bytes.Length);
            fs.Close();
            string str = Encoding.UTF8.GetString(bytes);
            Debug.Log(str);
        }
      
    }



}
