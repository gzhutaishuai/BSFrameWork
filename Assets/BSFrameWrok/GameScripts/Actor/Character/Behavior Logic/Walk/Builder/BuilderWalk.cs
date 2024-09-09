using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = ("Builder_Walk_SO"), menuName = ("Character Logic/Walk Logic/Builder"))]
public class BuilderWalk:CharacterWalkSOBase
{
    public override void DoAnimationTriggerEventLogic(Character.AnimationTriggerType type)
    {
        base.DoAnimationTriggerEventLogic(type);
    }

    public override void DoEnterLogic(object obj)
    {
        base.DoEnterLogic(obj);
        Vector3 targetPos=(Vector3)obj;
        character.animator.Play("Walk");
        character.Move(targetPos);
    }

    public override void DoExitLogic()
    {
        base.DoExitLogic();
    }

    public override void DoFrameUpdateLogic()
    {
        base.DoFrameUpdateLogic();
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
    }
}
