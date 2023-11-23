using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HedgehogEffect : MonoBehaviour
{
    [SerializeField] private ParticleSystem _one;
    [SerializeField] private ParticleSystem _two;
    [SerializeField] private ParticleSystem _trail;

    public void SetDuration(float flame, float trail)
    {
        var main1 = _one.main;
        main1.duration = flame;
        var main2 = _two.main;
        main2.duration = flame;

        var trailMain = _trail.main;
        trailMain.duration = trail;
    }

    public void Play(Vector2 pos, Quaternion rotation)
    {
        transform.position = pos;
        transform.rotation = rotation;
        
        gameObject.SetActive(true);
        _one.Play();
        _two.Play();
        _trail.Play();
    }
    
    
}
