using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStateMachine
{
    public CharacterState currentState {  get; set; }

    /// <summary>
    /// ��ʼ��״̬
    /// </summary>
    /// <param name="startState"></param>
    public void Initlalize(CharacterState startState)
    {
        currentState = startState;
        currentState.EnterState();
    }

    /// <summary>
    /// �л�״̬
    /// </summary>
    /// <param name="newState"></param>
    public void ChangeState(CharacterState newState,object obj=null)
    {
        currentState.ExitState();
        currentState=newState;
        currentState.EnterState(obj);
    }
}
