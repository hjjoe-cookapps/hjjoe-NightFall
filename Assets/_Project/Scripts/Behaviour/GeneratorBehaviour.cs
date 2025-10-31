using System;
using System.Collections;
using _Project.Scripts.Defines;
using UnityEngine;
using Random = UnityEngine.Random;

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
        Vector3 position = new Vector3(_status.X, _status.Y, 0);
        for (int i = 0; i < _status.Count; ++i)
        {
            Vector2 randomDir2D = Random.insideUnitCircle.normalized * 0.5f;
            MonsterBehaviour monster = ResourceManager.Instance
                .Instantiate("Monster/" + _status.Type.ToDescription(), position + (Vector3)randomDir2D)
                .GetComponent<MonsterBehaviour>();
            GameManager.Instance.AddMonster(monster.gameObject);
            monster.OnDeadEvent -= GameManager.Instance.OnMonsterDisableAction;
            monster.OnDeadEvent += GameManager.Instance.OnMonsterDisableAction;

            yield return CoroutineManager.WaitForSeconds(_generateDelay);
        }

        ResourceManager.Instance.Destroy(gameObject);
    }

}
