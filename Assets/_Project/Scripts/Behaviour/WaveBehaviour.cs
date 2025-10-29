using System.Collections;
using System.Collections.Generic;
using _Project.Scripts.Defines;
using UnityEngine;

public class WaveBehaviour : MonoBehaviour
{
    private static readonly float _troopDelay = 10f; // 모든 제너레이터 삭제 이후 delay 경과, 다음 제너레이터 생성

    [SerializeField]
    private WaveStatus _status;
    private readonly List<GeneratorBehaviour> _generators = new();

    public void Init(WaveStatus status)
    {
        _status = status;
        StartCoroutine(Wave());
    }

    private void OnGeneratorDestroyAction(GeneratorBehaviour generator)
    {
        _generators.Remove(generator);
    }

    private IEnumerator Wave()
    {
        for (int i = 0; i < _status.Troops.Count; i++)
        {
            GenerateGenerators(i);

            while (_generators.Count != 0)
            {
                yield return null;
            }

            if (i != _status.Troops.Count - 1)
            {
                yield return new WaitForSeconds(_troopDelay);
            }
        }

        ResourceManager.Instance.Destroy(gameObject);
    }

    private void GenerateGenerators(int currentTroopCount)
    {
        foreach (GeneratorStatus status in _status.Troops[currentTroopCount].Generators)
        {
            var generator = ResourceManager.Instance.Instantiate("Generator", transform).GetOrAddComponent<GeneratorBehaviour>();
            generator.Init(status);
            generator.OnDisableAction -= OnGeneratorDestroyAction;
            generator.OnDisableAction += OnGeneratorDestroyAction;

            _generators.Add(generator);
        }
    }
}
