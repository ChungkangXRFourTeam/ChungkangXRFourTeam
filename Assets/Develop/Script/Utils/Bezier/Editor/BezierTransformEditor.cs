using UnityEngine;
using UnityEditor;
using System.Collections;
using System;

[CustomEditor(typeof(BezierTransform))]
public class BezierTransformEditor : Editor
{
    [SerializeField]private BezierTransform _bezierTransform;
    [SerializeField]private Transform _cacheTransform;

    [SerializeField]private Texture2D _favicon;

    [SerializeField]private SerializedProperty value, p1, p2, p3, p4, playOnAwake, autoLookAt, duration, animationCurve, simulated;

    [SerializeField]private Vector3 _localRotation;
    [SerializeField]private Vector3 _localScal;

    [SerializeField] private DateTime _cachingTime = DateTime.MaxValue;
    [SerializeField] private float _playTime = 0f;

    private void OnEnable()
    {
        _favicon = EditorGUIUtility.FindTexture("d_AvatarPivot");

        _bezierTransform = (BezierTransform)target;
        _cacheTransform = _bezierTransform.CacheTransform;

        playOnAwake = serializedObject.FindProperty("playOnAwake");
        autoLookAt = serializedObject.FindProperty("autoLookAt");
        duration = serializedObject.FindProperty("duration");
        simulated = serializedObject.FindProperty("simulated");

        if (_bezierTransform .animationCurve == null || _bezierTransform.animationCurve.length == 0)
        {
            _bezierTransform.animationCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
        }
        animationCurve = serializedObject.FindProperty("animationCurve");
        value = serializedObject.FindProperty("value");
        p1 = serializedObject.FindProperty("p1");
        p2 = serializedObject.FindProperty("p2");
        p3 = serializedObject.FindProperty("p3");
        p4 = serializedObject.FindProperty("p4");
        _localRotation = _cacheTransform.eulerAngles;
        _localScal = _cacheTransform.localScale;

        // 기존 Transform을 숨긴다.
        //_cacheTransform.hideFlags = HideFlags.HideInInspector;

        // 컴포넌트를 최상단으로 올린다.
        UnityEditorInternal.ComponentUtility.MoveComponentUp(_bezierTransform);

        Undo.undoRedoPerformed += UndoRedoPerformed;
        EditorApplication.update += Update;
    }

    private void OnDisable()
    {
        Undo.undoRedoPerformed -= UndoRedoPerformed;
        EditorApplication.update -= Update;

        if (!Application.isPlaying)
        {
            if (_bezierTransform != null)
                _bezierTransform.enabled = true;
            value.floatValue = 0f;
        }
        if (_cacheTransform != null)
        {
            if (autoLookAt.boolValue)
            {
                _cacheTransform.eulerAngles = Vector3.zero;
            }
            _cacheTransform.hideFlags = HideFlags.None;
        }
        if (_bezierTransform != null)
        {
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
            serializedObject.Update();
            _bezierTransform.FixedUpdate();
        }
    }

    private void Update()
    {
        if (!Application.isPlaying && _bezierTransform.gameObject.activeInHierarchy)
        {
            var now = DateTime.Now;

            if (_playTime > duration.floatValue)
            {
                // Reset Timer
                _playTime = 0f;
            }

            if (_cachingTime == DateTime.MaxValue)
            {
                _cachingTime = now;
            }

            var diff = now - _cachingTime;
            _playTime += (float)diff.TotalSeconds;
            var t = _playTime / duration.floatValue;
            value.floatValue = animationCurve.animationCurveValue.Evaluate(t);

            if (_cacheTransform != null)
            {
                _localRotation = _cacheTransform.eulerAngles;
                _localScal = _cacheTransform.localScale;
            }
            if (_bezierTransform != null)
            {
                serializedObject.ApplyModifiedPropertiesWithoutUndo();
                serializedObject.Update();
                _bezierTransform.FixedUpdate();
            }

            _cachingTime = now;
        }
    }

    private void UndoRedoPerformed()
    {
        _localRotation = _cacheTransform.eulerAngles;
        _localScal = _cacheTransform.localScale;
        _localRotation = EditorGUILayout.Vector3Field("Rotation", _localRotation);
        _localScal = EditorGUILayout.Vector3Field("Scale", _localScal);
    }

