using System.Collections;
using _Project.Scripts.Defines;
using UnityEngine;

public class ParticleEffectBehaviour : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem _particleSystem;
    [SerializeField]
    private ParticleSystemRenderer _renderer;

    private ParticleSystem.MinMaxCurve _initStartRotationZ;

    private void Awake()
    {
        _particleSystem = (_particleSystem == null) ? GetComponent<ParticleSystem>() : _particleSystem;
        _renderer =  (_renderer == null) ? _particleSystem.GetComponent<ParticleSystemRenderer>() : _renderer;

        var main = _particleSystem.main;
        _initStartRotationZ = main.startRotationZ;
    }
    private void OnEnable()
    {
        _particleSystem.Play();
        StartCoroutine(DestroyWhenEnd());
    }

    private void OnDisable()
    {
        var main = _particleSystem.main;
        main.startRotationZ = _initStartRotationZ;
        _renderer.flip = Vector3.zero;
    }

    private IEnumerator DestroyWhenEnd()
    {
        while (_particleSystem.isPlaying)
        {
            yield return null;
        }

        ResourceManager.Instance.Destroy(gameObject);
    }

    public void SetDirection(Direction direction)
    {
        if (direction == Direction.Left)
        {
            _renderer.flip = new Vector3(1, 0, 0);
            var main = _particleSystem.main;
            main.startRotationZ = _initStartRotationZ.constant * -1;
        }
        else
        {
            _renderer.flip = Vector3.zero;
        }
    }
}
