using Mappe.Configuration.Interfaces;
using System;
using System.Collections.Generic;

namespace Mappe.Configuration
{
    public static class InstanceStore
    {
        private static readonly Dictionary<TypePair, object> Instances = new Dictionary<TypePair, object>();

        public static void Store(Type src, Type dst, object instance)
        {
            Instances.Add(new TypePair(src, dst), instance);
        }

        public static IMapper<T1, T2> GetInstance<T1, T2>(Type src, Type dest)
        {
            return (IMapper<T1, T2>)Instances[new TypePair(src, dest)];
        }
    }

    internal class TypePair : IEquatable<TypePair>
    {
        private readonly Type _sourceType;

        private readonly Type _destinationType;

        public TypePair(Type sourceType, Type destinationType)
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
            if (obj.GetType() != GetType()) return false;
            return Equals((TypePair) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((_sourceType != null ? _sourceType.GetHashCode() : 0) * 397) ^ (_destinationType != null ? _destinationType.GetHashCode() : 0);
            }
        }
    }
}
