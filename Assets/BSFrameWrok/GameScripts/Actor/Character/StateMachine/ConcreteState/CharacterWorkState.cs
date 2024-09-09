using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterWorkState:CharacterState
{
    public CharacterWorkState(Character character, CharacterStateMachine characterStateMachine) : base(character, characterStateMachine) { }

    public override void AniamtionTriggerEvent(Character.AnimationTriggerType animationTriggerType)
    {
        base.AniamtionTriggerEvent(animationTriggerType);
        character.CharacterWorkInstance.DoAnimationTriggerEventLogic(animationTriggerType);
    }

    public override void EnterState(object obj)
    {
        base.EnterState(obj);
        character.CharacterWorkInstance.DoEnterLogic(obj);
    }

    public override void ExitState()
    {
        base.ExitState();
        character.CharacterWorkInstance.DoExitLogic();
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();
        character.CharacterWorkInstance.DoFrameUpdateLogic();
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        character.CharacterWorkInstance.DoPhysicsUpdateLogic();
    }
}
