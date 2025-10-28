using System.Collections;
using _Project.Scripts.Defines;
using UnityEngine;

public class PlayerStateSkill : PlayerStateBase
{
    private Coroutine _coroutine;

    public PlayerStateSkill(StateMachine<PlayerState> stateMachine, PlayerBehaviour context) : base(stateMachine, context)
    {
    }

    public override void Enter()
    {
        _coroutine = _context.StartCoroutine(SkillCoroutine());

    }

    public override void Execute()
    {
        UpdateState();
    }

    public override void Exit()
    {
        _context.ExternCharacterScript.Animator.Play("IdleMelee");
        _context.ExternCharacterScript.Animator.ResetTrigger("Slash");
        _context.ExternCharacterScript.Animator.SetBool("Action", false);

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

    private IEnumerator SkillCoroutine()
    {
        while (true)
        {
            _context.ExternCharacterScript.Slash();
            yield return CoroutineManager.WaitForSeconds(_context.WaitTime);
        }
    }
}
