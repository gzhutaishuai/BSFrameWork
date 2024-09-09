using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterWorkSOBase : ScriptableObject
{
    public Character character;
    public GameObject gameObject;

    public virtual void Initialize(GameObject gameObject, Character character)
    {
        this.gameObject = gameObject;
        this.character = character;
    }

    public virtual void DoEnterLogic(object obj)
    {

    }

    public virtual void DoExitLogic()
    {
        ResetValues();
    }

    public virtual void DoFrameUpdateLogic()
    {

    }

    public virtual void DoPhysicsUpdateLogic()
    {

    }

    public virtual void DoAnimationTriggerEventLogic(Character.AnimationTriggerType type)
    {

    }

    /// <summary>
    ///  用来处理需要重新设置的参数
    /// </summary>
    public virtual void ResetValues() { }
}
