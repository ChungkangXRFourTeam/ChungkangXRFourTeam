using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class PlayerCalculation 
{
    public static InteractionController GetClickedActorOrNull(Vector2 centerPos, float grabDistance, int mask)
    {
        var colArr = Physics2D.OverlapCircleAll(centerPos, grabDistance, mask);

        Plane p = new Plane(-Vector3.forward, Vector3.zero);
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (!p.Raycast(ray, out var enter)) return null;

        Vector2 mousePoint = ray.origin + ray.direction * enter;
        var actor = GetCloserActorOrNull(colArr, mousePoint);

        return actor;
    }
    
    public static InteractionController GetCloserActorOrNull(Collider2D[] arr, Vector2 point)
    {
        if (arr == null) return null;
        if (arr.Length == 0) return null;

        Collider2D minCol = null;
        float dis = Mathf.Infinity;

        for (int i = 0; i < arr.Length; i++)
        {
            float tempDis = (point - (Vector2)arr[i].transform.position).sqrMagnitude;
            if (dis >= tempDis)
            {
                dis = tempDis;
                minCol = arr[i];
            }
        }

        return minCol.GetComponentInChildren<InteractionController>();
    }
    
    public static Vector2 GetSwingDirection(Camera  camera, Vector2 actorPosition)
    {
        var ray = camera.ScreenPointToRay(Input.mousePosition);
        Plane p = new Plane(-Vector3.forward, Vector3.forward * -1f);

        if (!p.Raycast(ray, out var enter))
            throw new System.Exception();

        var dir = Vector3.Normalize((Vector2)(ray.origin + ray.direction * enter) - actorPosition);
        return dir;
    }

    public static Vector3[] GetReflectionPoints(Vector2 start, Vector2 dir)
    {
        Vector2 currentPos = start;
        Vector2 currentDir = dir;

        List<Vector3> points = new List<Vector3>(10);
        points.Add(currentPos);
        int maxIter = 30;
        int currentIter = 0;
        while (maxIter > currentIter)
        {
            currentIter++;
            var hit = Physics2D.BoxCast(currentPos, Vector2.one * 1.3f, 0f, currentDir, Mathf.Infinity,
                ~LayerMask.GetMask("Player", "Enemy", "Confiner", "Ignore Raycast", "EnemyBody"));
            if (!hit) break;

            var com = hit.collider.GetComponent<KnockbackObject>();
            if (!com)
            {
                points.Add(hit.point);
                break;
            }

            currentDir = com.ReflecDirection;
            currentPos = hit.centroid + com.ReflecDirection * 0.01f;
            points.Add(hit.point + com.ReflecDirection * 0.01f);
        }

        return points.ToArray();
    }
}
