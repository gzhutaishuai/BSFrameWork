using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterWalkState:CharacterState
{
    public CharacterWalkState(Character character, CharacterStateMachine characterStateMachine) : base(character, characterStateMachine) { }

    public override void AniamtionTriggerEvent(Character.AnimationTriggerType animationTriggerType)
    {
        base.AniamtionTriggerEvent(animationTriggerType);
        character.CharacterWalkInstance.DoAnimationTriggerEventLogic(animationTriggerType);
    }

    public override void EnterState(object obj)
    {
        base.EnterState();
        character.CharacterWalkInstance.DoEnterLogic(obj);
    }

    public override void ExitState()
    {
        base.ExitState();
        character.CharacterWalkInstance.DoExitLogic();
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();
        character.CharacterWalkInstance.DoFrameUpdateLogic();
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        character.CharacterWalkInstance.DoPhysicsUpdateLogic();
    }
}
