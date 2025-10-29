using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.Defines;
using CookApps.Inspector;
using UnityEngine;

public class GameManager : SingletonBehaviour<GameManager>
{
    [Required]
    [SerializeField]
    private GameObject _castle;
    [SerializeField]
    private GameObject _player;

    [SerializeField]
    private GameObject _waveStartButton;


    // 게임 로직
    private readonly HashSet<GameObject> _monsters = new();  // 필요한가?
    private WaveBehaviour _waveBehaviour;
    private HashSet<BuildingBehaviour> _buildings;
    [SerializeField]    // TODO : erase
    private StageStatus _status;
    private int _currentWaveCount;

    // 게임 플레이용 재화
    private int _wood = 10; // TODO : 10 지우기

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

    public int Wood => _wood;

    #endregion

    private void Start()
    {
        if (_player == null)
        {
            _player = GameObject.FindWithTag("Player");
        }

        _buildings = GameObject.FindGameObjectsWithTag("Building").Select(obj => obj.GetComponent<BuildingBehaviour>())
            .ToHashSet();

        _currentWaveCount = 0;
    }

    private void Update()
    {

    }

    #region for Actions

    public void AddWood()
    {
        ++_wood;
    }

    public void LoseWood(int num)
    {
        _wood -= num;
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

    public bool AddMonster(GameObject monster)
    {
        return _monsters.Add(monster);
    }
}
