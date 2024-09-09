using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;

//public enum Actor_Type
//{
//    Shield,//盾兵
//    Archer,//弓手
//    Builder,//农民
//}

public class Actor : MonoBehaviour
{
    [HideInInspector]public Animator _ani;
    [HideInInspector]public NavMeshAgent _agent;
    private GameObject _selectedSprite;
    private AnimationEventListener _aniListener;
    public Actor_Type actorType;

    [HideInInspector]public UnitAttack _unitAttack;

    private void Awake()
    {
        _ani = transform.GetComponentInChildren<Animator>();
        _agent = GetComponent<NavMeshAgent>();
        _selectedSprite = transform.Find("SelectedSprite").gameObject;
        _aniListener = GetComponentInChildren<AnimationEventListener>();
        _unitAttack = GetComponentInChildren<UnitAttack>();
    }

    private void Start()
    {
        IsSelecterActor(false);
        
    }

    private void OnEnable()
    {
        _aniListener.attackEvent.AddListener(Attack);
    }

   public virtual void Update()
    {
        _ani.SetFloat("Speed",_agent.velocity.magnitude);
    }

    /// <summary>
    /// 是否选中角色
    /// </summary>
    /// <param name="isSelect"></param>
    public void IsSelecterActor(bool isSelect)
    {
        _selectedSprite.SetActive(isSelect);
        if(isSelect) 
        _selectedSprite.transform.DOScale(0, 0.35f).From().SetEase(Ease.OutBack);
    }
    /// <summary>
    /// 移动方法
    /// </summary>
    /// <param name="targetPos"></param>
    public virtual void Move(Vector3 targetPos,bool isStopCurTask)
    {
        _ani.Play("Walk");
        _agent.SetDestination(targetPos);
    }

    public virtual void Attack()
    {
        //根据兵种不同重写攻击方法
    }
    public virtual void AttackTargt(object obj)
    {

    }

}
