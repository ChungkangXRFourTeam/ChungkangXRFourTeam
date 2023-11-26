using UnityEngine;

public static class BazierMath
{
    /// <summary>
    /// 4개의 정점으로 러프된 베지에 값을 가져온다
    /// </summary>
    /// <param name="p1">정점1</param>
    /// <param name="p2">정점2</param>
    /// <param name="p3">정점3</param>
    /// <param name="p4">정점4</param>
    /// <param name="value">러프값(0f ~ 1f)</param>
    /// <returns>러프된 베지에 값</returns>
    public static Vector3 Lerp(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, float value)
    {
        Vector3 a = Vector2.Lerp(p1, p2, value);
        Vector3 b = Vector2.Lerp(p2, p3, value);
        Vector3 c = Vector2.Lerp(p3, p4, value);

        Vector3 d = Vector2.Lerp(a, b, value);
        Vector3 e = Vector2.Lerp(b, c, value);

        Vector3 f = Vector2.Lerp(d, e, value);
        return f;
    }
}