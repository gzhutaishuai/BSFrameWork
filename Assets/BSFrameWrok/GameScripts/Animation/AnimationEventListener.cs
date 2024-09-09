using BS;
using RTSGame.Event;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimationEventListener : MonoBehaviour
{ 
    [HideInInspector]public UnityEvent attackEvent=new UnityEvent();

    public void AttackEvent()
    {
        attackEvent.Invoke();
    }
}
