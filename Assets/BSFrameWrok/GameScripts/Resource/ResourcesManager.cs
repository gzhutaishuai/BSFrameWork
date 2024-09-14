using RTSGame.Event;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct ResourcesCount
{
    public Resources_Type type;
    public int count;
    public ResourcesCount(Resources_Type _type,int _count)
    {
        type = _type;
        count = _count;
    }
}

public class ResourcesManager : MonoSingleton<ResourcesManager>
{
    RaycastHit hitInfo;
    public  List<ResourcesCount> resources_Types=new List<ResourcesCount>();

    public Dictionary<int,ResourcesEntity> foodState=new Dictionary<int, ResourcesEntity>();
    public Dictionary<int, ResourcesEntity> stoneState =new Dictionary<int, ResourcesEntity>();
    public Dictionary<int, ResourcesEntity> goldState =new Dictionary<int, ResourcesEntity>();
    public Dictionary<int, ResourcesEntity> woodState = new Dictionary<int, ResourcesEntity>();

    public static int ResIndex = 0;

    private void OnEnable()
    {
        EventManager.Listen(EEventType.Update_ResState, Update_ResState);
    }

    private void Start()
    {
        EventManager.Trigger(EEventType.Refresh_ResourcesUI,resources_Types);
        Initialize();
    }

    private void OnDisable()
    {
        EventManager.Ignore(EEventType.Update_ResState, Update_ResState);
    }

    void Update()
    {
        ClickToCollect();
    }

    private void Initialize()
    {
        foreach(ResourcesEntity info in this.GetComponentsInChildren<ResourcesEntity>())
        {
            info.resIdx = ResIndex;
            switch (info.resourceInfo.resType)
            { 
               case Resources_Type.Wood:
                    woodState.Add(ResIndex, info);
                    break;
               case Resources_Type.Food:
                    foodState.Add(ResIndex, info);
                    break;
                case Resources_Type.Stone:
                    stoneState.Add(ResIndex, info);
                    break;
                case Resources_Type.Gold:
                    goldState.Add(ResIndex, info);
                    break;
            }
            ResIndex++;
        }
        //Debug.Log($"wood:{woodState.Count}");
        //Debug.Log($"food:{foodState.Count}");
        //Debug.Log($"stone:{stoneState.Count}");
        //Debug.Log($"gold:{goldState.Count}");
    }

    public void Update_ResState(params object[] obj) 
    {
        int index = (int)obj[0];
        ResourcesEntity resourcesEntiy = (ResourcesEntity)obj[1];

        switch(resourcesEntiy.resourceInfo.resType)
        {
            case Resources_Type.Wood:
                if (woodState.TryGetValue(index, out resourcesEntiy))
                {
                    woodState.Remove(resourcesEntiy.resIdx);
                }
                break;
            case Resources_Type.Food:
                if (foodState.TryGetValue(index, out resourcesEntiy))
                {
                    foodState.Remove(resourcesEntiy.resIdx);
                }
                    break;
            case Resources_Type.Stone:
                if (stoneState.TryGetValue(index, out resourcesEntiy))
                {
                    stoneState.Remove(resourcesEntiy.resIdx);
                }
                    break;
            case Resources_Type.Gold:
                if (goldState.TryGetValue(index, out resourcesEntiy))
                {
                    goldState.Remove(resourcesEntiy.resIdx);
                }
                    break;
        }
    }


    /// <summary>
    /// 点击收集
    /// </summary>
    private void ClickToCollect()
    {
        if (Input.GetMouseButtonDown(1))
        {
            hitInfo = Utility.ClickToDo();
            Collider collider = hitInfo.collider;
            if(collider.CompareTag("Resource"))
            {
                for (int i = 0; i < ActorManager.Instance.selectActors.Count; i++)
                {
                    if (ActorManager.Instance.selectActors[i].TryGetComponent(out FSMBuilder builder))
                    {
                        if (collider.TryGetComponent(out ResourceHealth resourceHealth))
                        {
                            if (!isExistedPos(resourceHealth.GetComponent<ResourcesEntity>()))//判断是否有空闲位置可以工作
                            {
                                //builder.AttackTargt(resourceHealth);
                                builder.stateMachine.ChangeState(builder.workState,resourceHealth);
                            }
                            else
                            {
                                return;
                            }
                            
                        }
                    }
                }
            }

        }
    }
    /// <summary>
    /// 资源是否存在多余位置
    /// </summary>
    /// <param name="resources"></param>
    /// <returns></returns>
    private bool isExistedPos(ResourcesEntity resources)
    {
        bool isFull = true;//是否有空闲位置 false为有，true为无
        for(int i=0;i<resources.workPosList.Count;i++)
        {
            if (resources.workPosList[i].builder==null)
            {
                isFull = false ;
            }
        }
        return isFull;
    }

    public void Refresh(ResourcesCount resourcesCount)
    {
        for(int i=0;i< resources_Types.Count;i++)
        {
            if(resourcesCount.type==resources_Types[i].type)
            {
                ResourcesCount resourcesCount1 = resources_Types[i];
                resourcesCount1.count += resourcesCount.count;//将传进来的资源数量累加
                resources_Types[i] = resourcesCount1;
            }
        }
        EventManager.Trigger(EEventType.Refresh_ResourcesUI, resources_Types);
    }
}
