
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.Defines;
using UnityEngine;

public class TowerBehaviour : BuildingBehaviour
{
    [SerializeField]
    private TowerStatus _towerStatus;
    [SerializeField]
    private Transform _arrowGenerateTransform;

    private List<MonsterBehaviour> _inRadiusMonsters = new();
    private bool _isAttackAble = true;
    private Coroutine _scanRadiusMonsterCoroutine;
    private Coroutine _checkTimeCoroutine;

    //Todo: Temp
    protected override void Start()
    {
        base.Start();
        StartBattle(); // 반드시 지워야함 !!!!!!!!!!!!!!!!!!!
    }

    public override void StartBattle()
    {
        base.StartBattle();
        _scanRadiusMonsterCoroutine = StartCoroutine(ScanRadiusMonsters());
        _checkTimeCoroutine = StartCoroutine(CheckTime());
    }

    protected override void Active()
    {
        if (_inRadiusMonsters.Count > 0 && _isAttackAble)
        {
            _isAttackAble = false;
            Attack();
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        StopCoroutine(_scanRadiusMonsterCoroutine);
        StopCoroutine(_checkTimeCoroutine);
    }

    private void UpdateRadiusMonsters()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, _towerStatus.Range, Defines.MonsterLayer);
        var inRadiusMonsters = new List<(MonsterBehaviour monster, float dist)>();

        foreach (Collider collider in hitColliders)
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
            ArrowBehaviour arrow = ResourceManager.Instance.Instantiate("Effect/Arrow", _arrowGenerateTransform.position).GetOrAddComponent<ArrowBehaviour>();
            arrow.Init(gameObject, _towerStatus.Damage, transform.position, monster.BodyTransform);
        }

    }


    private IEnumerator ScanRadiusMonsters()
    {
        while (true)
        {
            UpdateRadiusMonsters();
            yield return CoroutineManager.WaitForSeconds(0.2f);
        }
    }

    private IEnumerator CheckTime()
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

}
