using System;
using System.Collections.Generic;
using CookApps.Inspector;
using UnityEngine;
using UnityEngine.UI;

// 다음 테크 건설 가능시 건설 건설 UI 활성화 기능 -> 처리는 GameManager가 한다 치고

public abstract class BuildingBehaviour : MonoBehaviour
{
    #region variable

    //TODO : SerializeField 지우기
    [SerializeField]
    protected BuildingStatus _status;
    [SerializeField]
    protected HPModule _hpModule;
    [SerializeField]
    protected Slider _slider;

    protected BuildingState _state;

    private int _level;

    // 테크트리 추가되면 buildUI를 또 처리해야되는데 어케하지

    [SerializeField]
    protected List<GameObject> _default;    // List로 변경
    [Required]
    [SerializeField]
    private GameObject _destroyed;
    [Required]
    [SerializeField]
    private BuildCarpetBehaviour _buildCarpet;

    #endregion

    #region property

    public int Level
    {
        get => _level;
        private set
        {
            _level = value;
            _status = SpecDataManager.Instance.GetBuildingStatus(this, _level);
            _buildCarpet.OnUpgrade(SpecDataManager.Instance.UpgradeStatus[_status!.UpgradeID]!.Cost);
        }
    }

    #endregion

    #region event

    private void Awake()
    {
        _hpModule = _hpModule == null ?  GetComponent<HPModule>() : _hpModule;
    }

    protected virtual void Start()
    {
        Level = 0;
        _state = BuildingState.Wait;

        _hpModule.OnDeadEvent -= OnBuildingDestroy;
        _hpModule.OnDeadEvent += OnBuildingDestroy;

        //_hpModule.OnDamageEvent -= UpdateHpUI;
        //_hpModule.OnDamageEvent += UpdateHpUI;
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
        _hpModule.Init(_status.HP);

        _buildCarpet.gameObject.SetActive(false);
    }

    public virtual void EndWave() // 웨이브 종료시 호출
    {
        _state = BuildingState.Wait;
        _hpModule.Reset();

        if (_default[Level] != null)
        {
            _default[Level].SetActive(true);
        }
        _destroyed.SetActive(false);

        if (_status.UpgradeID != 0)
        {
            _buildCarpet.gameObject.SetActive(true);
        }

        // 체력 초기화?
    }

    public virtual void Upgrade()
    {
        // 돈처리
        if (_default[Level] != null)
        {
            _default[Level].SetActive(false);
        }
        ++Level;    // buildcarpet update 포함됨
        _default[Level].SetActive(true);
    }

    // Idle에서의 동작
    protected virtual void Active()
    {
    }

    protected virtual void OnBuildingDestroy()
    {
        _state = BuildingState.Crash;
        // 충돌체 끄기
        if (_default[Level] != null)
        {
            _default[Level].SetActive(false);
        }

        _destroyed.SetActive(true);
    }

    //private void UpdateHpUI()
    //{
    //    _slider.value = _hpModule.HP / _hpModule.MaxHP;
    //
    //    if (_hpModule.HP == _hpModule.MaxHP)
    //    {
    //        _slider.gameObject.SetActive(false);
    //    }
    //    else
    //    {
    //        _slider.gameObject.SetActive(true);
    //    }
    //}
}
