using _Project.Scripts.Defines;

public class MonsterStateDead : MonsterStateBase
{
    public MonsterStateDead(StateMachine<MonsterState> stateMachine, MonsterBehaviour context) : base(stateMachine, context)
    {
    }

    public override void Enter()
    {
        _context.Agent.ResetPath();
        _context.Animator.Play("Death");
    }

    public override void Execute()
    {
        // 애니메이션 종료시 사망 처리
    }

    public override void Exit()
    {
    }

}
