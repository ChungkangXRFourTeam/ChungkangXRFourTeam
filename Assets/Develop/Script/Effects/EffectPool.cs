using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using XRProject.Utils.Log;

public class EffectPrototypeContainer
{
    private const string B_LOG_SIGNATURE = "EffectPrototypeContainer";
    
    private Dictionary<string, Dictionary<string, GameObject>> _tableList = new();

    public EffectPrototypeContainer(List<EffectTable> tables)
    {
        if (tables == null || tables.Count == 0) return;

        foreach (var effectTable in tables)
        {
            if (!effectTable) continue;

            Dictionary<string, GameObject> table = new();

            var tableKey = effectTable.TableKey;
            if (!ValidKey(ref tableKey))
            {
                XLog.LogError($"{B_LOG_SIGNATURE}: Table('{effectTable.name}')의 TableKey키가 유효하지 않습니다.",EffectManager.LOG_SIGNATURE);
                continue;
            }
            
            _tableList.Add(tableKey, table);

            if (CheckValidKeyInTable(tables))
            {
                foreach (var pair in effectTable.Table)
                {
                    var obj = GameObject.Instantiate(pair.Effect);
                    obj.SetActive(false);
                    table.Add(pair.Key, pair.Effect);
                }
            }
        }
    }

    private static bool ValidKey(ref string key)
    {
        return !string.IsNullOrEmpty(key) || !string.IsNullOrWhiteSpace(key);
    }

    private bool CheckValidKeyInTable(List<EffectTable> a)
    {
        bool invalid = false;
        StringBuilder keyMsgBuilder = new StringBuilder();

        foreach (var table in a)
        {
            if (!table) continue;
            var list = table.Table;

            for (int i = 0; i < list.Count; i++)
            {
                var item1 = list[i];
                if (!ValidKey(ref item1.Key))
                {
                    keyMsgBuilder.Append($"EffectContainer: 올바르지 않은 EffectKey: ('{item1.Key}')");
                    invalid = true;
                    continue;
                }

                for (int j = i; j < list.Count; j++)
                {
                    if(i == j)continue;
                    var item2 = list[j];

                    if (item1.Key == item2.Key)
                    {
                        keyMsgBuilder.Append($"EffectContainer: 키가 중복되었습니다. EffectKey: ('{item1.Key}', '{item2.Key}')");
                        invalid = true;
                    }
                }
            }

            if (invalid)
            {
                XLog.LogError(
                    $"EffectContainer: EffectTable('{table.name}')에서 다음과 같은 문제 발생\n{keyMsgBuilder.ToString()}",
                    "default"
                    );
            }
        }


        return !invalid;
    }

    public GameObject CreateOrNull(string tableKey, string itemKey)
    {
        if (_tableList.TryGetValue(tableKey, out var dict))
        {
            if (dict.TryGetValue(itemKey, out var effect))
            {
                return GameObject.Instantiate(effect);
            }
        }

        return null;
    }
}

public class EffectItem
{
    public string TableKey => Container.TableKey;
    public string ItemKey => Container.ItemKey;
    public string EffectKey => $"{TableKey}/{ItemKey}";
    private EffectPoolItemContainer Container { get; set; }
    public ParticleSystem ParticleSystem { get; private set; }
    public GameObject EffectObject { get; private set; } 
    private InteractionController _interaction;
    public InteractionController Interaction
    {
        get
        {
            if (ReferenceEquals(_interaction, null) && ReferenceEquals(EffectObject, null) == false)
            {
                if (EffectObject.TryGetComponent<InteractionController>(out var interaction))
                {
                    _interaction = interaction;
                    
                    if (ProxyContractInfo == null)
                        _interaction.SetContractInfo(ActorContractInfo.Create(EffectObject.transform, () => false));
                    else
                        _interaction.SetContractInfo(ProxyContractInfo);
                }
            }

            return _interaction;
        }
    }
    public BaseContractInfo ProxyContractInfo { get; private set; }

    public bool IsInteractionNull => ReferenceEquals(Interaction, null);

