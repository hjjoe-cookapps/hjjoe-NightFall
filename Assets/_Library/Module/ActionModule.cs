using System;
using UnityEngine;

public class ActionModule : MonoBehaviour
{
    //public event Action OnAwakeEvent;
    //public event Action OnEnableEvent;
    public event Action OnStartEvent;
    //public event Action OnUpdateEvent;
    public event Action OnDisableEvent;
    public event Action OnDestroyEvent;

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
        OnStartEvent?.Invoke();
    }
    //private void Update()
    //{
    //    OnUpdateEvent?.Invoke();
    //}
    private void OnDisable()
    {
        OnDisableEvent?.Invoke();
    }
    private void OnDestroy()
    {
        OnDestroyEvent?.Invoke();
    }

}
