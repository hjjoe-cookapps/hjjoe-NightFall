using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Pool
{
    public GameObject Original { get; private set; }
    public Transform Root;
    private readonly Queue<PoolMonoBehaviour> _poolQueue = new Queue<PoolMonoBehaviour>();

    public void Init(GameObject origin, int count = 20)
    {
        Original = origin;
        Root = new GameObject($"{Original.name}_root").transform;
        for (int i = 0; i < count; ++i)
        {
            Enqueue(Create());
        }
    }

    public PoolMonoBehaviour Create(Vector3 position = default)
    {
        GameObject obj = Object.Instantiate(Original, position, Quaternion.identity);
        obj.name = Original.name;

        return obj.GetOrAddComponent<PoolMonoBehaviour>();
    }

    public void Enqueue(PoolMonoBehaviour PoolMonoBehaviour)
    {
        if (PoolMonoBehaviour == null)
        {
            return;
        }

        PoolMonoBehaviour.gameObject.SetActive(false);
        PoolMonoBehaviour.transform.SetParent(Root);


        if (!_poolQueue.Contains(PoolMonoBehaviour))
        {
            _poolQueue.Enqueue(PoolMonoBehaviour);
        }
    }

    public PoolMonoBehaviour Dequeue(Transform transform = null)
    {
        PoolMonoBehaviour poolMonoBehaviour;
        if (_poolQueue.Count > 0)
        {
            poolMonoBehaviour = _poolQueue.Dequeue();
        }
        else
        {
            poolMonoBehaviour = Create();
        }

        poolMonoBehaviour.transform.parent = transform;
        if (transform == null)
        {
            SceneManager.MoveGameObjectToScene(poolMonoBehaviour.gameObject, SceneManager.GetActiveScene());
        }

        poolMonoBehaviour.gameObject.SetActive(true);

        return poolMonoBehaviour;
    }

    public PoolMonoBehaviour Dequeue(Vector3 position)
    {
        PoolMonoBehaviour poolMonoBehaviour;
        if (_poolQueue.Count > 0)
        {
            poolMonoBehaviour = _poolQueue.Dequeue();
            poolMonoBehaviour.transform.parent = null;
            SceneManager.MoveGameObjectToScene(poolMonoBehaviour.gameObject, SceneManager.GetActiveScene());
            poolMonoBehaviour.transform.position = position;
        }
        else
        {
            poolMonoBehaviour = Create(position);
        }

        poolMonoBehaviour.gameObject.SetActive(true);

        return poolMonoBehaviour;
    }

    public void Clear()
    {
        foreach (PoolMonoBehaviour obj in _poolQueue)
        {
            Object.Destroy(obj.gameObject);
        }

        _poolQueue.Clear();

        Object.Destroy(Root.gameObject);
        Original = null;
    }
}
