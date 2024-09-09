using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Builder : Actor
{
    private ResourceHealth _resHealth;
    private ResourcesEntity _resources;
    public int builderIndex=-1;//采集位置的索引下标
    //private ResourcesCount _resourcesCount=new ResourcesCount(Resources_Type.Wood,0);//记录采集的资源
    public Dictionary<Resources_Type,int> resourceDic = new Dictionary<Resources_Type,int>();  
    public float gatherSpeed = 1.0f;//采集速度
    public int maxGatherCount;//最大采集数量
    public int curCarryCount = 0;//当前携带的资源量

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
        GatherResource();//进行采集资源的判断
    }

    /// <summary>
    /// 农民的移动方法
    /// </summary>
    /// <param name="targetPos"></param>
    /// <param name="isStopCurTask">是否停止当前的任务</param>
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

    //移动位置，并且更新索引
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
    /// 收集资源的总协程
    /// </summary>
    /// <returns></returns>
    IEnumerator CollectCycle()
    {
        while(_resources)
        {
            //前往资源采集点
            curTaskCor = StartCoroutine(StartAttck());
            yield return curTaskCor;

            //存放资源
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
            //寻找对应资源类型
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
            //第一次采集
            else
            {
                resCount = (int)gatherSpeed * _resources.resourceInfo.resValue.amount;
                resourceDic.Add(_resources.resourceInfo.resType, resCount);
            }
            resourceDic[_resources.resourceInfo.resType] = resCount;

            //当前携带量
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
