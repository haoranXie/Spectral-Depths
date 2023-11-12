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
    public event EventHandler OnBindingRebind;
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
        playerInputActions.Enable();

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

    public string GetBindingText(Binding binding){
        switch(binding){
            default:
            case Binding.Move_Up:
                return playerInputActions.Player.Move.bindings[1].ToDisplayString();
            case Binding.Move_Down:
                return playerInputActions.Player.Move.bindings[2].ToDisplayString();
            case Binding.Move_Left:
                return playerInputActions.Player.Move.bindings[3].ToDisplayString();
            case Binding.Move_Right:
                return playerInputActions.Player.Move.bindings[4].ToDisplayString();
        }
    }

    public void RebindBinding(Binding binding, Action onActionRebound){
        playerInputActions.Player.Disable();
        InputAction inputAction;
        int bindingIndex;

        switch(binding){
            default:
            case Binding.Move_Up:
                inputAction = playerInputActions.Player.Move;
                bindingIndex=1;
                break;
            case Binding.Move_Down:
                inputAction = playerInputActions.Player.Move;
                bindingIndex=2;
                break;
            case Binding.Move_Left:
                inputAction = playerInputActions.Player.Move;
                bindingIndex=3;
                break;
            case Binding.Move_Right:
                inputAction = playerInputActions.Player.Move;
                bindingIndex=4;
                break;
        }

        inputAction.PerformInteractiveRebinding(bindingIndex)
            .OnComplete(callback=>{
                //callback.action.bindings[1];
                callback.Dispose();
                playerInputActions.Player.Enable();
                onActionRebound();
                PlayerPrefs.SetString(PLAYER_PREFS_BINDINGS,playerInputActions.SaveBindingOverridesAsJson());
                PlayerPrefs.Save();

                OnBindingRebind?.Invoke(this,EventArgs.Empty);
            }).Start();
    }
}
