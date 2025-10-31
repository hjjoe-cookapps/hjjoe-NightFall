using System;
using Unity.VisualScripting;

public abstract class Singleton<T> : IDisposable where T : new()
{
    #region Fields

    private static readonly object Lock = new();

    protected static T _instance;

    #endregion

    #region Properties

    public static T Instance
    {
        get
        {
            if (_instance != null)
            {
                return _instance;
            }

            lock (Lock)
            {
                _instance ??= new T();
                (_instance as Singleton<T>)?.Init();
            }

            return _instance;
        }
    }

    #endregion

    //-------------- internal, public Methods --------------//
    public static bool IsCreated()
    {
        return _instance != null;
    }

    public virtual void Dispose()
    {
        ReleaseUnmanagedResources();
        GC.SuppressFinalize(this);
    }

    //-------------- protected, private Methods --------------//
    static Singleton()
    {
    }

    protected Singleton()
    {
    }

    ~Singleton()
    {
        ReleaseUnmanagedResources();
    }

    protected virtual void Init()
    {

    }

    protected virtual void ReleaseUnmanagedResources()
    {
    }

    #region UNITY_EDITOR

#if UNITY_EDITOR
    // TODO : 싱글톤 구현한 클래스에 이 부분 복사 후 주석 해제하기.
    // [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    // private static void ReloadDomain()
    // {
    //     _inst = null;
    // }
#endif

    #endregion
}
