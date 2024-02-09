using System;

namespace Mappee.Store.Models;

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
        return obj.GetType() == GetType() && Equals((TypePair)obj);
    }

    public override int GetHashCode() => unchecked(_sourceType.GetHashCode() * 397) ^ _destinationType.GetHashCode();
}