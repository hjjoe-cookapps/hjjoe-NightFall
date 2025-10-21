using System.Collections.Generic;
using UnityEngine;

public static class CoroutineManager
{
    class FloatComparer : IEqualityComparer<float>
    {
        bool IEqualityComparer<float>.Equals(float x, float y)
        {
            return x == y;
        }

        public int GetHashCode(float obj)
        {
            return obj.GetHashCode();
        }
    }

    public static readonly WaitForEndOfFrame WaitForEndOfFrame = new WaitForEndOfFrame();
    public static readonly WaitForFixedUpdate WaitForFixedUpdate = new WaitForFixedUpdate();

    private static readonly Dictionary<float, WaitForSeconds> waitForSecondsCoroutines =
        new Dictionary<float, WaitForSeconds>(new FloatComparer());

    public static WaitForSeconds WaitForSeconds(float seconds)
    {
        if (!waitForSecondsCoroutines.TryGetValue(seconds, out WaitForSeconds target))
        {
            waitForSecondsCoroutines.Add(seconds, target = new WaitForSeconds(seconds));
        }

        return target;
    }
}
