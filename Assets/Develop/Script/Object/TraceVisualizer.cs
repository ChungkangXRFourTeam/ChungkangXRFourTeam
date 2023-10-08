using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TraceVisualizer : MonoBehaviour
{
    [SerializeField] private bool isDirectionUp = true;
    [SerializeField] private bool _drawMesh = true;
    [SerializeField] private bool _drawWireMesh = true;
    [SerializeField] private bool _drawOutline = true;
    [SerializeField] private Color _meshColor;
    [SerializeField] private Color _wireMesholor;
    [SerializeField] private Color _outlineColor;
    [SerializeField] private LayerMask _layerMask;
    [SerializeField] private float _rayLength;
    [SerializeField] private int _iteration;
    [SerializeField] private Vector3 _handle1;
    [SerializeField] private Vector3 _handle2;
    
    [SerializeField] [HideInInspector] private List<Vector2> _hittedPointsInEditor;
    [SerializeField] [HideInInspector] private bool _isDirty;
    

    private Mesh _debugRenderMesh;

    public void CalculatePoints(Vector3 point1, Vector3 point2)
    {
        if (_hittedPointsInEditor == null) return;
        _hittedPointsInEditor.Clear();

        for (int i = 0; i <= _iteration; i++)
        {
            Vector2 point = Vector2.Lerp(point1, point2, (float)i / (float)_iteration);
            _hittedPointsInEditor.Add(Raycast(point));
        }
    }

    private Vector3 Raycast(Vector3 point)
    {
        var dir = isDirectionUp ? transform.up : transform.right;

        var hit = Physics2D.Raycast(point, dir, _rayLength, _layerMask);

        if (!hit) return point + dir * _rayLength;

        return hit.point;
    }

    private Mesh GenerateMesh()
    {
        Mesh mesh = new Mesh();
        mesh.name = "debug mesh";

        List<Vector3> v = new List<Vector3>();
        List<int> indices = new List<int>();

        var h1 = transform.TransformPoint(_handle1);
        var h2 = transform.TransformPoint(_handle2);

        for (int i = 0; i < _hittedPointsInEditor.Count - 1; i++)
        {
            var vlb = Vector2.Lerp(h1, h2, (float)i / (float)_iteration);
            var vrb = Vector2.Lerp(h1, h2, (float)(i + 1) / (float)_iteration);
            var vlt = _hittedPointsInEditor[i];
            var vrt = _hittedPointsInEditor[i + 1];

            v.Add(vlb);
            v.Add(vlt);
            v.Add(vrt);

            v.Add(vlb);
            v.Add(vrt);
            v.Add(vrb);
        }

        for (int i = 0; i < _hittedPointsInEditor.Count - 1; i++)
        {
            int index = i * 6;

            indices.Add(index);
            indices.Add(index + 1);
            indices.Add(index + 2);

            indices.Add(index + 3);
            indices.Add(index + 4);
            indices.Add(index + 5);
        }

        mesh.vertices = v.ToArray();
        mesh.triangles = indices.ToArray();
        mesh.RecalculateNormals();
        return mesh;
    }

    private void OnDrawGizmos()
    {
        if (_hittedPointsInEditor == null || _hittedPointsInEditor.Count <= 2) return;

        if(_isDirty || _debugRenderMesh == null)
        {
            _debugRenderMesh = GenerateMesh();
            _isDirty = false;
        }

        if (_drawMesh)
        {
            Gizmos.color = _meshColor;
            Gizmos.DrawMesh(_debugRenderMesh);
        }
        if(_drawWireMesh)
        {
            Gizmos.color = _wireMesholor;
            Gizmos.DrawWireMesh(_debugRenderMesh);
        }

        // WireMesh가 그려지면 outline은 그려지는게 의미가 없다. 완전히 중복된다.
        if (_drawOutline && !_drawWireMesh)
        {
            var h1 = transform.TransformPoint(_handle1);
            var h2 = transform.TransformPoint(_handle2);
            var begin = Vector2.Lerp(h1, h2, 0f);
            var end = Vector2.Lerp(h1, h2, 1f);

            Gizmos.color = _outlineColor;
            Gizmos.DrawLine(begin, end);
            Gizmos.DrawLine(_hittedPointsInEditor[0], begin);
            Gizmos.DrawLine(_hittedPointsInEditor[^1], end);
            
            for (int i = 0; i < _hittedPointsInEditor.Count - 1; i++)
            {
                var p1 = _hittedPointsInEditor[i];
                var p2 = _hittedPointsInEditor[i + 1];
                Gizmos.DrawLine(p1, p2);
                
            }

        }
    }
}