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
        return uuu * p0 +
               3f * uu * t * p1 +
               3f * u * tt * p2 +
               ttt * p3;
    }

    // 3차 베지어 곡선 계산 (2D 버전)
    public static Vector2 GetBezierPoint2D(float t, Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3)
    {
        t = Mathf.Clamp01(t);
        float u = 1 - t;
        float uu = u * u;
        float uuu = uu * u;
        float tt = t * t;
        float ttt = tt * t;


        // B(t) = (1-t)³P₀ + 3(1-t)²tP₁ + 3(1-t)t²P₂ + t³P₃
        return uuu * p0 +
               3f * uu * t * p1 +
               3f * u * tt * p2 +
               ttt * p3;
    }

}

