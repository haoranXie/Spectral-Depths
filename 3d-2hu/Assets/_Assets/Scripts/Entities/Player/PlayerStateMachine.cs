using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.ReorderableList;
using UnityEngine;

public class PlayerStateMachine : StateMachine
{
    [HideInInspector]
    public PlayerIdleState idleState;
    [HideInInspector]
    public PlayerMovingState movingState;
    public Transform player;

    public event EventHandler<OnStateChangedEventArgs> OnStateChanged;

    public class OnStateChangedEventArgs: EventArgs{
        public BaseState state;
    }




    private void Awake(){
        idleState = new PlayerIdleState(this);
        movingState = new PlayerMovingState(this);
    }
    protected override BaseState GetInitialState()
    {
        return idleState;
    }

    public override void ChangeState(BaseState newState)
    {
        base.ChangeState(newState);
        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs{
            state=newState
        });
    }



}
