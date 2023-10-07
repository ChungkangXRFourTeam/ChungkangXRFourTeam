using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class TraceVisualizer : MonoBehaviour
{
    [SerializeField] private bool isDirectionUp = true;
    [SerializeField] private Color _meshColor;
    [SerializeField] private LayerMask _layerMask;
    [SerializeField] private float _rayLength;
    [SerializeField] private int _iteration;
    [SerializeField] [HideInInspector] private List<Vector2> _hittedPointsInEditor;
    [SerializeField]  private Vector3 _savedPosition;
    [SerializeField]  private Vector3 _handle1;
    [SerializeField]  private Vector3 _handle2;

    public float RayLength => _rayLength;

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

        var hit = Physics2D.Raycast(point, dir, _rayLength,_layerMask);

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
            var vrb = Vector2.Lerp(h1, h2, (float)(i+1) / (float)_iteration);
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
        
        var mesh = GenerateMesh();
        Gizmos.color = _meshColor;
        Gizmos.DrawMesh(mesh);
    }
}