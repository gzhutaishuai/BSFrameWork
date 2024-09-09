using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterState
{
    public Character character;
    public CharacterStateMachine characterStateMachine;//fsm

    public CharacterState(Character character, CharacterStateMachine characterStateMachine)
    {
        this.character = character;
        this.characterStateMachine = characterStateMachine;
    }

    public virtual void EnterState(object obj=null) { }

    public virtual void ExitState() { }

    public virtual void FrameUpdate() {
        character.animator.SetFloat("Speed", character.agent.velocity.magnitude);
    }

    public virtual void PhysicsUpdate() { }

    public virtual void AniamtionTriggerEvent(Character.AnimationTriggerType animationTriggerType) { }

}
