using RTSGame.Event;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// 解决相机Y轴旋转的问题
/// </summary>
public struct StraightLine
{
    private float k;
    private float b;

    public void SetStrightLine(Vector3 p,float angle)
    {
        k=MathF.Tan(angle*Mathf.Deg2Rad);
        b = p.z - k * p.x;
    }
    public float GetPointDistance(Vector3 p)
    {
        return Mathf.Abs(k * p.x - p.z + b) / Mathf.Sqrt(k * k + 1);
    }
}

public class ActorManager : MonoSingleton<ActorManager>
{
    private List<Character> _allActors = new List<Character>();//所有角色
    [SerializeField] public List<Character> selectActors = new List<Character>();//框选中的角色

    public static int curActorsCount = 0;
    public static int MaxActorsCount = 4;
    public static bool canAdd = true;

    #region Draw
    private LineRenderer _line;
    private bool _isDragging;
    private Vector3 _beginDrag;
    private Vector3 _endDrag;
    private Vector3 _upDrag;
    private Vector3 _downDrag;

    private Vector3 _leftupPoint;
    private Vector3 _rightdownPoint;
    RaycastHit hitInfo;
    #endregion

    #region Move
    [SerializeField] private float offsetDis;//单位之间的偏移距离
    private Vector3 lastPos=Vector3.zero;//上一次的点击位置

    #endregion
    private void Awake()
    {
        _line=GetComponent<LineRenderer>();
    }

    private void Start()
    {
        _allActors.AddRange(GetComponentsInChildren<Character>());
        EventManager.Trigger(EEventType.Update_population, _allActors.Count, 0);
    }

    private void Update()
    {
        Select();
        DrawSelected();
        ClickToMove();
    }

