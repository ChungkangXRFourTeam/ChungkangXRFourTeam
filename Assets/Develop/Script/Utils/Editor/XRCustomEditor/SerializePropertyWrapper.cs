using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace XRProject.Utils.Editors
{
    public interface ISerializedPropertyWrapper
    {
        public SerializedProperty SerializeProperty { get; }
    }

    public struct Vector3SPW : ISerializedPropertyWrapper
    {
        public Vector3 Value
        {
            get => SerializeProperty.vector3Value;
            set => SerializeProperty.vector3Value = value;
        }

        public SerializedProperty SerializeProperty { get; private set; }

        public Vector3SPW(SerializedObject obj, string field)
        {
            this.SerializeProperty = obj.FindProperty(field);
        }

        public static implicit operator Vector3(Vector3SPW v)
        {
            return v.Value;
        }
    }
}