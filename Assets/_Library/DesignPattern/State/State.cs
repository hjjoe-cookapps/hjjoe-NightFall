using System;
using UnityEngine;

public abstract class State<TState> where TState : struct, Enum
{
    protected StateMachine<TState> stateMachine;

    public State(StateMachine<TState> stateMachine)
    {
        this.stateMachine = stateMachine;
    }

    public virtual void Enter()
    {
    }

    public virtual void Execute()
    {
    }

    public virtual void Exit()
    {
    }

}

public class StateDefault<TState> : State<TState> where TState : struct, Enum
{
    public StateDefault(StateMachine<TState> stateMachine, MonoBehaviour context) : base(stateMachine)
    { }
}
