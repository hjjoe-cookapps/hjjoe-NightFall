using System;
using System.Collections;
using _Project.Scripts.Defines;
using UnityEngine;


public class GeneratorBehaviour : MonoBehaviour
{
    private static readonly float _generateDelay = 1f;

    [SerializeField]
    private GeneratorStatus _status;

    public event Action<GeneratorBehaviour> OnDisableAction;

    public void Init(GeneratorStatus status)
    {
        _status = status;
        StartCoroutine(GenerateMonster());
    }

    private void OnDisable()
    {
        OnDisableAction?.Invoke(this);
        StopAllCoroutines();
    }

    private IEnumerator GenerateMonster()
    {
        for (int i = 0; i < _status.GenerateCount; ++i)
        {
           MonsterBehaviour monster = ResourceManager.Instance.Instantiate("Monster/" + _status.Type.ToDescription(), _status.Position).GetComponent<MonsterBehaviour>();
           monster.OnDeadEvent -= GameManager.Instance.OnMonsterDisableAction;
           monster.OnDeadEvent += GameManager.Instance.OnMonsterDisableAction;

           yield return CoroutineManager.WaitForSeconds(_generateDelay);
        }

        ResourceManager.Instance.Destroy(gameObject);
    }

}
