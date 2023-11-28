using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.InputSystem;

public class TypingSystem : MonoBehaviour
{
    public static TypingSystem _instance;
    
    [SerializeField]
    private float typingTimer = 0.05f;
    [SerializeField]
    private float typingTimer_fast = 0.02f;
    [SerializeField]
    private float typingTime;
    private string[] texts;
    private TextMeshProUGUI tmpSave;

    public static bool isDialogEnd;
    public bool isTypingEnd;
    private int dialogNumber = 0;

    private float timer;

    void Awake()
    {
        timer = typingTimer;
        typingTime = typingTimer;
    }
    
    public static TypingSystem Instance
    {
        get
        {
            if (_instance == null)
                return null;

            return _instance;
        }
        
    }

    public static void Init()
    {
        if (_instance)
        {
            Destroy(_instance.gameObject);
            _instance = null;
        }
        _instance = new GameObject("[TypingSystem]").AddComponent<TypingSystem>();
        DontDestroyOnLoad(_instance.gameObject);
    }

    public void Typing(string[] dialogs, TextMeshProUGUI textObj, bool isClear = true)
    {
        isDialogEnd = false;
        texts = dialogs;
        tmpSave = textObj;
        if(isClear) 
            textObj.text = "";
        typingTime = typingTimer;
        if (dialogNumber < dialogs.Length)
        {
            char[] chars = dialogs[dialogNumber].ToCharArray();
            StartCoroutine(Typer(chars,textObj));
        }
        else
        {
            tmpSave.text = "";
            isDialogEnd = true;
            texts = null;
            tmpSave = null;
            dialogNumber = 0;
        }
    }


    private void Update()
    {
        if (!isTypingEnd)
        {
            InputAction action = InputManager.GetTalkEventAction("NextText");
            if (action != null && action.WasPressedThisFrame())
            {
                typingTime = typingTimer_fast;
            }
        }
    }
    
    IEnumerator Typer(char[] chars, TextMeshProUGUI textObj)
    {
        int currentChar = 0;
        int charLength = chars.Length;
        typingTime = typingTimer;
        isTypingEnd = false;

        while (currentChar < charLength)
        {
            if (timer >= 0)
            {
                yield return null;
                timer -= Time.unscaledDeltaTime;
            }
            else
            {
                if (chars[currentChar] == '<')
                {
                    string richText = "";
                    while (true)
                    {
                        richText += chars[currentChar].ToString();
                        if (chars[currentChar] == '>')
                        {
                            currentChar++;
                            break;
                        }
                        currentChar++;
                    }

                    textObj.text += richText;
                }
                else
                {
                    textObj.text += chars[currentChar].ToString();
                    currentChar++;
                }
                timer = typingTime;
            }

            if (currentChar >= charLength)
            {
                isTypingEnd = true;
                typingTime = typingTimer;
                dialogNumber++;
                if (texts.Length == dialogNumber)
                {
                    isDialogEnd = true;
                    texts = null;
                    tmpSave = null;
                    dialogNumber = 0;
                }
                yield break;
            }
            
        }
    }

}
