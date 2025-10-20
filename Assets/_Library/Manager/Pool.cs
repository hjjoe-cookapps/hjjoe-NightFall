using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

    public class Pool
    {
        public GameObject Original { get; private set; } = null;
        public Transform Root = null;
        private readonly Queue<PoolMonoBehaviour> poolQueue = new Queue<PoolMonoBehaviour>();

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


            if(!poolQueue.Contains(PoolMonoBehaviour))
            {
                poolQueue.Enqueue(PoolMonoBehaviour);
            }
        }

        public PoolMonoBehaviour Dequeue(Transform transform = null)
        {
            PoolMonoBehaviour PoolMonoBehaviour;
            if (poolQueue.Count > 0)
            {
                PoolMonoBehaviour = poolQueue.Dequeue();
            }
            else
            {
                PoolMonoBehaviour = Create();
            }
            
            PoolMonoBehaviour.transform.parent = transform;
            if (transform == null)
            {
                SceneManager.MoveGameObjectToScene(PoolMonoBehaviour.gameObject, SceneManager.GetActiveScene());
            }
            
            PoolMonoBehaviour.gameObject.SetActive(true);

            return PoolMonoBehaviour;
        }

        public PoolMonoBehaviour Dequeue(Vector3 position)
        {
            PoolMonoBehaviour PoolMonoBehaviour;
            if (poolQueue.Count > 0)
            {
                PoolMonoBehaviour = poolQueue.Dequeue();
                PoolMonoBehaviour.transform.parent = null;
                SceneManager.MoveGameObjectToScene(PoolMonoBehaviour.gameObject, SceneManager.GetActiveScene());
                PoolMonoBehaviour.transform.position = position;
            }
            else
            {
                PoolMonoBehaviour = Create(position);
            }

            PoolMonoBehaviour.gameObject.SetActive(true);

            return PoolMonoBehaviour;
        }

        public void Clear()
        {
            foreach(var obj in poolQueue)
            {
                Object.Destroy(obj.gameObject);
            }
            poolQueue.Clear();

            Object.Destroy(Root.gameObject);
            Original = null;
        }
    }
