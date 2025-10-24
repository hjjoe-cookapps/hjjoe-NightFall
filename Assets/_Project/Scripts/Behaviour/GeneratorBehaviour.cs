using System;
using System.Collections;
using _Project.Scripts.Defines;
using UnityEngine;

[Serializable]
public struct GeneratorStatus
{
    public MonsterType Type;
    public int GenerateCount;
    public Vector3 Position;
}

public class GeneratorBehaviour : MonoBehaviour
{
    private static readonly float _generateDelay = 0.1f;

    [SerializeField]
    private GeneratorStatus _status;
    [SerializeField]
    private GameObject _generateObject;

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
           GameObject monster = ResourceManager.Instance.Instantiate("Monster/" + _status.Type.ToDescription(), _status.Position);
           ActionModule module = monster.GetOrAddComponent<ActionModule>();
           module.OnDisableEvent -= GameManager.Instance.OnMonsterDestroyAction;
           module.OnDisableEvent += GameManager.Instance.OnMonsterDestroyAction;

           yield return CoroutineManager.WaitForSeconds(_generateDelay);
        }

        ResourceManager.Instance.Destroy(gameObject);
    }

}
