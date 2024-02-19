using System;

namespace Mappee.Store.Models;

/// <summary>
/// Represents a type association between two types.
/// </summary>
internal class TypeAssociation : IEquatable<TypeAssociation>
{
    private Type _sourceType;
    private Type _destinationType;

    /// <summary>
    /// Initializes a new instance of the <see cref="TypeAssociation"/> class.
    /// </summary>
    /// <param name="sourceType">The source type.</param>
    /// <param name="destinationType">The destination type.</param>
    public TypeAssociation(Type sourceType, Type destinationType)
    {
        _sourceType = sourceType;
        _destinationType = destinationType;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TypeAssociation"/> class.
    /// </summary>
    public TypeAssociation()
    {
    }

    /// <summary>
    /// Sets the source type and destination type of the association.
    /// </summary>
    /// <param name="sourceType">The source type.</param>
    /// <param name="destinationType">The destination type.</param>
    public void Set(Type sourceType, Type destinationType)
    {
        _sourceType = sourceType;
        _destinationType = destinationType;
    }

    /// <summary>
    /// Determines whether the current <see cref="TypeAssociation"/> object is equal to another object of the same type.
    /// </summary>
    /// <param name="other">The object to compare with the current object.</param>
    /// <returns>true if the current object is equal to the other parameter; otherwise, false.</returns>
    public bool Equals(TypeAssociation other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return _sourceType == other._sourceType && _destinationType == other._destinationType;
    }

    /// <summary>
    /// Determines whether the current <see cref="TypeAssociation"/> object is equal to another object.
    /// </summary>
    /// <param name="obj">The object to compare with the current object.</param>
    /// <returns>true if the current object is equal to the other parameter; otherwise, false.</returns>
    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == GetType() && Equals((TypeAssociation)obj);
    }

    /// <summary>
    /// Serves as the default hash function.
    /// </summary>
    /// <returns>A hash code for the current object.</returns>
    public override int GetHashCode() => unchecked(_sourceType.GetHashCode() * 397) ^ _destinationType.GetHashCode();
}
