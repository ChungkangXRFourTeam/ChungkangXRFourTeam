using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XRProject.Boss;

public class Boss : MonoBehaviour
{
    private Track _track;

    private void Awake()
    {
        _track = BossPatternFactory.CompletionTrack();
    }

    private void Update()
    {
        _track.EValuate();
    }
}
