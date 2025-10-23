using System.Collections.Generic;
using UnityEngine;

public class PoolManager : SingletonBehaviour<PoolManager>
{
    private readonly Dictionary<string, Pool> _poolDict = new();

    public void Enqueue(PoolMonoBehaviour obj)
    {
        if (obj != null && !_poolDict.ContainsKey(obj.name))
        {
            Object.Destroy(obj.gameObject);
            return;
        }

        _poolDict[obj.name].Enqueue(obj);
    }

    public PoolMonoBehaviour Dequeue(GameObject original, Transform transform = null)
    {
        if (original == null)
        {
            return null;
        }

        if (!_poolDict.ContainsKey(original.name))
        {
            CreatePool(original);
        }

        return _poolDict[original.name].Dequeue(transform);
    }

    public PoolMonoBehaviour Dequeue(GameObject original, Vector3 position)
    {
        if (original == null)
        {
            return null;
        }

        if (!_poolDict.ContainsKey(original.name))
        {
            CreatePool(original);
        }

        return _poolDict[original.name].Dequeue(position);
    }

    public GameObject GetOriginal(string name)
    {
        if (!_poolDict.ContainsKey(name))
        {
            return null;
        }

        return _poolDict[name].Original;
    }

    public void Clear()
    {
        foreach (var pool in _poolDict.Values)
        {
            pool.Clear();
        }
        _poolDict.Clear();
    }

    private void CreatePool(GameObject original)
    {
        if (original != null && !_poolDict.ContainsKey(original.name))
        {
            int count = original.GetOrAddComponent<PoolMonoBehaviour>().PoolCreateCount;

            Pool pool = new Pool();
            pool.Init(original, count);
            pool.Root.SetParent(gameObject.transform, false);
            _poolDict[original.name] = pool;
        }
    }
}
