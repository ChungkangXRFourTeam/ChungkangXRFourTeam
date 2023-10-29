using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TEST_FRAME_STATE : MonoBehaviour
{
    [SerializeField] private TMP_Text _text;

    private void Update()
    {
        _text.text = $"frame: {1f / Time.deltaTime}";
    }
}
