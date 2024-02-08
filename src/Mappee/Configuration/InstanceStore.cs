using System;
using System.Collections.Generic;
using System.Reflection;

namespace Mappe.Configuration
{
    public static class InstanceStore
    {
        private static readonly TypeList Instances = new();

        public static void Store(Type src, Type dst, object instance, string methodName)
        {
            var functionPointer = instance.GetType().GetMethod(methodName, BindingFlags.Public | BindingFlags.Static)?.MethodHandle.GetFunctionPointer();
            TypeList.Add(new TypePair(src, dst), new ClassInstance(instance, functionPointer));
        }

        public static unsafe T2 Execute<T2>(Type src, Type dest, object instance)
        {
            return (T2)((delegate*<object, object>)Instances[src, dest].FunctionPointer)(instance);
        }
    }

    internal class TypeList
    {
        private static readonly List<TypePair> _types = new();

        private static readonly List<ClassInstance> _instances = new();

        public static void Add(TypePair typePair, ClassInstance instance)
        {
            _types.Add(typePair);
            _instances.Add(instance);
        }

        private static readonly TypePair _typePair = new();

        public ClassInstance this[Type a, Type b] {
            get
            {
                _typePair.Set(a, b);
                return _instances[_types.IndexOf(_typePair)];
            }
        }
    }

    internal class TypePair : IEquatable<TypePair>
    {
        private Type _sourceType;

        private Type _destinationType;

        public TypePair(Type sourceType, Type destinationType)
        {
            _sourceType = sourceType;
            _destinationType = destinationType;
        }

        public TypePair()
        {
        }

        public void Set(Type sourceType, Type destinationType)
        {
            _sourceType = sourceType;
            _destinationType = destinationType;
        }

        public bool Equals(TypePair other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return _sourceType == other._sourceType && _destinationType == other._destinationType;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((TypePair) obj);
        }

        public override int GetHashCode() => unchecked (_sourceType.GetHashCode() * 397) ^ _destinationType.GetHashCode();
    }

    internal class ClassInstance
    {
        public object Instance { get; set; }

        public IntPtr? FunctionPointer;

        public ClassInstance(object instance, IntPtr? functionPointer)
        {
            Instance = instance;
            FunctionPointer = functionPointer;
        }
    };
}
