using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public struct DirtyContainer<T> where T : IEquatable<T>
{
    private T _value;
    private T _prevValue;

    public DirtyContainer(T v)
    {
        _value = _prevValue = v;
        IsDirty = false;
    }

    public bool IsDirty { get; private set; }
    public void SetDirtyOff() => IsDirty = false;

    public T PrevValue => _prevValue;

    public T Value
    {
        get => _value;
        set
        {
            if (!value.Equals(_value))
            {
                _prevValue = _value;
                _value = value;
                IsDirty = true;
            }
        }
    }

    public static implicit operator T(DirtyContainer<T> v)
    {
        return v.Value;
    }
}

public interface ISerializedPropertyWrapper
{
    public SerializedProperty SerializeProperty { get; }
}

public struct Vector3SPW : ISerializedPropertyWrapper
{
    public Vector3 Value
    {
        get => SerializeProperty.vector3Value;
        set => SerializeProperty.vector3Value = value;
    }

    public SerializedProperty SerializeProperty { get; private set; }

    public Vector3SPW(SerializedObject obj, string field)
    {
        this.SerializeProperty = obj.FindProperty(field);
    }

    public static implicit operator Vector3(Vector3SPW v)
    {
        return v.Value;
    }
}


[CustomEditor(typeof(TraceVisualizer))]
public class TraceVisualizerEditor : Editor
{
    private TraceVisualizer _visualizer = null;

    void OnEnable()
    {
        _visualizer = (TraceVisualizer)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var h1 = serializedObject.FindProperty("_handle1").vector3Value;
        var h2 = serializedObject.FindProperty("_handle2").vector3Value;
        _visualizer.CalculatePoints(_visualizer.transform.TransformPoint( h1), _visualizer.transform.TransformPoint(h2));
    }

    private Vector3 GetNextHandlePosition(Vector3 handle, Vector3 currentOrigin, Vector3 prevOrigin)
    {
        var dir = handle - prevOrigin;
        return currentOrigin + dir;
    }

    private Vector3 RotatePoint(Vector3 point)
    {
        var transform = _visualizer.transform;

        return transform.TransformPoint(point);
    }
    private Vector3 InverseRotatePoint(Vector3 point)
    {
        var transform = _visualizer.transform;
        return transform.InverseTransformPoint(point);
    }

    private void OnSceneGUI()
    {
        var transform = _visualizer.transform;
        var rotation = transform.rotation;
        var position = transform.position;

        var handles = new DirtyContainer<Vector3>[2]
        {
            new(serializedObject.FindProperty("_handle1").vector3Value),
            new(serializedObject.FindProperty("_handle2").vector3Value),
        };

        handles[0].Value = Handles.PositionHandle(RotatePoint(handles[0].Value), rotation);
        handles[1].Value = Handles.PositionHandle(RotatePoint(handles[1].Value), rotation);
        

        if (handles[0].IsDirty || handles[1].IsDirty)
        {
            _visualizer.CalculatePoints(handles[0], handles[1]);

            handles[0].SetDirtyOff();
            handles[1].SetDirtyOff();
        }

        handles[0].Value = InverseRotatePoint(handles[0].Value);
        handles[1].Value = InverseRotatePoint(handles[1].Value);
        
        serializedObject.FindProperty("_handle1").vector3Value = handles[0].Value;
        serializedObject.FindProperty("_handle2").vector3Value = handles[1].Value;
        serializedObject.ApplyModifiedProperties();
    }

}