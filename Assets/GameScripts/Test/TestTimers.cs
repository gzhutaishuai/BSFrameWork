using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestTimers : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        BS.Timers.inst.Add(2, 0, TestCallBack,"ÓÄÄ¬");
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Q))
        {
            BS.Timers.inst.Remove(TestCallBack);
        }
    }

    void TestCallBack(object param)
   {
        string t = (string)param;
        Debug.Log("2 Seconds Print"+t);
   }
}
