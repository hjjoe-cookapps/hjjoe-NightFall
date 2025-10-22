using System.Collections;
using System;

public static class IEnumeratorExtensions
{
    public static IEnumerator WaitForDurationAndAction(float duration, Action action)
    {
        yield return CoroutineManager.WaitForSeconds(duration);
        action?.Invoke();
    }
}
