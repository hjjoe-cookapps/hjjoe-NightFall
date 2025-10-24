using UnityEngine;

public static class Bezier
{
    public static Vector3 GetBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        t = Mathf.Clamp01(t); // t를 0과 1 사이로 제한
        float u = 1 - t;
        float uu = u * u;
        float uuu = uu * u;
        float tt = t * t;
        float ttt = tt * t;

        // B(t) = (1-t)³P₀ + 3(1-t)²tP₁ + 3(1-t)t²P₂ + t³P₃
        Vector3 p = uuu * p0;          // (1-t)³P₀
        p += 3 * uu * t * p1;        // 3(1-t)²tP₁
        p += 3 * u * tt * p2;        // 3(1-t)t²P₂
        p += ttt * p3;               // t³P₃

        return p;
    }
}

