using System.Collections.Generic;
using UnityEngine;

public class GameManager : SingletonBehaviour<GameManager>
{
    [SerializeField]
    private GameObject _castle;

    [SerializeField]
    private GameObject _player;

    private HashSet<GameObject> _monsters;  // 필요한가?

    private HashSet<BuildingBehaviour> _buildings;
    private int _waveCount;

    private int _coin = 10; // TODO : 10 지우기

    public GameObject Castle => _castle;
    public GameObject Player => _player;

    private void Start()
    {
        if (_player == null)
        {
            _player = GameObject.FindWithTag("Player");
        }
    }

    public void OnMonsterDestroyAction()
    {
        --_waveCount;

    }
}
