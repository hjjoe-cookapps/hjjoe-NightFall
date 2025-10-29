using _Project.Scripts.Defines;
using UnityEngine;

public class UnitStateReturn : UnitStateBase
{
    public UnitStateReturn(StateMachine<UnitState> stateMachine, UnitBehaviour context) : base(stateMachine, context)
    {
    }

    public override void Enter()
    {
    }

    public override void Execute()
    {

    }

    public override void Exit()
    {

    }

    private void Move()
    {
        Vector3 direction = _context.Barracks.transform.position - _context.transform.position;
        float magnitude = direction.magnitude;
        direction.Normalize();
        _context.transform.position = _context.transform.position + direction * _context.Status.MoveSpeed;

        if (magnitude < 0.2f)
        {
            _context.StateMachine.ChangeState(UnitState.Idle);
        }
    }
}
