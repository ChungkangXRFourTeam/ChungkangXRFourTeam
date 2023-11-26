using System;
using UnityEngine;
using System.Collections;

[AddComponentMenu("Bezier Transform")]
public class BezierTransform : MonoBehaviour
{
    [SerializeField] private Transform _cacheTransform = null;
    public Transform CacheTransform
    {
        get
        {
            if (_cacheTransform == null)
                _cacheTransform = this.transform;
            return _cacheTransform;
        }
    }

    [Range(0f, 1f)]
    [SerializeField] private float value = 0f;
    public Vector2 p1;
    public Vector2 p2;
    public Vector2 p3;
    public Vector2 p4;
    public bool playOnAwake = true;
    public bool autoLookAt = false;
    public bool simulated = false;
    public bool isAnimStarted;
    public float duration = 2.5f;
    public AnimationCurve animationCurve;

    private float _playTime = 0f;

    private void Awake()
    {
        if (animationCurve == null)
        {
            animationCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
        }
    }

    private void OnEnable()
    {
        value = 0f;
        animationCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
        if (playOnAwake)
        {
            StopCoroutine("PlayAnimation");
            StartCoroutine("PlayAnimation");
        }
    }

    private void OnDisable()
    {
        StopCoroutine("PlayAnimation");
    }

    public void startAnimation()
    {
        StopCoroutine("PlayAnimation");
        StartCoroutine("PlayAnimation");
    }
    private IEnumerator PlayAnimation()
    {
        isAnimStarted = true;
        _playTime = 0f;
        while (_playTime < duration)
        {
            _playTime += Time.deltaTime;
            if (_playTime >= duration)
                _playTime = duration;

            var t = _playTime / duration;
            this.value = animationCurve.Evaluate(t);
            yield return null;
        }

        isAnimStarted = false;
        this.enabled = false;
    }

    public void FixedUpdate()
    {
        if (isAnimStarted)
        {
            var currentPosition = BazierMath.Lerp(p1, p2, p3, p4, value);
            this.CacheTransform.localPosition = currentPosition;
        }
#if UNITY_EDITOR
        if (simulated)
        {
            var currentPosition_Editor = BazierMath.Lerp(p1, p2, p3, p4, value);
            this.CacheTransform.localPosition = currentPosition_Editor;
        }
#endif
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Vector3 p1 = this.p1;
        Vector3 p2 = this.p2;
        Vector3 p3 = this.p3;
        Vector3 p4 = this.p4;

        if (CacheTransform.parent != null)
        {
            p1 += _cacheTransform.parent.position;
            p2 += _cacheTransform.parent.position;
            p3 += _cacheTransform.parent.position;
            p4 += _cacheTransform.parent.position;
        }

        GUI.color = Color.green;
        const int DRAW_COUNT = 25;
        for (float i = 0; i < DRAW_COUNT; i++)
        {
            float value_Before = i / DRAW_COUNT;
            Vector3 before = BazierMath.Lerp(p1, p2, p3, p4, value_Before);

            float value_After = (i + 1) / DRAW_COUNT;
            Vector3 after = BazierMath.Lerp(p1, p2, p3, p4, value_After);

            Gizmos.DrawLine(before, after);
        }
        GUI.color = Color.white;
    }
#endif
}
