using System;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine<TState> where TState : struct, Enum
{
    private Dictionary<TState, State<TState>> states = new Dictionary<TState, State<TState>>();
    private State<TState> curState = null;
    private TState curStateType = default;

    public State<TState> CurState { get { return curState; } }
    public TState CurStateType { get { return curStateType; } set { ChangeState(value); } }

    public T RegisterState<T>(TState stateType, MonoBehaviour context) where T : State<TState>
    {
        T state = Activator.CreateInstance(typeof(T), this, context) as T;
        states[stateType] = state;

        return state;
    }

    public T GetState<T>(TState stateType) where T : State<TState>
    {
        if (states.ContainsKey(stateType) && states[stateType] is T)
        {
            return states[stateType] as T;
        }

        return null;
    }

    public void ChangeState(TState stateType)
    {
        if (!states.ContainsKey(stateType))
        {
            Debug.Log($"Change Undefined State {stateType.ToString()}");
            return;
        }

        if (curState == states[stateType])
        {
            return;
        }

        State<TState> preState = curState;
        curState = states[stateType];
        curStateType = stateType;

        if (preState != null)
        {
            preState.Exit();
        }

        if (curState != null)
        {
            curState.Enter();
        }
    }

    public void Execute()
    {
        if (curState != null)
        {
            curState.Execute();
        }
    }
}
