using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace XRProject.Utils.Editors
{
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
            var transform = _visualizer.transform;
            _visualizer.CalculatePoints(
                transform.TransformPoint(h1),
                transform.TransformPoint(h2)
            );
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

            var handles = new DirtyContainer<Vector3>[]
            {
                new(serializedObject.FindProperty("_handle1").vector3Value),
                new(serializedObject.FindProperty("_handle2").vector3Value),
            };

            
            // handle은 local 좌표계이기 때문에 PositionHandle에 넘겨줄 때 마다 world 좌표계로 변환시켜준다.
            handles[0].Value = Handles.PositionHandle(RotatePoint(handles[0].Value), rotation);
            handles[1].Value = Handles.PositionHandle(RotatePoint(handles[1].Value), rotation);

            // 핸들 값이 이전과 다르게 변경됐다면 visualizing 하기위해 정점들을 계산해준다.
            // 더티 플래그 패턴 사용함
            if (handles[0].IsDirty || handles[1].IsDirty)
            {
                _visualizer.CalculatePoints(handles[0], handles[1]);

                handles[0].SetDirtyOff();
                handles[1].SetDirtyOff();
            }

            // world 좌표계로 바꾼 것을 다시 local 좌표계로 변경해준다.
            handles[0].Value = InverseRotatePoint(handles[0].Value);
            handles[1].Value = InverseRotatePoint(handles[1].Value);

            
            serializedObject.FindProperty("_handle1").vector3Value = handles[0].Value;
            serializedObject.FindProperty("_handle2").vector3Value = handles[1].Value;
            serializedObject.ApplyModifiedProperties();
        }
    }
}