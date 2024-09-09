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
    public List<Transform> workPosition=new List<Transform>();//��¶�ڴ��ڵ�λ�ã���Ҫ�ֶ����λ��
    /*[HideInInspector]*/public List<WorkPos> workPosList = new List<WorkPos>();//��¼�ɼ�λ�ú�ũ��
    
    
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
