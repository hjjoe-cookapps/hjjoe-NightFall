using System.Collections;
using System.Collections.Generic;
using CookApps.Inspector;
using UnityEngine;
using Slider = UnityEngine.UI.Slider;

public class BarracksBehaviour : BuildingBehaviour
{
    private static readonly float _createTime = 5f;

    [SerializeField]
    private UnitType _unitType;

    private int _maxUnitCount;
    private readonly HashSet<UnitBehaviour> _unitBehaviours = new();

    private Coroutine _createUnitCoroutine;

    [Required]
    [SerializeField]
    private Transform _createPoint;
    [Required]
    [SerializeField]
    private Slider _createSlider;


    public override void StartWave()
    {
        base.StartWave();
        foreach (var unit in _unitBehaviours)
        {
            unit.StartWave();
        }

        _createUnitCoroutine = StartCoroutine(CreateUnitCoroutine());
    }

    public override void EndWave()
    {
        base.EndWave();
        foreach (var unit in _unitBehaviours)
        {
            unit.EndWave();
        }

        this.StopAndNullifyRef(ref _createUnitCoroutine);
        _createSlider.gameObject.SetActive(false);

        if (Level != 0)
        {
            CreateUnit(_maxUnitCount -  _unitBehaviours.Count);
        }
    }

    public override void Upgrade()
    {
        base.Upgrade();

        _maxUnitCount = Level * 4;
        CreateUnit(_maxUnitCount -  _unitBehaviours.Count);
    }

    public void OnUnitDestroyed(UnitBehaviour unit)
    {
        _unitBehaviours.Remove(unit);
    }

    protected override void OnBuildingDestroy()
    {
        base.OnBuildingDestroy();
        this.StopAndNullifyRef(ref _createUnitCoroutine);
        _createSlider.gameObject.SetActive(false);
    }

    private void CreateUnit(int count = 1)
    {
        for (int i = 0; i < count; i++)
        {
            Vector2 randomDir2D = Random.insideUnitCircle.normalized * 0.1f;
            UnitBehaviour unitBehaviour = ResourceManager.Instance.Instantiate("Unit/" + _unitType.ToDescription(), _createPoint.position + (Vector3)randomDir2D).GetComponent<UnitBehaviour>();
            unitBehaviour.Barracks = _createPoint;
            unitBehaviour.OnDeadEvent -= OnUnitDestroyed;
            unitBehaviour.OnDeadEvent += OnUnitDestroyed;
            _unitBehaviours.Add(unitBehaviour);

            if (_state == BuildingState.Idle)
            {
                unitBehaviour.StartWave();
            }
        }

        Physics2D.Simulate(Time.fixedDeltaTime);
    }

    private IEnumerator CreateUnitCoroutine()
    {
        while (true)
        {
            if (_unitBehaviours.Count < _maxUnitCount)
            {
                _createSlider.gameObject.SetActive(true);
                // 1. ui 활성화
                yield return StartCoroutine(ProgressBar());
                CreateUnit();
                if (_unitBehaviours.Count >= _maxUnitCount)
                {
                    _createSlider.gameObject.SetActive(false);
                }
            }

            yield return null;
        }
    }

    private IEnumerator ProgressBar()
    {
        float time = 0f;
        while (time < _createTime)
        {
            time += Time.deltaTime;
            _createSlider.value = time / _createTime;
            yield return null;
        }
    }
}
