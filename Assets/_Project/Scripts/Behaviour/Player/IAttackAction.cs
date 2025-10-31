using System.Collections.Generic;
using UnityEngine;

public interface IAttackAction<T>
{
    public void AttackAction(HashSet<T> targets);

    public void SkillAction(HashSet<T> targets);
}

