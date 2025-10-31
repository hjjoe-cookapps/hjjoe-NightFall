using System.Collections.Generic;

public class KnightBehaviour : UnitBehaviour
{
    public override void AttackAction(HashSet<MonsterBehaviour> targets)
    {
        foreach (var target in targets)
        {
            target.gameObject.GetRoot().GetComponent<HPModule>()?.TakeDamage(_status.Damage, gameObject);
        }
    }

    public override void SkillAction(HashSet<MonsterBehaviour> targets)
    {
    }
}
