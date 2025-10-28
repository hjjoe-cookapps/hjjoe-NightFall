using UnityEngine;

public class SingletonBehaviour<T> : MonoBehaviour where T : Component
{
    protected static T _instance = null;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindAnyObjectByType<T>();

                if (_instance == null)
                {
                    GameObject obj = new GameObject { name = string.Format(typeof(T).Name) };
                    _instance = obj.AddComponent<T>();
                }
            }

            return _instance;
        }
    }

    protected virtual void Awake()
    {
        if (!Application.isPlaying)
        {
            return;
        }

        if (_instance == null)
        {
            _instance = this as T;
        }
        else
        {
            if (this != _instance)
            {
                Debug.LogError("[X] 싱글톤(" + transform.name + ") : " + typeof(T).Name + "의 중복 생성을 시도하고 있습니다.");

                Destroy(this.gameObject);
            }
        }
    }
}
