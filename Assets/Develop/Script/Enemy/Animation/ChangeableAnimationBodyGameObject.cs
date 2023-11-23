using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ChangeableAnimationBodyGameObject : ChangeableAnimationBody
{
    public override bool IsEnabled => gameObject.activeSelf;
    public override void SetEnable(bool isEnabled)
    {
        gameObject.SetActive(isEnabled);
    }
}