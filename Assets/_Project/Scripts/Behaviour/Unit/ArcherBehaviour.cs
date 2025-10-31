using System.Collections.Generic;
using CookApps.Inspector;
using UnityEngine;

public class ArcherBehaviour : UnitBehaviour
{
    [Required]
    [SerializeField]
    private Transform  _arrowGenerateTransform;

    public override void AttackAction(HashSet<MonsterBehaviour> targets)
    {
        foreach (MonsterBehaviour target in targets)
        {
            ArrowBehaviour arrow = ResourceManager.Instance.Instantiate("VFX/Arrow", _arrowGenerateTransform.position).GetOrAddComponent<ArrowBehaviour>();
            arrow.Init(gameObject, _status.Damage, transform.position, target.CenterTransform);
        }
    }

    public override void SkillAction(HashSet<MonsterBehaviour> targets)
    {
    }
}

