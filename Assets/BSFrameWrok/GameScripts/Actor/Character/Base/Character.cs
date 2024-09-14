using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public enum Actor_Type
{
    Shield,//盾兵
    Archer,//弓手
    Builder,//农民
}

public class Character : MonoBehaviour,IMoveable,IAttackable,IHealthEntity
{
    #region FSM
    [SerializeField] private CharacterIdleSOBase characterIdleSOBase;
    [SerializeField] private CharacterWalkSOBase characterWalkSOBase;
    [SerializeField] private CharacterWorkSOBase characterWorkSOBase;


    //需要实例化的SO文件
    public CharacterIdleSOBase CharacterIdleInstance { get;set; }
    public CharacterWalkSOBase CharacterWalkInstance { get;set; }
    public CharacterWorkSOBase CharacterWorkInstance { get;set; }

    public CharacterStateMachine stateMachine { get;set; }

    public CharacterIdleState idleSate { get;set; }
    public CharacterWalkState walkState { get;set; }   
    public CharacterWorkState workState { get;set; }
    #endregion

    #region 公有属性
    public NavMeshAgent agent { get; set ; }

    public Actor_Type actorType;

    public Animator animator { get; set; }

    public UnitAttack unitAttack { get ; set ; }



    private GameObject _selectedSprite;

    [HideInInspector]public AnimationEventListener _aniListener;
    #endregion

    #region Health
    public int MaxHealth { get ; set; }
    public int CurHealth { get; set; }
    public bool _canIncrease { get; set; }
    public bool _canDecrease { get; set; }
    public bool _isDead { get; set; }
    #endregion
    private void Awake()
    {
        CharacterIdleInstance = Instantiate(characterIdleSOBase);
        CharacterWalkInstance = Instantiate(characterWalkSOBase);
        CharacterWorkInstance = Instantiate(characterWorkSOBase);

        stateMachine = new CharacterStateMachine();

        idleSate = new CharacterIdleState(this, stateMachine);
        walkState = new CharacterWalkState(this, stateMachine);
        workState = new CharacterWorkState(this, stateMachine);

        
        agent=GetComponent<NavMeshAgent>();
        animator=GetComponentInChildren<Animator>();
        _selectedSprite = transform.Find("SelectedSprite").gameObject;
        unitAttack = GetComponentInChildren<UnitAttack>();
        _aniListener = GetComponentInChildren<AnimationEventListener>();
    }


    private void Start()
    {
        CharacterIdleInstance.Initialize(gameObject, this);
        CharacterWalkInstance.Initialize(gameObject, this);
        CharacterWorkInstance.Initialize(gameObject, this);


        stateMachine.Initlalize(idleSate);
    }

    private void Update()
    {
        stateMachine.currentState.FrameUpdate();
    }

    private void FixedUpdate()
    {
        stateMachine.currentState.PhysicsUpdate();
    }

    public void Move(Vector3 targetPos)
    {
        agent.SetDestination(targetPos);
    }

    public virtual void Attack()
    {
        
    }

    public void IsSelecterActor(bool isSelect)
    {
        _selectedSprite.SetActive(isSelect);
        if (isSelect)
            _selectedSprite.transform.DOScale(0, 0.35f).From().SetEase(Ease.OutBack);
    }

    public WaitUntil WaitForMesh()
    {
        return new WaitUntil(() => !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance);
    }

    public void TakeDamage(int damage)
    {
        
    }

    public void Healing(int heal)
    {
        
    }

    public void Die()
    {
        
    }

    public enum AnimationTriggerType
    {
        //处理声音?
    }
}