    bool _isfoldout = true;
    public override void OnInspectorGUI()
    {
        if (GUI.changed)
        {
            Undo.RecordObjects(new UnityEngine.Object[3] { this, _bezierTransform, _cacheTransform }, "Changed BezierTransformEditor");
        }

        //base.OnInspectorGUI();
        GUIStyle box = new GUIStyle("Box");
        GUIStyle style = new GUIStyle("Label");
        //style.normal.textColor = Color.white;
        style.fontStyle = FontStyle.Bold;

        EditorGUIUtility.SetIconForObject(_bezierTransform, _favicon);

        // Transform
        EditorGUILayout.LabelField("Transform", style);
        EditorGUILayout.BeginVertical(box);
        //EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.Vector3Field("Position", _cacheTransform.localPosition);
        //EditorGUI.EndDisabledGroup();

        if (autoLookAt.boolValue)
        {
            EditorGUI.BeginDisabledGroup(true);
            _localRotation = EditorGUILayout.Vector3Field("Rotation (Auto LookAt)", _localRotation);
            EditorGUI.EndDisabledGroup();
        }
        else
        {
            _localRotation = EditorGUILayout.Vector3Field("Rotation", _localRotation);
        }
        
        _localScal = EditorGUILayout.Vector3Field("Scale", _localScal);
        EditorGUILayout.EndVertical();

        // Point
        EditorGUILayout.Space(10f);
        EditorGUILayout.LabelField("Point", style);
        EditorGUILayout.BeginVertical(box);
        EditorGUILayout.PropertyField(value);
        EditorGUILayout.PropertyField(p1);
        EditorGUILayout.PropertyField(p2);
        EditorGUILayout.PropertyField(p3);
        EditorGUILayout.PropertyField(p4);
        EditorGUILayout.EndVertical();

        // Animation
        EditorGUILayout.Space(10f);
        _isfoldout = EditorGUILayout.BeginFoldoutHeaderGroup(_isfoldout, "Animation");
        if (_isfoldout)
        {
            EditorGUILayout.BeginVertical(box);
            EditorGUILayout.PropertyField(simulated);
            EditorGUILayout.PropertyField(playOnAwake);
            EditorGUILayout.PropertyField(autoLookAt);
            EditorGUILayout.PropertyField(duration);
            EditorGUILayout.PropertyField(animationCurve);
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        if (GUI.changed)
        {
            _cacheTransform.eulerAngles = _localRotation;
            _cacheTransform.localScale = _localScal;
            Undo.RecordObjects(new UnityEngine.Object[3] { this, _bezierTransform, _cacheTransform }, "Changed BezierTransformEditor");
        }
        serializedObject.ApplyModifiedPropertiesWithoutUndo();
        serializedObject.Update();
    }

    private void OnSceneGUI()
    {
        Vector3 parentPos = Vector3.zero;
        if (_cacheTransform.parent != null)
        {
            parentPos = _cacheTransform.parent.position;
        }

        if (simulated.boolValue)
        {
            Vector3 p1 = this.p1.vector3Value + parentPos;
            Vector3 p2 = this.p2.vector3Value + parentPos;
            Vector3 p3 = this.p3.vector3Value + parentPos;
            Vector3 p4 = this.p4.vector3Value + parentPos;

            this.p1.vector3Value = Handles.PositionHandle(p1, Quaternion.identity) - parentPos;
            this.p2.vector3Value = Handles.PositionHandle(p2, Quaternion.identity) - parentPos;
            this.p3.vector3Value = Handles.PositionHandle(p3, Quaternion.identity) - parentPos;
            this.p4.vector3Value = Handles.PositionHandle(p4, Quaternion.identity) - parentPos;

            Handles.Label(p1, "P1");
            Handles.Label(p2, "P2");
            Handles.Label(p3, "P3");
            Handles.Label(p4, "P4");

            Handles.DrawLine(p1, p2);
            Handles.DrawLine(p2, p3);
            Handles.DrawLine(p3, p4);

            Handles.color = Color.green;
            const int DRAW_COUNT = 25;
            for (float i = 0; i < DRAW_COUNT; i++)
            {
                float value_Before = i / DRAW_COUNT;
                Vector3 before = BazierMath.Lerp(p1, p2, p3, p4, value_Before);

                float value_After = (i + 1) / DRAW_COUNT;
                Vector3 after = BazierMath.Lerp(p1, p2, p3, p4, value_After);

                Handles.DrawLine(before, after);
            }
            Handles.color = Color.white;

            if (GUI.changed)
            {
                Undo.RecordObjects(new UnityEngine.Object[3] { this, _bezierTransform, _cacheTransform }, "Changed BezierTransformEditor");
            }
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
            serializedObject.Update();
        }
    }
}