using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class PlayerIdleState: BaseState
{

    public PlayerIdleState(PlayerStateMachine stateMachine) : base("Idle",stateMachine){    }
    public override void Enter()
    {
        base.Enter();
    }

    public override void Awake()
    {
        base.Awake();
    }

    public override void Update()
    {
        Vector2 inputVector = GameInput.Instance.GetMovementVectorNormalized();
        if(inputVector!=Vector2.zero){ // If Player doesn't press any movement keys
            stateMachine.ChangeState(((PlayerStateMachine)stateMachine).movingState);
        }
    }

}
