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



    private void Start()
    {
        EventManager.Trigger(EEventType.Refresh_Resources,resources_Types);
    }

    void Update()
    {
        ClickToCollect();
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
        EventManager.Trigger(EEventType.Refresh_Resources, resources_Types);
    }
}
