using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    BaseState currentState;

    void Start()
    {
        currentState = GetInitialState();
        if (currentState != null)
            currentState.Start();
    }

    void Awake(){
        if (currentState != null)
            currentState.Awake();
    }

    void Update()
    {
        if (currentState != null)
            currentState.Update();
    }


    protected virtual BaseState GetInitialState()
    {
        return null;
    }

    public virtual void ChangeState(BaseState newState)
    {
        currentState.Exit();

        currentState = newState;
        newState.Enter();
    }

    /* Debug Option for putting current state on UI
    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10f, 10f, 200f, 100f));
        string content = currentState != null ? currentState.name : "(no current state)";
        GUILayout.Label($"<color='black'><size=40>{content}</size></color>");
        GUILayout.EndArea();
    }
    */
}