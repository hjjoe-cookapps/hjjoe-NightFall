using System.Collections;
using System.Linq;
using _Project.Scripts.Defines;
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
        // exit current animation
        _context.Character.Animator.Play("IdleMelee");
        _context.Character.Animator.ResetTrigger("Jab");
        _context.Character.Animator.SetBool("Action", false);

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
            _context.Character.Jab();
            yield return CoroutineManager.WaitForSeconds(_context.WaitTime);
        }
    }
}
