using System;
using System.Collections.Generic;
using UnityEngine;

namespace Bayat.Core.Reflection
{

    public static class TypeUtility
    {

        public static readonly HashSet<Type> NumericTypes = new HashSet<Type>
        {
            typeof(byte),
            typeof(sbyte),
            typeof(short),
            typeof(ushort),
            typeof(int),
            typeof(uint),
            typeof(long),
            typeof(ulong),
            typeof(float),
            typeof(double),
            typeof(decimal)
        };

        public static readonly HashSet<Type> NumericConstructTypes = new HashSet<Type>
        {
            typeof(Vector2),
            typeof(Vector3),
            typeof(Vector4),
            typeof(Quaternion),
            typeof(Matrix4x4),
            typeof(Rect),
        };

        public static readonly HashSet<Type> TypesWithShortStrings = new HashSet<Type>()
        {
            typeof(string),
            typeof(Vector2),
            typeof(Vector3),
            typeof(Vector4)
        };

        public static readonly Dictionary<Type, object> DefaultPrimitives = new Dictionary<Type, object>()
        {
            { typeof(int), default(int) },
            { typeof(uint), default(uint) },
            { typeof(long), default(long) },
            { typeof(ulong), default(ulong) },
            { typeof(short), default(short) },
            { typeof(ushort), default(ushort) },
            { typeof(byte), default(byte) },
            { typeof(sbyte), default(sbyte) },
            { typeof(float), default(float) },
            { typeof(double), default(double) },
            { typeof(decimal), default(decimal) },
            { typeof(Vector2), default(Vector2) },
            { typeof(Vector3), default(Vector3) },
            { typeof(Vector4), default(Vector4) },
        };

        public static bool IsValueType<T>(T obj)
        {
            return typeof(T).IsValueType();
        }

    }

}