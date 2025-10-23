using System;
using _Project.Scripts.Defines;
using UnityEngine;


[Serializable]
public struct BuildingStatus
{
    public BulidingType Type;
    public int HP;
    public int CoinReward;

    // 타워용 전투 관련 어쩌구저쩌구

    // 업그레이드 관련 데이터?


    // 병영 관련 데이터



}

// 테크트리 구현은 나중에 처리
[Serializable]
public struct UpgradeStatus
{

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
    }

    private void Start()
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

    public virtual void StartBattle()   // 전투 시작시 동작
    {
        _state = BuildingState.Idle;
    }

    // Idle에서의 동작
    protected virtual void Active()
    {
    }


    protected virtual void Regenerate() // 웨이브 종료시 호출
    {
        _state = BuildingState.Wait;
        // status 초기화
    }
    protected virtual void OnDestroy()
    {
        _state = BuildingState.Crash;
        // 충돌체 끄기
    }
}

