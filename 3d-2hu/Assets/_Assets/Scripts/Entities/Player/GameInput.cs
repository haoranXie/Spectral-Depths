using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour
{
    private const string PLAYER_PREFS_BINDINGS = "InputBindings";
    public enum Binding{
        Move_Up,
        Move_Down,
        Move_Left,
        Move_Right,
        Pause,
    }

    private PlayerInputActions playerInputActions;
    public event EventHandler OnPauseAction;
    public static GameInput Instance {get;private set;}

    private void Awake(){
        if(Instance==null){
            Instance = this;
        } else{
            Debug.LogWarning("Another instance of "+typeof(GameInput)+" already exists.");
            Destroy(this.gameObject);
        }
        playerInputActions = new PlayerInputActions();
        if(PlayerPrefs.HasKey(PLAYER_PREFS_BINDINGS)){ //Checks if there are saved bindings and loads them
            playerInputActions.LoadBindingOverridesFromJson(PlayerPrefs.GetString(PLAYER_PREFS_BINDINGS));
        }

        //DontDestroyOnLoad(this.gameObject);
    }

    //Make to sure to remove all listeners
    private void OnDestroy(){
        playerInputActions.Dispose();
    }

    public Vector2 GetMovementVectorNormalized(){
        Vector2 inputVector = playerInputActions.Player.Move.ReadValue<Vector2>(); //Auto Normalized
        return inputVector;
    }
}
