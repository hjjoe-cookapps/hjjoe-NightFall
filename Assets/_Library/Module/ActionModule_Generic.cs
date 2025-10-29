using System;
using UnityEngine;

public class ActionModule_Generic<T> : MonoBehaviour
{
    private T _generic;
    public T Generic
    {
        get
        {
            if (_generic == null)
            {
                _generic = gameObject.GetComponent<T>();
                if (_generic == null)
                {
                    Debug.Log("ActionModule_Generic<T>: No component of type " + typeof(T).Name);
                    enabled = false;
                }
            }

            return _generic;
        }
    }

    //public event Action OnAwakeEvent;
    //public event Action OnEnableEvent;
    public event Action<T> OnStartEvent;
    //public event Action<T> OnUpdateEvent;
    public event Action<T> OnDisableEvent;
    public event Action<T> OnDestroyEvent;

    //private void Awake()
    //{
    //    OnAwakeEvent?.Invoke();
    //}

    //private void OnEnable()
    //{
    //    OnEnableEvent?.Invoke();
    //}
    private void Start()
    {
        OnStartEvent?.Invoke(Generic);
    }
    //private void Update()
    //{
    //    OnUpdateEvent?.Invoke();
    //}
    private void OnDisable()
    {
        OnDisableEvent?.Invoke(Generic);
    }
    private void OnDestroy()
    {
        OnDestroyEvent?.Invoke(Generic);
    }

}
