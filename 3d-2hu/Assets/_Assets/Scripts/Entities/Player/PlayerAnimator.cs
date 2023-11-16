using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] private PlayerStateMachine playerStateMachine;
    private Animator animator;
    private const string IS_WALKING = "IsWalking";

    private void Awake(){
        playerStateMachine.OnStateChanged+=PlayerStateMachine_OnStateChanged;


        animator = GetComponent<Animator>();


    }
    private void PlayerStateMachine_OnStateChanged(object sender, PlayerStateMachine.OnStateChangedEventArgs e)
    {
        if(e.state==playerStateMachine.idleState){
            animator.SetBool(IS_WALKING,false);
        } else if(e.state==playerStateMachine.movingState){
            animator.SetBool(IS_WALKING,true);
        }
    }

}
