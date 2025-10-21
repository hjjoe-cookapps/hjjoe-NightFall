using System.Collections.Generic;
using UnityEngine;

public class PoolManager : SingletonBehaviour<PoolManager>
{
    private readonly Dictionary<string, Pool> poolDict = new Dictionary<string, Pool>();

    private Transform rootTransform;

    public void Enqueue(PoolMonoBehaviour obj)
    {
        if (obj != null && !poolDict.ContainsKey(obj.name))
        {
            Object.Destroy(obj.gameObject);
            return;
        }

        poolDict[obj.name].Enqueue(obj);
    }

    public PoolMonoBehaviour Dequeue(GameObject original, Transform transform = null)
    {
        if (original == null)
        {
            return null;
        }

        if (!poolDict.ContainsKey(original.name))
        {
            CreatePool(original);
        }

        return poolDict[original.name].Dequeue(transform);
    }

    public PoolMonoBehaviour Dequeue(GameObject original, Vector3 position)
    {
        if (original == null)
        {
            return null;
        }

        if (!poolDict.ContainsKey(original.name))
        {
            CreatePool(original);
        }

        return poolDict[original.name].Dequeue(position);
    }

    public GameObject GetOriginal(string name)
    {
        if (!poolDict.ContainsKey(name))
        {
            return null;
        }

        return poolDict[name].Original;
    }

    public void Clear()
    {
        foreach (var pool in poolDict.Values)
        {
            pool.Clear();
        }
        poolDict.Clear();
    }

    private void CreatePool(GameObject original)
    {
        if (original != null && !poolDict.ContainsKey(original.name))
        {
            int count = original.GetOrAddComponent<PoolMonoBehaviour>().PoolCreateCount;

            Pool pool = new Pool();
            pool.Init(original, count);
            pool.Root.parent = rootTransform;
            poolDict[original.name] = pool;
        }
    }
}
