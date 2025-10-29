using UnityEngine;

public static class ComponentExtensions
{
    public static bool IsDead<T>(this T component) where T : Component
    {
        return component == null || (component != null && component.gameObject.activeSelf == false);
    }
    public static T GetOrAddComponent<T>(this GameObject obj) where T : Component
    {
        T component = obj.GetComponent<T>();
        if (component == null)
            component = obj.AddComponent<T>();

        return component;
    }

    public static GameObject GetRoot(this GameObject obj)
    {
        return obj.transform.root.gameObject;
    }

    public static GameObject GetParent(this GameObject obj)
    {
        return obj.transform.parent.gameObject != null ?  obj.transform.parent.gameObject : obj;
    }
}


