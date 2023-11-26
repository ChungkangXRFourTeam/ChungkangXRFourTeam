using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class DiaryUIView : MonoBehaviour
{
    [SerializeField] private TMP_Text _text;

    private void Awake()
    {
        SetText("");
    }

    public void SetText(string text)
    {
        _text.text = text;
    }
}