    /// <summary>
    /// 点击移动
    /// </summary>
    private void ClickToMove()
    {
        if (selectActors.Count == 0)
        {
            return;
        }
        if (Input.GetMouseButtonDown(1))
        {
            //if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo, 1000, LayerMask.GetMask("Terrain")))

            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo, 1000))
            {
                Collider collider = hitInfo.collider;
                //点击地形
                if (collider.CompareTag("Terrain"))
                {
                    List<Vector3> solidersPos = GetSolidersPos(hitInfo.point);
                    Vector3 centerPoint = Vector3.zero;
                    //计算群组的中心点
                    for (int i = 0; i < selectActors.Count; i++)
                    {
                        centerPoint += selectActors[i].transform.position;
                    }
                    centerPoint /= selectActors.Count;
                    if (Vector3.Angle((hitInfo.point - selectActors[0].transform.position).normalized, selectActors[0].transform.forward) > 60)
                    {
                        selectActors.Sort((a, b) =>
                            {
                                if (a.actorType < b.actorType)
                                    return -1;//枚举值大的排序到最前端
                                else if (a.actorType == b.actorType)//相同兵种才进行距离排序
                                {
                                    //距离目标点远的的单位排序更靠后，位置更靠后
                                    if (Vector3.Distance(a.transform.position, hitInfo.point) > Vector3.Distance(b.transform.position, hitInfo.point))
                                        return 1;
                                    else return -1;
                                }
                                else
                                    return 1;
                            });

                    }

                    for (int i = 0; i < selectActors.Count; i++)
                    {
                        
                        switch (selectActors[i].actorType)
                        {
                                case Actor_Type.Builder:
                                if (selectActors[i].TryGetComponent(out FSMBuilder builder))
                                {
                                    //builder.Move(solidersPos[i]);
                                    builder.stateMachine.ChangeState(builder.walkState, solidersPos[i]);
                                }
                                break;
                                case Actor_Type.Shield:
                                selectActors[i].Move(solidersPos[i]);
                                break;
                                case Actor_Type.Archer:
                                selectActors[i].Move(solidersPos[i]);
                                break;                        }

                    }
                }
                else if(collider.CompareTag("Building")) //右键点击建筑物
                {
                    if(collider.TryGetComponent(out Building building))
                    {
                        if(building.isBuildOver)
                        {
                            foreach (FSMBuilder ac in selectActors )
                            {
                                //ac.ReturntoTown(building.targetPos.position, building);
                                ac.stateMachine.ChangeState(ac.workState, building);
                            }
                        }
                    }
                }
            }
        }
    }

    private List<Vector3> GetSolidersPos(Vector3 targetPos)
    {
        //鼠标点击位置和上一次位置的点做差得出正方向向量
        Vector3 curPointForward = Vector3.zero;
        //正方向向量向右偏转90°
        Vector3 curPointRight= Vector3.zero;

        if (lastPos != Vector3.zero)
        {
            //说明是重复同一个群组移动
            curPointForward=(targetPos- lastPos).normalized;
        }
        else
        {
            Vector3 centerPoint= Vector3.zero;
            //计算群组的中心点
            for(int i=0;i< selectActors.Count;i++) 
            {
                centerPoint += selectActors[i].transform.position;
            }
            centerPoint/=selectActors.Count;
            curPointForward = (targetPos-centerPoint).normalized;
        }
        curPointRight = Quaternion.Euler(0, 90, 0) * curPointForward;
        lastPos=targetPos;
        List<Vector3> solidersPos=new List<Vector3>();
        switch (selectActors.Count)
        {
            case 1:
                solidersPos.Add(targetPos);
                break;
            case 2:
                solidersPos.Add(targetPos + curPointRight * offsetDis / 2);
                solidersPos.Add(targetPos - curPointRight * offsetDis / 2);
                break;
            case 3:
                solidersPos.Add(targetPos);
                solidersPos.Add(targetPos + curPointRight * offsetDis / 2);
                solidersPos.Add(targetPos - curPointRight * offsetDis / 2);
                break;
            case 4:
                solidersPos.Add(targetPos);
                solidersPos.Add(targetPos + curPointRight * offsetDis / 2);
                solidersPos.Add(targetPos - curPointRight * offsetDis / 2);
                solidersPos.Add(targetPos - curPointForward * offsetDis / 2);
                break;
            case 5:
                solidersPos.Add(targetPos);
                solidersPos.Add(targetPos + curPointRight * offsetDis / 2);
                solidersPos.Add(targetPos - curPointRight * offsetDis / 2);
                solidersPos.Add(targetPos - curPointForward * offsetDis / 2);
                solidersPos.Add(targetPos - curPointForward * offsetDis / 2 - curPointRight * offsetDis / 2);
                break;
            case 6:
                solidersPos.Add(targetPos);
                solidersPos.Add(targetPos + curPointRight * offsetDis / 2);
                solidersPos.Add(targetPos - curPointRight * offsetDis / 2);
                solidersPos.Add(targetPos - curPointForward * offsetDis / 2);
                solidersPos.Add(targetPos - curPointForward * offsetDis / 2 - curPointRight * offsetDis / 2);
                solidersPos.Add(targetPos - curPointForward * offsetDis / 2 + curPointRight * offsetDis / 2);
                break;
            case 7:
                solidersPos.Add(targetPos);
                solidersPos.Add(targetPos + curPointRight * offsetDis / 2);
                solidersPos.Add(targetPos - curPointRight * offsetDis / 2);
                solidersPos.Add(targetPos - curPointForward * offsetDis / 2);
                solidersPos.Add(targetPos - curPointForward * offsetDis / 2 - curPointRight * offsetDis / 2);
                solidersPos.Add(targetPos - curPointForward * offsetDis / 2 + curPointRight * offsetDis / 2);
                solidersPos.Add(targetPos - curPointForward * offsetDis);
                break;
            case 8:
                solidersPos.Add(targetPos);
                solidersPos.Add(targetPos + curPointRight * offsetDis / 2);
                solidersPos.Add(targetPos - curPointRight * offsetDis / 2);
                solidersPos.Add(targetPos - curPointForward * offsetDis / 2);
                solidersPos.Add(targetPos - curPointForward * offsetDis / 2 - curPointRight * offsetDis / 2);
                solidersPos.Add(targetPos - curPointForward * offsetDis / 2 + curPointRight * offsetDis / 2);
                solidersPos.Add(targetPos - curPointForward * offsetDis);
                solidersPos.Add(targetPos - curPointForward * offsetDis - curPointRight * offsetDis/2);
                break;
            case 9:
                solidersPos.Add(targetPos);
                solidersPos.Add(targetPos + curPointRight * offsetDis / 2);
                solidersPos.Add(targetPos - curPointRight * offsetDis / 2);
                solidersPos.Add(targetPos - curPointForward * offsetDis / 2);
                solidersPos.Add(targetPos - curPointForward * offsetDis / 2 - curPointRight * offsetDis / 2);
                solidersPos.Add(targetPos - curPointForward * offsetDis / 2 + curPointRight * offsetDis / 2);
                solidersPos.Add(targetPos - curPointForward * offsetDis);
                solidersPos.Add(targetPos - curPointForward * offsetDis - curPointRight * offsetDis/2);
                solidersPos.Add(targetPos - curPointForward * offsetDis + curPointRight * offsetDis/2);
                break;
        }
        return solidersPos;
    }
    private void Select()
    {
        if (Input.GetMouseButtonDown(0))
        {
            lastPos = Vector3.zero;
            _isDragging = true;
            _beginDrag = Input.mousePosition;

            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo, 1000, LayerMask.GetMask("Terrain")))
            {
                _leftupPoint = hitInfo.point;
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            _isDragging = false;
            _line.positionCount = 0;

            //清空选择框
            foreach (Character actor in selectActors)
            {
                actor.IsSelecterActor(false);
            }
            selectActors.Clear();

            Vector3 endDrag=Input.mousePosition;
            //单击判断
            if(Vector3.Distance(_beginDrag,endDrag) <10.0f)
            {
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo, 1000, LayerMask.GetMask("Actor")))
                {
                    if (hitInfo.collider.CompareTag("Actor"))
                    {
                        Character actor = hitInfo.collider.transform.GetComponent<Character>();//根据actor脚本来添加
                        if (actor != null)
                        {
                            actor.IsSelecterActor(true);
                            selectActors.Add(actor);
                        }
                    }
                }
                    return;
            }

            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo, 1000, LayerMask.GetMask("Terrain")))
            {
                Vector3 center = new Vector3((hitInfo.point.x + _leftupPoint.x) / 2, 0, (hitInfo.point.z + _leftupPoint.z) / 2);

                Vector3 halfSize = new Vector3(0, 4, 0);

                float angle = Camera.main.transform.eulerAngles.y;

                if (angle % 180 == 0)
                {
                    halfSize.x = MathF.Abs(hitInfo.point.x - _leftupPoint.x) / 2;
                    halfSize.z = MathF.Abs(hitInfo.point.z - _leftupPoint.z) / 2;
                }
                else if (angle % 90 == 0)
                {
                    halfSize.z = MathF.Abs(hitInfo.point.x - _leftupPoint.x) / 2;
                    halfSize.x = MathF.Abs(hitInfo.point.z - _leftupPoint.z) / 2;
                }
                else
                {
                    StraightLine line = new StraightLine();
                    line.SetStrightLine(hitInfo.point, 90 - angle);
                    halfSize.x = line.GetPointDistance(center);
                    line.SetStrightLine(hitInfo.point, 180 - angle);
                    halfSize.z = line.GetPointDistance(center);
                }
                // Vector3 halfSize=new Vector3(MathF.Abs(hitInfo.point.x-_leftupPoint.x),4,MathF.Abs(hitInfo.point.z-_leftupPoint.z));

                Collider[] colliders = Physics.OverlapBox(center, halfSize, Quaternion.Euler(0, angle, 0));

                //生成对应的实体box
                //GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
                //go.transform.position = center;
                //go.transform.localScale = halfSize * 2;
                //go.transform.rotation = Quaternion.Euler(0, angle, 0);

                for (int i = 0; i < colliders.Length; i++)
                {
                    Character actor = colliders[i].GetComponent<Character>();
                    if (actor != null&&selectActors.Count<=8)
                    {
                        actor.IsSelecterActor(true);
                        selectActors.Add(actor);
                    }
                }
            }
        }
    }

    private void DrawSelected()
    {
      if(_isDragging)
      {
            _endDrag=Input.mousePosition;
            _beginDrag.z = 5;
            _endDrag.z=5;
            _upDrag.x = _endDrag.x;
            _upDrag.y = _beginDrag.y;
            _upDrag.z = 5;
            _downDrag.x=_beginDrag.x;
            _downDrag.y=_endDrag.y;
            _downDrag.z = 5;

            _line.positionCount = 4;
            _line.SetPosition(0, Camera.main.ScreenToWorldPoint(_beginDrag));
            _line.SetPosition(1, Camera.main.ScreenToWorldPoint(_upDrag));
            _line.SetPosition(2, Camera.main.ScreenToWorldPoint(_endDrag));
            _line.SetPosition(3, Camera.main.ScreenToWorldPoint(_downDrag));
        }
    }
}
