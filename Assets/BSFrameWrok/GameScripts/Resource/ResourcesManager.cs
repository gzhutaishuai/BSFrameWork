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
    /// ����ռ�
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
                            if (!isExistedPos(resourceHealth.GetComponent<ResourcesEntity>()))//�ж��Ƿ��п���λ�ÿ��Թ���
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
    /// ��Դ�Ƿ���ڶ���λ��
    /// </summary>
    /// <param name="resources"></param>
    /// <returns></returns>
    private bool isExistedPos(ResourcesEntity resources)
    {
        bool isFull = true;//�Ƿ��п���λ�� falseΪ�У�trueΪ��
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
                resourcesCount1.count += resourcesCount.count;//������������Դ�����ۼ�
                resources_Types[i] = resourcesCount1;
            }
        }
        EventManager.Trigger(EEventType.Refresh_Resources, resources_Types);
    }
}
