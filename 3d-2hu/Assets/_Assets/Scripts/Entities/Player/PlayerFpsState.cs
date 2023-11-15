using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFpsState: BaseState
{
    [SerializeField] private float acceleration = 500f;
    [SerializeField] private float maxSpeed = 300f;
    [SerializeField] private float friction = 70f;

    [SerializeField] private float notMovingThreshold = 0.1f; //How slow player should be before changing to IdleState

    private Vector3 velocity;
    public event EventHandler PlayerFpsStateNotMoving;
    public event EventHandler PlayerFpsStateIsMoving;

    public PlayerFpsState(PlayerStateMachine stateMachine) : base("FPS",stateMachine){}
    public override void Enter()
    {
        base.Enter();
    }

    public override void Update()
    {
        base.Update();
        HandleMovement();
        if(velocity.magnitude<notMovingThreshold){
            PlayerFpsStateNotMoving?.Invoke(this, EventArgs.Empty);
        } else{
            PlayerFpsStateIsMoving?.Invoke(this,EventArgs.Empty);
        }
    }

    private void HandleMovement(){
        Vector2 inputVector = GameInput.Instance.GetMovementVectorNormalized();
        Vector3 movementDir = new Vector3(inputVector.x,0f,inputVector.y);
        movementDir.Normalize();
        //Apply Acceleration
        velocity+=movementDir*acceleration*Time.deltaTime; 
        //Limit Speed to Maximum Speed
        velocity = Vector3.ClampMagnitude(velocity,maxSpeed); 
        //Apply Friction to gradually slow down
        velocity -= velocity*friction*Time.deltaTime;
        //Translates Player based on velocity
        ((PlayerStateMachine)stateMachine).player.transform.Translate(velocity*Time.deltaTime,Space.World);
        //((PlayerStateMachine)stateMachine).player.GetComponent<Rigidbody>().MovePosition(((PlayerStateMachine)stateMachine).player.transform.position+velocity*Time.deltaTime);

        //Turns Player based on movement keys
        //((PlayerStateMachine)stateMachine).player.transform.forward  = Vector3.Slerp(((PlayerStateMachine)stateMachine).player.transform.forward,movementDir,Time.deltaTime*rotateSpeed);
        //If Player is slow enough change to idleState
    }
}
