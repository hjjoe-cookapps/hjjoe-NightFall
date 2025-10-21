using System.Collections;
using System;
using UnityEngine;
public static class IEnumerators
{
    public static IEnumerator WaitForDurationAndAction(float duration, Action action)
    {
        yield return CoroutineManager.WaitForSeconds(duration);
        action?.Invoke();
    }
}
