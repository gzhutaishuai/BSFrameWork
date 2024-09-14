using BS;
using RTSGame.Event;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

[CreateAssetMenu(fileName = ("Builder_Work_SO"), menuName = ("Character Logic/Work Logic/Builder"))]
public class BuilderWork : CharacterWorkSOBase
{
    #region 资源实体
    private ResourceHealth _resHealth;
    private ResourcesEntity _resEntity;
    public Dictionary<Resources_Type, int> resourceDic = new Dictionary<Resources_Type, int>();
    public float gatherSpeed = 1.0f;//采集速度
    public int maxGatherCount;//最大采集数量
    public int curCarryCount = 0;//当前携带的资源量
    public int builderIndex = -1;
    public float maxGatherDis;
    private Resources_Type lastGaherRes;
    #endregion

    #region 建筑
    private Building targetBuilding;

    #endregion

    [HideInInspector] public Coroutine curTaskCor;//管理的协程



    public override void DoAnimationTriggerEventLogic(Character.AnimationTriggerType type)
    {
        base.DoAnimationTriggerEventLogic(type);
    }

    /// <summary>
    /// 移动到工作地点
    /// </summary>
    /// <param name="resourcesEntity"></param>
    private void MoveToWrokPosition(ResourcesEntity resourcesEntity)
    {
        if (builderIndex != -1)
        {
            if (builderIndex < resourcesEntity.workPosition.Count)
            {
                character.Move(resourcesEntity.workPosition[builderIndex].position);
                return;
            }
        }

        for (int i = 0; i < resourcesEntity.workPosList.Count; i++)
        {
            if (resourcesEntity.workPosList[i].builder == null)
            {
                WorkPos workPos = resourcesEntity.workPosList[i];
                workPos.builder = character;
                resourcesEntity.workPosList[i] = workPos;
                builderIndex = i;
                character.Move(resourcesEntity.workPosition[i].position);
                break;
            }
        }
    }
    /// <summary>
    /// 控制朝向
    /// </summary>
    private void Look()
    {
        if (_resEntity != null && targetBuilding == null)
        {
            character.transform.LookAt(_resEntity.transform.position);
        }
        else if (targetBuilding != null)
        {
            character.transform.LookAt(targetBuilding.transform.position);
        }
    }

    public ResourcesEntity ResortResPos(Resources_Type resType)
    {
        List<ResourcesEntity> resList= new List<ResourcesEntity>();

        switch (resType)
        {
            case Resources_Type.Wood:
                foreach(var item in ResourcesManager.Instance.woodState)
                {
                    resList.Add(item.Value);
                }
                break;

            case Resources_Type.Food:

                break;

            case Resources_Type.Gold:

                break;
            case Resources_Type.Stone:

                break;
        }
        if (resList.Count == 0) return null;
        resList =resList.OrderBy(b=>Vector3.Distance(character.transform.position,b.transform.position)).ToList();
        if (Vector3.Distance(resList[0].transform.position, character.transform.position) > maxGatherDis)
            return null;
        return resList[0];
    }

    /// <summary>
    /// 采集协程循环
    /// </summary>
    /// <returns></returns>
    IEnumerator CollectCycle()
    {
        if(curCarryCount>=maxGatherCount)
        {
            curTaskCor = Timers.inst.StartCoroutine(ComeBackHome());
            yield return curTaskCor;    
        }


        while (_resEntity)
        {
            curTaskCor=Timers.inst.StartCoroutine(StartAttack());
            yield return curTaskCor;

            if (curCarryCount >= maxGatherCount)
            {
                curTaskCor = Timers.inst.StartCoroutine(ComeBackHome());
                yield return curTaskCor;
            }
        }

        EventManager.Trigger(EEventType.Update_ResState, _resEntity.resIdx, _resEntity);
        curTaskCor = null;
        builderIndex = -1;
        while(ResortResPos(lastGaherRes) != null)
        {
            _resEntity = ResortResPos(lastGaherRes);
            //Debug.Log($"{character.name}:{_resEntity.name}");
            _resHealth = _resEntity.transform.GetComponent<ResourceHealth>();
            //Debug.Log(_resEntity.name);
            while (_resEntity)
            {
                curTaskCor = Timers.inst.StartCoroutine(StartAttack());
                yield return curTaskCor;

                if (curCarryCount >= maxGatherCount)
                {
                    curTaskCor = Timers.inst.StartCoroutine(ComeBackHome());
                    yield return curTaskCor;
                }
            }
            EventManager.Trigger(EEventType.Update_ResState, _resEntity.resIdx, _resEntity);
        }
        curTaskCor =null;
        builderIndex=-1;
        if(curTaskCor==null&&_resEntity==null)
        {
            character.stateMachine.ChangeState(character.idleSate);
        }
    }

    IEnumerator StartAttack()
    {
        MoveToWrokPosition(_resEntity);
        yield return character.WaitForMesh();

        //Debug.Log($"当前资源量:{curCarryCount}" +
        //    $"最大资源量:{maxGatherCount}");
        while(_resEntity&&curCarryCount<maxGatherCount)
        {
            if (_resEntity != null && curCarryCount < maxGatherCount)
            {
                character.animator.SetBool("Attack1", true);
            }
            else
            {
                character.animator.SetBool("Attack1", false);
            }
            yield return new WaitForSeconds(0.5f);
        }
        character.animator.SetBool("Attack1", false);
        yield return new WaitForSeconds(0.5f);
    }

