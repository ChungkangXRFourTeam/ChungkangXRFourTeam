using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace XRProject.Helper
{
    [System.Serializable]
    public class WrappedValue<T> where T : struct
    {
        public WrappedValue()
        {
        }
        public WrappedValue(T v)
        {
            Value = v;
        }
        [SerializeField] protected T _value;

        public virtual T Value
        {
            get => _value;
            set => _value = value;
        }

        public static implicit operator T(WrappedValue<T> c)
        {
            return c.Value;
        }
    }

    [Serializable]
    public class WrappedTriggerValue : WrappedValue<bool>
    {
        public WrappedTriggerValue()
        {
            Value = false;
        }
        public override bool Value
        {
            get
            {
                var temp = _value;
                return temp;
            }
            set => _value = value;
        }
    }

    [Serializable]
    public class WrappedNullableValue<T> : WrappedValue<T> where T : struct
    {
        public WrappedNullableValue()
        {
            HasValue = false;
        }

        public WrappedNullableValue(T value)
        {
            Value = value;
        }

        public override T Value
        {
            get { return _value; }
            set
            {
                _value = value;
                HasValue = true;
            }
        }

        public bool HasValue { get; private set; }

        public T Reset()
        {
            var temp = Value;
            HasValue = false;
            Value = default;
            return temp;
        }
    }

    public class Blackboard
    {
        private Dictionary<string, object> _table = new Dictionary<string, object>();

        public List<string> GetAllKey()
            => _table.Keys.ToList();

        public List<object> GetAllValue()
            => _table.Values.ToList();

        public Blackboard AddProperty(string key, object value)
        {
            if (!_table.TryAdd(key, value))
            {
                Debug.LogError($"'{key}' key already exist");
            }

            return this;
        }

        public Blackboard SetProperty(string key, object value)
        {
            if (!_table.ContainsKey(key))
            {
                Debug.LogError($"'{key}' key not exist");
            }

            _table[key] = value;
            return this;
        }


        public void SetWrappedProperty<T>(string key, T value) where T : struct
        {
            if (_table.TryGetValue(key, out var v))
            {
                if (v is WrappedValue<T> wv)
                {
                    wv.Value = value;
                }
                else
                {
                    throw new ArgumentException($"[{typeof(T).Name}]it is invalid type");
                }
            }
            else
            {
                throw new ArgumentException($"[{key}]it is invalid key");
            }
        }

        public object Internal_GetProperty(ref string key)
        {
            if (!_table.TryGetValue(key, out var obj))
            {
                throw new ArgumentException($"[{key}]it is invalid key");
            }

            return obj;
        }

        public object GetProperty(string key)
            => Internal_GetProperty(ref key);

        public T GetProperty<T>(string key) where T : class
        {
            object obj = Internal_GetProperty(ref key);
            var rtv = obj as T;
            if (rtv is null)
            {
                throw new ArgumentException($"[{typeof(T).Name}]it is invalid type");
            }

            return rtv;
        }

        public void GetProperty<T>(string key, out T result) where T : class
        {
            object obj = Internal_GetProperty(ref key);
            var rtv = obj as T;
            if (rtv is null)
            {
                throw new ArgumentException($"[{typeof(T).Name}]it is invalid type");
            }

            result = rtv;
        }

        public void GetPropertyOrNull<T>(string key, out T result) where T : class
        {
            object obj = Internal_GetProperty(ref key);
            var rtv = obj as T;

            result = rtv;
        }

        public T GetUnWrappedProperty<T>(string key) where T : struct
        {
            var wrapped = Internal_GetProperty(ref key);
            var castedWrapped = wrapped as WrappedValue<T>;

            if (castedWrapped is null)
            {
                throw new ArgumentException($"[{typeof(T).Name}]it is invalid type");
            }

            return castedWrapped.Value;
        }

        public void GetUnWrappedProperty<T>(string key, out T result) where T : struct
        {
            var wrapped = Internal_GetProperty(ref key);
            var castedWrapped = wrapped as WrappedValue<T>;

            if (castedWrapped is null)
            {
                throw new ArgumentException($"[{typeof(T).Name}]it is invalid type");
            }

            result = castedWrapped.Value;
        }

        public WrappedValue<T> GetWrappedProperty<T>(string key) where T : struct
        {
            var wrapped = Internal_GetProperty(ref key);
            var castedWrapped = wrapped as WrappedValue<T>;

            if (castedWrapped is null)
            {
                throw new ArgumentException($"[{typeof(T).Name}]it is invalid type");
            }

            return castedWrapped;
        }

        public void GetWrappedProperty<T>(string key, out WrappedValue<T> result) where T : struct
        {
            var wrapped = Internal_GetProperty(ref key);
            var castedWrapped = wrapped as WrappedValue<T>;

            if (castedWrapped is null)
            {
                throw new ArgumentException($"[{typeof(T).Name}]it is invalid type");
            }

            result = castedWrapped;
        }

        public void GetWrappedNullableProperty<T>(string key, out WrappedNullableValue<T> result) where T : struct
        {
            var wrapped = Internal_GetProperty(ref key);
            var castedWrapped = wrapped as WrappedNullableValue<T>;

            if (castedWrapped is null)
            {
                throw new ArgumentException($"[{typeof(T).Name}]it is invalid type");
            }

            result = castedWrapped;
        }

        public bool TryGetProperty<T>(string key, out T value) where T : class
        {
            if (_table.TryGetValue(key, out var obj))
            {
                value = obj as T;
                if (value is null)
                {
                    throw new ArgumentException($"[{typeof(T).Name}]it is invalid type");
                }

                return true;
            }

            value = null;
            return false;
        }
    }

    public class StateContainer
    {
        private Dictionary<Type, BaseState> _table = new Dictionary<Type, BaseState>();

        public BaseState InitialState { get; private set; }

        public StateContainer SetInitialState<T>() where T : BaseState, new()
        {
            var state = GetState<T>();
            InitialState = state;

            return this;
        }

        public StateContainer AddState<T>() where T : BaseState, new()
        {
            var type = typeof(T);
            if (_table.ContainsKey(type))
            {
                Debug.LogError($"{type.Name} class already added");
            }
            else
            {
                var obj = new T();
                _table.Add(type, obj);
            }

            return this;
        }

        public StateContainer RemoveState<T>() where T : BaseState
        {
            if (!_table.Remove(typeof(T)))
            {
                Debug.LogError($"not exist {typeof(T).Name} class");
            }

            return this;
        }

        public T GetState<T>() where T : BaseState
        {
            var type = typeof(T);
            if (!_table.TryGetValue(type, out var strategy))
            {
                Debug.LogError($"not exist {type.Name} class");
                throw new Exception();
            }

            return strategy as T;
        }

        public List<BaseState> GetAllState()
        {
            return _table.Values.ToList();
        }
    }

    public class StateExecutor
    {
        #region Generation

        private StateExecutor()
        {
        }

        public static StateExecutor Create(StateContainer container, Blackboard blackboard)
        {
            var obj = new StateExecutor();
            obj.Enabled = true;
            obj._container = container;
            obj._currentState = container.InitialState;
            obj.Blackboard = blackboard;

            foreach (var state in container.GetAllState())
            {
                state.Init(blackboard);
            }

            return obj;
        }

        #endregion

        public StateContainer _container;
        public Blackboard Blackboard { get; private set; }

        public bool IsDebug { get; set; }
        public bool Enabled { get; set; }

        private BaseState _nextState;
        private BaseState _currentState;

        public BaseState CurrentState => _currentState;

        public void SetNextState<T>() where T : BaseState
        {
            _nextState = _container.GetState<T>();
        }

        public void Execute()
        {
            if (!Enabled)
            {
                return;
            }

            bool loop = false;
            int count = 0;
            List<string> names = new List<string>();
            
            do
            {
                if (IsDebug)
                    Debug.Log(_currentState.GetType().Name);
                loop = _currentState.Update(Blackboard, this);

                if (_nextState == null) break;
                
                var list = Blackboard.GetAllValue();
                foreach (var item in list)
                {
                    if (item is WrappedTriggerValue wtv)
                    {
                        wtv.Value = false;
                    }
                }
                
                names.Add(_currentState.GetType().Name);

                _currentState.Exit(Blackboard);
                _currentState = _nextState;
                _nextState = null;
                _currentState.Enter(Blackboard);

                
                count++;

                if (count >= 30)
                {
                    string str = "state iteration over 30\n";
                    foreach (var item in names)
                    {
                        str += item + "\n";
                    }
                    Debug.LogError(str);
                    loop = false;
                }
            } while (loop);
        }
    }

    public abstract class BaseState
    {
        public virtual void Init(Blackboard blackboard)
        {
        }

        public virtual void Enter(Blackboard blackboard)
        {
        }

        public virtual bool Update(Blackboard blackboard, StateExecutor executor)
        {
            return false;
        }

        public virtual void Exit(Blackboard blackboard)
        {
        }

        public virtual void Release(Blackboard blackboard)
        {
        }
    }
}