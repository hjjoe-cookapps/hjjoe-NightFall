
using System.Collections.Generic;
using UnityEngine;

public class GameManager : SingletonBehaviour<GameManager>
{
    [SerializeField]
    private GameObject _castle;

    [SerializeField]
    private GameObject _player;

    private HashSet<MonsterBehaviour> _monsters;
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

    private void OnMonsterDestroyAction(MonsterBehaviour monster)
    {
        if (_monsters.Remove(monster))
        {
            --_waveCount;
        }
    }


}
