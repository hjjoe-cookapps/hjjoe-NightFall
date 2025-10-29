using System;
using System.Collections.Generic;
using _Project.Scripts.Defines;
using CookApps.Inspector;
using UnityEngine;


[Serializable]
public struct BuildingStatus
{
    public BulidingType Type;
    public int HP;

    // For Farm, castle
    public int CoinReward;

    // upgrade
    public int maxlevel;
    public int level;
    public UpgradeStatus[] upgradeStatuses;
    // 타워용 전투 관련 어쩌구저쩌구

    // 업그레이드 관련 데이터?


    // 병영 관련 데이터

}

// 테크트리 구현은 나중에 처리
[Serializable]
public struct UpgradeStatus
{
    public int level;   // 단계별
    public int Cost;
}

[Serializable]
public struct TowerStatus
{
    public bool IsSplash;
    public int Damage;
    public int WaitTime;
    public int Range;
    public int TargetCount;
}

[Serializable]
public struct BarracksStatus
{
    public UnitType Type;
    public int UnitCount;
    // 생산속도 : 모두 고정
}


// 고려사항 : 한번에 여러개 생성되는 경우는 어떻게 처리? -> farm

// 다음 테크 건설 가능시 건설 건설 UI 활성화 기능 -> 처리는 GameManager가 한다 치고


public abstract class BuildingBehaviour : MonoBehaviour
{
    #region variable

    [SerializeField]
    protected BuildingState _state;
    [SerializeField]
    protected BuildingStatus _buildingStatus;
    [SerializeField]
    protected HPModule _hpModule;

    // 1. 기본 스프라이트
    // 2. 파괴 상태 스프라이트
    // 밤에 비활성화 할 스픝라이트 모음
    // 3. 업그레이드 UI sprite -> 최종 인 경우에는 없어도 됨

    [SerializeField]
    private List<GameObject> OnBattleHiddenObjects =  new();

    // 테크트리 추가되면 buildUI를 또 처리해야되는데 어케하지
    //TODO : _buildUI는 건물 업그레이드 완료이후에는 낮에도, 밤에도 안보여야함

    [SerializeField]
    private GameObject _default;    // List로 변경
    [SerializeField]
    private GameObject _destroyed;
    [SerializeField]
    private GameObject _buildUI;

    #endregion

    #region property

    public BuildingState State
    {
        get => _state;
        set =>  _state = value;
    }
    public BuildingStatus BuildingStatus => _buildingStatus;
    public HPModule HPModule => _hpModule;

    #endregion

    #region event
    private void Awake()
    {
        _hpModule = _hpModule == null ?  GetComponent<HPModule>() : _hpModule;
    }

    protected virtual void Start()
    {
        _hpModule.OnDeadEvent -= OnDestroy;
        _hpModule.OnDeadEvent += OnDestroy;
    }

    private void Update()
    {
        if (_state == BuildingState.Idle)
        {
            Active();
        }
    }
    #endregion

    public virtual void StartWave()   // 전투 시작시 동작
    {
        _state = BuildingState.Idle;
        _hpModule.Init(_buildingStatus.HP);

        foreach (var obj in OnBattleHiddenObjects)
        {
            obj.SetActive(false);
        }

        _default?.SetActive(true);
        _destroyed?.SetActive(false);
        _buildUI?.SetActive(false);
    }

    public virtual void EndWave() // 웨이브 종료시 호출
    {
        _state = BuildingState.Wait;
        _hpModule.Reset();

        foreach (GameObject obj in OnBattleHiddenObjects)
        {
            obj.SetActive(true);
        }

        _default?.SetActive(true);
        _destroyed?.SetActive(false);
        _buildUI?.SetActive(true);
        // 체력 초기화?
    }

    // Idle에서의 동작
    protected virtual void Active()
    {
    }

    protected virtual void Upgrade()
    {

    }

    protected virtual void OnDestroy()
    {
        _state = BuildingState.Crash;
        // 충돌체 끄기
        _default?.SetActive(false);
        _destroyed?.SetActive(true);
    }
}