    private bool _isEnabled;
    public bool IsEnabled
    {
        get => _isEnabled;
        set
        {
            _isEnabled = value;
            EffectObject.SetActive(value);
        }
    }
    public EffectItem(GameObject obj, EffectPoolItemContainer container)
    {
        EffectObject = obj;
        Container = container;
        ParticleSystem = obj.GetComponentInChildren<ParticleSystem>();
        IsEnabled = false;
    }

    public void ApplyCommand(ref EffectCommand command)
    {
        var transform = EffectObject.transform;
        var module = ParticleSystem.main;
        var onContractActor = command.OnContractActor;
        
        transform.position = command.Position ?? Vector3.zero;
        transform.rotation = command.Rotation ?? Quaternion.identity;
        if(command.Scale.HasValue)
            transform.localScale = command.Scale.Value;
        module.flipRotation = command.FlipRotation;

        ProxyContractInfo = command.ProxyContractInfo;
        if (!IsInteractionNull)
        {
            Interaction.OnContractActor += x =>
            {
                onContractActor?.Invoke(this, x);
            };   
        }
    }
    public void ApplyCommand(EffectCommand command)
    {
        ApplyCommand(ref command);
    }

    public void Reset()
    {
        if (!IsInteractionNull)
        {
            Interaction.ClearContractEvent();
            Interaction.IsEnabled = true;
        }

        IsEnabled = false;
    }
}

public class EffectPoolItemContainer
{
    public string TableKey { get; private set; }
    public string ItemKey { get; private set; }
    private EffectPrototypeContainer _prototypeContainer;
    private Stack<EffectItem> _stack;

    public EffectPoolItemContainer(EffectPrototypeContainer prototypeContainer, string tableKey, string itemKey,
        GameObject effect)
    {
        _prototypeContainer = prototypeContainer;
        ItemKey = itemKey;
        TableKey = tableKey;
        _stack = new Stack<EffectItem>();
    }

    public EffectItem Pop()
    {
        if (_stack.Count == 0)
        {
            var obj = _prototypeContainer.CreateOrNull(TableKey, ItemKey);

            if (obj == null)
                throw new NullReferenceException($"EffectPool: effect를 불러올 수 없음.\n'{TableKey}/{ItemKey}'");

            var effect = new EffectItem(obj, this);
            effect.Reset();
            _stack.Push(effect);
        }

        return _stack.Pop();
    }

    public void Push(EffectItem item)
    {
        item.Reset();
        _stack.Push(item);
    }
    
    public void Reset()
    {
        foreach (var item in _stack)
        {
            GameObject.Destroy(item.EffectObject);
        }

        _stack.Clear();
    }
}

public class EffectPool
{
    private const string B_LOG_SIGNAUTRE = "EffectPool";
    private EffectPrototypeContainer _container;

    private Dictionary<string, EffectPoolItemContainer> _table;

    public EffectPool(List<EffectTable> tables)
    {
        _container = new(tables);
        _table = new Dictionary<string, EffectPoolItemContainer>();
        StringBuilder builder = new StringBuilder();
        
        builder.Append($"{B_LOG_SIGNAUTRE}: 로드된 이펙트 목록\n");
        
        foreach (EffectTable table in tables)
        {
            foreach (EffectKeyPair effect in table.Table)
            {
                var key = $"{table.TableKey}/{effect.Key}";
                _table.Add(key, new EffectPoolItemContainer(
                    _container, 
                    table.TableKey, 
                    effect.Key, 
                    effect.Effect)
                );
                builder.Append(key);
                builder.Append("\n");
            }
        }
        XLog.LogDebug(builder.ToString(), EffectManager.LOG_SIGNATURE);
        
    }

    [CanBeNull]
    public EffectItem Pop(string effectKey)
    {
        if (string.IsNullOrEmpty(effectKey) || string.IsNullOrWhiteSpace(effectKey))
        {
            XLog.LogError($"{B_LOG_SIGNAUTRE}: 유효하지 않은 effectKey('{effectKey}')로 pop을 시도했습니다.",EffectManager.LOG_SIGNATURE);
            return null;
        }
        if (_table.TryGetValue(effectKey, out var container))
        {
            return container.Pop();
        }

        return null;
    }

    public void Push(EffectItem effect)
    {
        if (_table.TryGetValue(effect.EffectKey, out var container))
        {
            container.Push(effect);
        }
    }
}