    IEnumerator ComeBackHome()
    {
        FindNearCollectBuilding();
        character.Move(targetBuilding.targetPos.position);
        yield return character.WaitForMesh();
        PutResources();
    }
    private void FindNearCollectBuilding()
    {
        //Debug.Log("FindNearCollectBuilding");
        List<Building> buildings = BuildingManager.Instance.buildings.Where(b => b.isResCollectBuilding).ToList();

        buildings = buildings.OrderBy(b => Vector3.Distance(character.transform.position, b.transform.position)).ToList();

        if (buildings.Count == 0) return;

        targetBuilding = buildings[0];

        //foreach (Building building in buildings)
        //{
        //    Debug.Log(building.name);
        //}
    }
    private void PutResources()
    {
        if (resourceDic != null)
        {
            ResourcesCount resCount = new ResourcesCount();
            foreach (var res in resourceDic)
            {
                resCount.type = res.Key;
                resCount.count = res.Value;
                ResourcesManager.Instance.Refresh(resCount);
                //Debug.Log($"Put resCount:{resCount.type} {resCount.count}");
            }
            resourceDic.Clear();
        }
        curCarryCount = 0;
        targetBuilding = null;
    }

    public void GatherAttack()
    {
        if (_resHealth != null)
        {
            _resHealth.TakeDamage(character.unitAttack.attackDamage.comonDamage);
        }
        GatherResource();
    }
    private void GatherResource()
    {
        if (_resEntity != null)
        {
            int resCount = 0;
            //寻找对应资源类型
            if (resourceDic.TryGetValue(_resEntity.resourceInfo.resType, out resCount))
            {
                if (resCount + (int)gatherSpeed * _resEntity.resourceInfo.resValue.amount >= maxGatherCount)
                {
                    resCount = maxGatherCount;
                }
                else
                {
                    resCount += (int)gatherSpeed * _resEntity.resourceInfo.resValue.amount;
                }
            }
            //第一次采集
            else
            {
                resCount = (int)gatherSpeed * _resEntity.resourceInfo.resValue.amount;
                resourceDic.Add(_resEntity.resourceInfo.resType, resCount);
            }
            resourceDic[_resEntity.resourceInfo.resType] = resCount;

            //当前携带量
            if (curCarryCount + (int)gatherSpeed * _resEntity.resourceInfo.resValue.amount >= maxGatherCount)
                curCarryCount = maxGatherCount;
            else
                curCarryCount += (int)gatherSpeed * _resEntity.resourceInfo.resValue.amount;
        }
    }

    public void ReturnToTown()
    {
        //character.animator.Play("Walk");
        curTaskCor = Timers.inst.StartCoroutine(ReturnHome());
    }
    IEnumerator ReturnHome()
    {
        character.Move(targetBuilding.targetPos.position);
        yield return character.WaitForMesh();
        PutResources();
    }
    /// <summary>
    /// 处理采集任务
    /// </summary>
    /// <param name="obj"></param>
    public void DealTask(object obj)
    {
        if (obj != null)
        {
            if (obj as ResourceHealth != null)
            {
                _resHealth = (ResourceHealth)obj;
                _resEntity = _resHealth.transform.GetComponent<ResourcesEntity>();
                lastGaherRes = _resEntity.resourceInfo.resType;
                curTaskCor = Timers.inst.StartCoroutine(CollectCycle());
            }
            else if (obj as Building != null)
            {
                targetBuilding = (Building)obj;
                ReturnToTown();
            }
        }
    }
    public override void DoEnterLogic(object obj)
    {
        base.DoEnterLogic(obj);
        character._aniListener.attackEvent.AddListener(GatherAttack);
        character.animator.SetBool("Attack1", false);
        ResetValues();
        DealTask(obj);
    }

    public override void DoExitLogic()
    {
        base.DoExitLogic();
        //Debug.Log("Exit Work");
        character._aniListener.attackEvent.RemoveListener(GatherAttack);
    }

    public override void DoFrameUpdateLogic()
    {
        base.DoFrameUpdateLogic();
        Look();

    }

    public override void DoPhysicsUpdateLogic()
    {
        base.DoPhysicsUpdateLogic();
    }

    public override void Initialize(GameObject gameObject, Character character)
    {
        base.Initialize(gameObject, character);
    }

    public override void ResetValues()
    {
        base.ResetValues();
        if(_resEntity != null )
        {
            WorkPos workPos = _resEntity.workPosList[builderIndex];
            workPos.builder = null;
            _resEntity.workPosList[builderIndex] = workPos;
            builderIndex = -1;
        }
        if(curTaskCor != null )
        {
            Timers.inst.StopCoroutine(curTaskCor);
        }
        character.animator.SetBool("Attack1", false);
        _resEntity =null;
        _resHealth = null;
        targetBuilding = null;
    }
}
