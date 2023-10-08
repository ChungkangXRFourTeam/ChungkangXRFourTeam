using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace  XRProject.Utils.Editors
{
    public struct DirtyContainer<T> where T : IEquatable<T>
    {
        private T _value;
        private T _prevValue;

        public DirtyContainer(T v)
        {
            _value = _prevValue = v;
            IsDirty = false;
        }

        public bool IsDirty { get; private set; }
        public void SetDirtyOff() => IsDirty = false;

        public T PrevValue => _prevValue;

        public T Value
        {
            get => _value;
            set
            {
                if (!value.Equals(_value))
                {
                    _prevValue = _value;
                    _value = value;
                    IsDirty = true;
                }
            }
        }

        public static implicit operator T(DirtyContainer<T> v)
        {
            return v.Value;
        }
    }
}