using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XRProject.Helper
{
    public interface IStrategy
    {
        public void Init(Blackboard blackboard);
        public void Update(Blackboard blackboard);

        public void Reset();
    }

    public class StrategyContainer
    {
        private class StrategyItem
        {
            public IStrategy Strategy;
            public bool IsEnabled;
        }
        
        private Dictionary<Type, StrategyItem> _table = new();

        public StrategyContainer Add(IStrategy strategy)
        {
            var type = strategy.GetType();
            if (_table.ContainsKey(type))
            {
                Debug.LogError($"{type.Name} class already added");
            }
            else
            {
                _table.Add(type, new StrategyItem{Strategy = strategy, IsEnabled = false});
            }

            return this;
        }

        public StrategyContainer SetActive<T>(bool value) where T : IStrategy
        {
            var type = typeof(T);
            if (!_table.TryGetValue(type, out var strategy))
            {
                Debug.LogError($"not exist {type.Name} class");
                throw new Exception();
            }

            strategy.IsEnabled = value;
            return this;
        }

        public T Get<T>() where T : IStrategy
        {
            var type = typeof(T);
            if (!_table.TryGetValue(type, out var strategy))
            {
                Debug.LogError($"not exist {type.Name} class");
                throw new Exception();
            }

            return (T)strategy.Strategy;
        }

        public void SetAllActive(bool value)
        {
            foreach (var item in _table.Values)
            {
                item.IsEnabled = value;
            }
        }

        public StrategyContainer Reset<T>() where T : IStrategy
        {
            Get<T>().Reset();
            return this;
        }
        public StrategyContainer ResetAll(bool onlyEnabled=false)
        {
            foreach (var item in _table.Values)
            {
                if (onlyEnabled)
                {
                    if (item.IsEnabled) item.Strategy.Reset();
                }
                else
                {
                    item.Strategy.Reset();
                }
            }

            return this;
        }
        public List<IStrategy> GetAll()
        {
            return _table.Values.Select(x=>x.Strategy).ToList();
        }

        public List<IStrategy> GetAllEnabled()
        {
            return _table.Values
                .Where(x => x.IsEnabled)
                .Select(x => x.Strategy)
                .ToList();
        }
    }

    public class StrategyExecutor
    {
        public StrategyContainer Container { get; private set; }
        public Blackboard Blackboard { get; private set; }

        private StrategyExecutor()
        {
        }

        public static StrategyExecutor Create(StrategyContainer container, Blackboard blackboard)
        {
            var e = new StrategyExecutor()
            {
                Container = container,
                Blackboard = blackboard
            };

            foreach (var item in container.GetAll())
            {
                item.Init(blackboard);
            }

            return e;
        }

        public void Execute()
        {
            foreach (var item in Container.GetAllEnabled())
            {
                item.Update(Blackboard);
            }
            
            var list = Blackboard.GetAllValue();
            foreach (var item in list)
            {
                if (item is WrappedTriggerValue wtv)
                {
                    wtv.Value = false;
                }
            }
        }
    }
}