using System.Collections;
using System.Linq;
using _Project.Scripts.Utils;
using UnityEngine;

public class PlayerStateAttack : PlayerStateBase
{
    private Coroutine _coroutine;
    public PlayerStateAttack(StateMachine<PlayerState> stateMachine, PlayerBehaviour context) : base(stateMachine, context)
    {
    }

    public override void Enter()
    {
        _coroutine = _context.StartCoroutine(AttackCoroutine());
    }

    public override void Execute()
    {
        UpdateState();
    }

    public override void Exit()
    {
        _context.StopCoroutine(_coroutine);
    }

    private void UpdateState()
    {
        if (_context.InRadiusMonsters.Count == 0)
        {
            _context.StateMachine.ChangeState(PlayerState.Idle);
        }
        else if (_context.IsSkillActive)
        {
            _context.StateMachine.ChangeState(PlayerState.Skill);
        }
    }

    private IEnumerator AttackCoroutine()
    {
        while (true)
        {
            yield return CoroutineManager.WaitForSeconds(_context.Cooltime);
            Attack();
        }
    }

    private void Attack()
    {
        _context.InRadiusMonsters.Take(_context.TargetCount).ToList();

        //Todo: instantiate Effects;
        // Set AttackMotion;

    }


}
