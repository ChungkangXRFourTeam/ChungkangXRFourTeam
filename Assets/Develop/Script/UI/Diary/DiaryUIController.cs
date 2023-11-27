using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class DiaryUIController : MonoBehaviour, IUIController
{
    [SerializeField] private DiaryUIView _view;
    
    private List<Dictionary<string, object>> _table;

    private const string PAGE_ID_KEY = "page_id";
    private const string CONTENT_KEY = "content";
    private const string CSV_PATH = "EventTextScript/Diary/DiaryContent";


    private UIManager _manager;
    private LinkedList<int> _collectedPages = new();
    private LinkedListNode<int> _currentNode;
    private void Awake()
    {
        _table = CSVReader.Read(CSV_PATH);
        DontDestroyOnLoad(gameObject);

        _manager = GameObject.Find("UIManager")?.GetComponent<UIManager>();
    }
    
    public bool IsEnabled
    {
        get => gameObject.activeSelf;
        set
        {
            if (value)
            {
                Activate();
            }
            else
            {
                DeActivate();
            }
        }
    }

    public void AddPage(int index)
    {
        if (_collectedPages.Contains(index)) return;

        var node = _collectedPages.Last;
        if (node == null)
        {
            _collectedPages.AddLast(index);
            _currentNode = _collectedPages.Last;
        }
        else
        {
            bool find = false;
            while (node != null)
            {
                if (index < node.Value)
                {
                    find = true;
                    _currentNode = _collectedPages.AddAfter(node, index);
                    break;
                }
                node = node.Next;
            }

            if (!find) _currentNode = _collectedPages.AddLast(index);
        }
    }
    public void Activate()
    {
        gameObject.SetActive(true);
        TryShowText(_currentNode);
    }
    public void DeActivate()
    {
        gameObject.SetActive(false);
    }

    public void OnClickCloseButton()
    {
        _manager?.Clear();
    }

    [CanBeNull]
    public string GetPageContent(int index)
    {
        foreach (var item in _table)
        {
            if (item.TryGetValue(PAGE_ID_KEY, out object pageId) == false) continue;
            if (item.TryGetValue(CONTENT_KEY, out object content) == false) continue;

            if (pageId is not int id) continue;
            if (id != index) continue;
            
            if (content is not string str) continue;
            return str;
        }

        return null;
    }
    

    public void OnClickNextButton()
    {
        if (TryShowText(_currentNode?.Next)) _currentNode = _currentNode?.Next;
    }
    public void OnClickPrevButton()
    {
        if (TryShowText(_currentNode?.Previous)) _currentNode = _currentNode?.Previous;
    }

    private bool TryShowText(int index)
    {
        string content = GetPageContent(index);
        if (string.IsNullOrEmpty(content)) return false;
        
        _view.SetText(content);
        return true;
    }
    private bool TryShowText(LinkedListNode<int> node)
    {
        if (node == null) return false;

        return TryShowText(node.Value);
    }
}