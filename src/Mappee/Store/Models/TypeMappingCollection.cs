using System;
using System.Collections.Generic;

namespace Mappee.Store.Models;

internal class TypeMappingCollection
{
    private static readonly List<TypeAssociation> Types = new();

    private static readonly List<MethodExecutionPointer> Instances = new();

    /// <summary>
    /// Adds a type association and method execution pointer to the collection.
    /// </summary>
    /// <param name="typeAssociation">The type association to add.</param>
    /// <param name="methodExecution">The method execution pointer to add.</param>
    public static void Add(TypeAssociation typeAssociation, MethodExecutionPointer methodExecution)
    {
        Types.Add(typeAssociation);
        Instances.Add(methodExecution);
    }

    private static readonly TypeAssociation TypeAssociation = new();

    /// <summary>
    /// Gets the method execution pointer for the specified types.
    /// </summary>
    /// <param name="a">The first type.</param>
    /// <param name="b">The second type.</param>
    /// <returns>The method execution pointer.</returns>
    public MethodExecutionPointer this[Type a, Type b]
    {
        get
        {
            TypeAssociation.Set(a, b);
            return Instances[Types.IndexOf(TypeAssociation)];
        }
    }
}