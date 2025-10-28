using System.Collections;
using _Project.Scripts.Defines;
using UnityEngine;

public class UnitStateAttack : UnitStateBase
{
    private Coroutine _coroutine;

    public UnitStateAttack(StateMachine<UnitState> stateMachine, UnitBehaviour context) : base(stateMachine, context)
    {
    }

    public override void Enter()
    {
        _coroutine = _context.StartCoroutine(AttackCoroutine());
    }

    public override void Execute()
    {

    }

    public override void Exit()
    {
    }

    private void UpdateState()
    {

    }

    private IEnumerator AttackCoroutine()
    {
        yield return null;
    }
}
