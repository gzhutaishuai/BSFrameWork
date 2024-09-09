using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Builder : Actor
{
    private ResourceHealth _resHealth;
    private ResourcesEntity _resources;
    public int builderIndex=-1;//�ɼ�λ�õ������±�
    //private ResourcesCount _resourcesCount=new ResourcesCount(Resources_Type.Wood,0);//��¼�ɼ�����Դ
    public Dictionary<Resources_Type,int> resourceDic = new Dictionary<Resources_Type,int>();  
    public float gatherSpeed = 1.0f;//�ɼ��ٶ�
    public int maxGatherCount;//���ɼ�����
    public int curCarryCount = 0;//��ǰЯ������Դ��

    [HideInInspector] public Coroutine curTaskCor;

    private Building targetBuilding;

    public override void Update()
    {
        base.Update();
        if (_resHealth != null && targetBuilding == null)
        {
            transform.LookAt(_resHealth.transform.position);
        }
        else if (targetBuilding != null)
        {
            transform.LookAt(targetBuilding.transform.position);
        }
    }
    public override void Attack()
    {
        if (_resHealth != null)
        {
            _resHealth.TakeDamage(_unitAttack.attackDamage.comonDamage);
        }
        GatherResource();//���вɼ���Դ���ж�
    }

    /// <summary>
    /// ũ����ƶ�����
    /// </summary>
    /// <param name="targetPos"></param>
    /// <param name="isStopCurTask">�Ƿ�ֹͣ��ǰ������</param>
    public override void Move(Vector3 targetPos,bool isStopCurTask=false)
    {
        base.Move(targetPos,isStopCurTask);
        if (isStopCurTask)
        {
            StopTask();
        }
    }

    public  void ReturntoTown(Vector3 homePos,Building building)
    {
        StopTask();
        targetBuilding = building;
        curTaskCor = StartCoroutine(ReturnHome(homePos));
    }

    IEnumerator ReturnHome(Vector3 homePos)
    {
        Move(homePos, false);
        yield return WaitForMesh();
        PutResources();

    }

    public WaitUntil WaitForMesh()
    {
        return new WaitUntil(() => !_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance);
    }

    //�ƶ�λ�ã����Ҹ�������
    public void MovtToWrokPosition(ResourcesEntity resources)
    {
        if(builderIndex!=-1)
        {
            if (builderIndex < resources.workPosition.Count)
            {
                Move(resources.workPosition[builderIndex].position, false);
                return;
            }
        }

        for (int i = 0; i < resources.workPosList.Count; i++)
        {
            if (resources.workPosList[i].builder==null)
            {
                WorkPos workPos = resources.workPosList[i];
                //workPos.builder=this;
                resources.workPosList[i] = workPos;
                builderIndex = i;
                Move(resources.workPosition[i].position, false);
                break;
            }
        }
    }

    public override void AttackTargt(object obj)
    {
        StopTask();
        ResourceHealth _resH= (ResourceHealth)obj;
        if (_resH != null)
        {
            _resHealth= _resH;
            _resources = _resH.transform.GetComponent<ResourcesEntity>();
            curTaskCor = StartCoroutine(CollectCycle());
        }
        curTaskCor = null;
    }

    /// <summary>
    /// �ռ���Դ����Э��
    /// </summary>
    /// <returns></returns>
    IEnumerator CollectCycle()
    {
        while(_resources)
        {
            //ǰ����Դ�ɼ���
            curTaskCor = StartCoroutine(StartAttck());
            yield return curTaskCor;

            //�����Դ
            if (curCarryCount >= maxGatherCount)
            {
                curTaskCor = StartCoroutine(ComeBackHome());
                yield return curTaskCor;
            }
        }

        curTaskCor= null;
        builderIndex = -1;
    }

    IEnumerator StartAttck()
    {
        MovtToWrokPosition(_resources);
        yield return WaitForMesh();
        while(_resHealth && curCarryCount < maxGatherCount)
        {

            if (_resHealth != null&& curCarryCount < maxGatherCount)
            {
                _ani.SetTrigger("Attack");
            }
            yield return new WaitForSeconds(1f);
        }
        
        yield return new WaitForSeconds(1f);
    }

    IEnumerator ComeBackHome()
    {
        FindNearCollectBuilding();
        Move(targetBuilding.targetPos.position);
        yield return WaitForMesh();
        PutResources();
    }

        public void StopTask()
    {
        if (_resources != null)
        {
            WorkPos workPos = _resources.workPosList[builderIndex];
            workPos.builder = null;
            _resources.workPosList[builderIndex] = workPos;
            builderIndex = -1;
        }
        if(curTaskCor != null)
        {
            StopCoroutine(curTaskCor);
        }
        _resHealth = null;
        _resources = null;
        targetBuilding=null;
    }

    private void GatherResource()
    {
        if (_resources != null)
        {
            int resCount = 0;
            //Ѱ�Ҷ�Ӧ��Դ����
            if (resourceDic.TryGetValue(_resources.resourceInfo.resType, out resCount))
            {
                if (resCount + (int)gatherSpeed * _resources.resourceInfo.resValue.amount >= maxGatherCount)
                {
                    resCount = maxGatherCount;
                }
                else
                {
                    resCount += (int)gatherSpeed * _resources.resourceInfo.resValue.amount;
                }
            }
            //��һ�βɼ�
            else
            {
                resCount = (int)gatherSpeed * _resources.resourceInfo.resValue.amount;
                resourceDic.Add(_resources.resourceInfo.resType, resCount);
            }
            resourceDic[_resources.resourceInfo.resType] = resCount;

            //��ǰЯ����
            if (curCarryCount + (int)gatherSpeed * _resources.resourceInfo.resValue.amount >= maxGatherCount)  
                curCarryCount = maxGatherCount; 
            else 
                curCarryCount += (int)gatherSpeed * _resources.resourceInfo.resValue.amount;
        }
    }

    private void PutResources()
    {        
          if(resourceDic!=null)
          {
            ResourcesCount resCount = new ResourcesCount();
            foreach (var res in resourceDic)
            {
                resCount.type = res.Key;
                resCount.count = res.Value;
                ResourcesManager.Instance.Refresh(resCount);
                //Debug.Log($"resCount:{resCount.type} {resCount.count}");
            }
            resourceDic.Clear();
          }
        curCarryCount = 0;
        targetBuilding = null;
    }

    private void FindNearCollectBuilding()
    {
        //Debug.Log("FindNearCollectBuilding");
        List<Building> buildings=BuildingManager.Instance.buildings.Where(b=>b.isResCollectBuilding).ToList();

        buildings=buildings.OrderBy(b=>Vector3.Distance(transform.position, b.transform.position)).ToList();

        if (buildings.Count == 0) return;

        targetBuilding = buildings[0];

        //foreach (Building building in buildings)
        //{
        //    Debug.Log(building.name);
        //}
    }
}
