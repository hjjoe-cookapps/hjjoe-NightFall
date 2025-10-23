using System.Collections.Generic;

public interface IAttackAction
{
    public void AttackAction(HashSet<MonsterBehaviour> targets);

    public void SkillAction(HashSet<MonsterBehaviour> targets);
}

