using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
//public struct WorkPos
//{
//    public Transform workPos;
//    public Builder builder;
//}

public struct WorkPos
{
    public Transform workPos;
    public Character builder;
}
public class ResourcesEntity : MonoBehaviour
{

    public ResourceInfoSO resourceInfoSO;
    [HideInInspector]public ResourceInfoSO resourceInfo;
    public List<Transform> workPosition=new List<Transform>();//暴露在窗口的位置，需要手动添加位置
    /*[HideInInspector]*/public List<WorkPos> workPosList = new List<WorkPos>();//记录采集位置和农民
    
    
    private void Awake()
    {
        resourceInfo=Instantiate(resourceInfoSO);

    }
    private void Start()
    {
         InitWorkPosStruct();
    }

    private void InitWorkPosStruct()
    {
        for (int i = 0; i < workPosition.Count; i++)
        {
            WorkPos work = new WorkPos();
            work.workPos = workPosition[i];
            workPosList.Add(work);
        }
    }
}
