
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.Defines;
using CookApps.Inspector;
using UnityEngine;

public class TowerBehaviour : BuildingBehaviour
{
    [SerializeField]
    private TowerStatus _towerStatus;
    [Required]
    [SerializeField]
    private Transform _arrowGenerateTransform;

    private List<MonsterBehaviour> _inRadiusMonsters = new();
    private bool _isAttackAble = true;
    private Coroutine _scanRadiusMonsterCoroutine;
    private Coroutine _checkTimeCoroutine;

    public override void StartWave()
    {
        base.StartWave();
        _scanRadiusMonsterCoroutine = StartCoroutine(ScanRadiusMonstersCoroutine());
        _checkTimeCoroutine = StartCoroutine(CheckTimeCoroutine());
    }

    protected override void Active()
    {
        if (Level != 0 && _inRadiusMonsters.Count > 0 && _isAttackAble)
        {
            _isAttackAble = false;
            Attack();
        }
    }

    protected override void OnBuildingDestroy()
    {
        base.OnBuildingDestroy();
        if (_scanRadiusMonsterCoroutine != null)
        {
            StopCoroutine(_scanRadiusMonsterCoroutine);
        }

        StopCoroutine(_checkTimeCoroutine);
    }

    private void UpdateRadiusMonsters()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, _towerStatus.Range, Defines.MonsterLayer);
        var inRadiusMonsters = new List<(MonsterBehaviour monster, float dist)>();

        foreach (Collider2D collider in hitColliders)
        {
            if (collider.gameObject.TryGetComponent(out MonsterBehaviour monsterBehaviour))
            {
                inRadiusMonsters.Add(
                    (monsterBehaviour, (transform.position - collider.transform.position).sqrMagnitude));
            }
        }

        _inRadiusMonsters = inRadiusMonsters.OrderBy(md => md.dist).Select(md => md.monster).ToList();
    }

    private void Attack()
    {
        HashSet<MonsterBehaviour> targets = _inRadiusMonsters.Take(_towerStatus.TargetCount).ToHashSet();

        foreach (var monster in targets)
        {
            ArrowBehaviour arrow = ResourceManager.Instance.Instantiate("VFX/Arrow", _arrowGenerateTransform.position).GetOrAddComponent<ArrowBehaviour>();
            arrow.Init(gameObject, _towerStatus.Damage, transform.position, monster.CenterTransform);
        }
    }

    private IEnumerator ScanRadiusMonstersCoroutine()
    {
        while (true)
        {
            UpdateRadiusMonsters();
            yield return CoroutineManager.WaitForSeconds(0.2f);
        }
    }

    private IEnumerator CheckTimeCoroutine()
    {
        while (true)
        {
            if (!_isAttackAble)
            {
                yield return CoroutineManager.WaitForSeconds(_towerStatus.WaitTime);
                _isAttackAble = true;
            }

            yield return null;
        }
    }

    public override void Upgrade()
    {
        base.Upgrade();
        _towerStatus = SpecDataManager.Instance.GetTowerStatus(Level);
    }
}
