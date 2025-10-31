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
        Move();
    }

    public override void Exit()
    {

    }

    private void Move()
    {
        Vector3 direction = _context.Barracks.position - _context.transform.position;
        float magnitude = direction.magnitude;
        direction.Normalize();
        _context.Rigidbody.linearVelocity = direction * _context.Status.MoveSpeed;

        if (magnitude < 1f)
        {
            _context.StateMachine.ChangeState(UnitState.Idle);
        }
    }
}
