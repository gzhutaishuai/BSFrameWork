using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterIdleState : CharacterState
{
    public CharacterIdleState(Character character,CharacterStateMachine characterStateMachine) : base(character, characterStateMachine) { }

    public override void AniamtionTriggerEvent(Character.AnimationTriggerType animationTriggerType)
    {
        base.AniamtionTriggerEvent(animationTriggerType);
        character.CharacterIdleInstance.DoAnimationTriggerEventLogic(animationTriggerType);
    }

    public override void EnterState(object obj)
    {
        base.EnterState(obj);
        character.CharacterIdleInstance.DoEnterLogic(obj);
    }

    public override void ExitState()
    {
        base.ExitState();
        character.CharacterIdleInstance.DoExitLogic();
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();
        character.CharacterIdleInstance.DoFrameUpdateLogic();
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        character.CharacterIdleInstance.DoPhysicsUpdateLogic();
    }
}
