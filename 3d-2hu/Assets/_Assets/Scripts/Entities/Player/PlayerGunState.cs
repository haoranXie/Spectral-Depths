using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PlayerGunState : BaseState
{

    [SerializeField] private float acceleration = 500f;
    [SerializeField] private float maxSpeed = 300f;
    [SerializeField] private float friction = 70f;
    [SerializeField] private float rotateSpeed = 20f;

    [SerializeField] private float idleThreshold = 0.1f; //How slow player should be before changing to IdleState

    private Vector3 velocity;
    private Vector3 lookPos;
    private Transform player;
    public PlayerGunState(PlayerStateMachine stateMachine) : base("Moving",stateMachine){}
    public override void Enter()
    {
        base.Enter();

    }

    public override void Awake()
    {
        base.Awake();
        player = ((PlayerStateMachine)stateMachine).player; 
    }

    public override void Update()
    {
        base.Update();
        HandleMovement();
        AimAtMouse();
    }

    private void AimAtMouse(){
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        float range = 100f; //Sets Range
        if(Physics.Raycast(ray,out hit,range)){
            lookPos = hit.point;
        }
        Vector3 lookDir=lookPos-player.transform.position;
        lookDir.y=0;
        player.transform.LookAt(player.transform.position+lookDir,Vector3.up);
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
        player.transform.Translate(velocity*Time.deltaTime,Space.World);
        //((PlayerStateMachine)stateMachine).player.GetComponent<Rigidbody>().MovePosition(((PlayerStateMachine)stateMachine).player.transform.position+velocity*Time.deltaTime);

        //Turns Player based on movement keys
        ((PlayerStateMachine)stateMachine).player.transform.forward  = Vector3.Slerp(((PlayerStateMachine)stateMachine).player.transform.forward,movementDir,Time.deltaTime*rotateSpeed);
        //If Player is slow enough change to idleState
    }
}
