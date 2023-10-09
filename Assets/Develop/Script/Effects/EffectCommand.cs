using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public struct EffectCommand
{
    public string EffectKey { get; set; }
    public Vector3? Position { get; set; }
    public Quaternion? Rotation { get; set; }
    public Vector3? Scale { get; set; }
    public float FlipRotation { get; set; }
}
