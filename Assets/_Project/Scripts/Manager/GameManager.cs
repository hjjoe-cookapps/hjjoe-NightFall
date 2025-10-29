using System;
using System.Collections;
using System.Collections.Generic;
using _Project.Scripts.Defines;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;



public class GameManager : SingletonBehaviour<GameManager>
{
    [NotNull]
    [SerializeField]
    private GameObject _castle;
    [SerializeField]
    private GameObject _player;

    [SerializeField]
    private GameObject _waveStartButton;


    // 게임 로직
    private readonly HashSet<GameObject> _monsters = new();  // 필요한가?
    private WaveBehaviour _waveBehaviour;
    private readonly HashSet<BuildingBehaviour> _buildings = new();
    [SerializeField]    // TODO : erase
    private StageStatus _status;
    private int _currentWaveCount;

    // 게임 플레이용 재화
    private int _coin = 10; // TODO : 10 지우기

    #region property

    public GameObject Castle => _castle;
    public GameObject Player
    {
        get
        {
            if (_player == null)
            {
                _player = GameObject.FindWithTag("Player");
            }

            return _player;
        }
    }
    public IReadOnlyCollection<GameObject> Monsters => _monsters;

    public int Coin => _coin;

    #endregion

    private void Start()
    {
        if (_player == null)
        {
            _player = GameObject.FindWithTag("Player");
        }

        _currentWaveCount = 0;
    }

    private void Update()
    {

    }

    #region for Actions

    public void AddCoin()
    {
        ++_coin;
    }

    public void LoseCoin(int num)
    {
        _coin -= num;
    }

    public void OnMonsterDisableAction(GameObject gameObject)
    {
        _monsters.Remove(gameObject);
    }

    #endregion

    public void StartWave()
    {
        foreach (BuildingBehaviour building in _buildings)
        {
            building.StartWave();
        }

        _waveBehaviour = ResourceManager.Instance.Instantiate("Wave", transform).GetComponent<WaveBehaviour>();
        _waveBehaviour.Init(_status.Waves[_currentWaveCount]);

        StartCoroutine(CheckWave());
    }

    public void EndWave()
    {
        _waveBehaviour = null;
        ++_currentWaveCount;

        foreach (BuildingBehaviour building in _buildings)
        {
            building.EndWave();
        }

        if (_currentWaveCount >= _status.Waves.Count)
        {
            Debug.Log("Finish");
            // finish game
        }
    }

    public void Lose()
    {
        Debug.Log("Lose");
        // 패배 처리
    }

    private IEnumerator CheckWave()
    {
        while (true)
        {
            if (_waveBehaviour.IsDead() && _monsters.Count == 0)
            {
                EndWave();
                _waveStartButton.SetActive(true);
                yield break;
            }

            yield return null;
        }
    }

}
