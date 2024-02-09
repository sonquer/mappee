using System;
using System.Collections.Generic;

namespace Mappee.Store.Models;

internal class TypeList
{
    private static readonly List<TypePair> Types = new();

    private static readonly List<InstancePointer> Instances = new();

    public static void Add(TypePair typePair, InstancePointer instance)
    {
        Types.Add(typePair);
        Instances.Add(instance);
    }

    private static readonly TypePair TypePair = new();

    public InstancePointer this[Type a, Type b]
    {
        get
        {
            TypePair.Set(a, b);
            return Instances[Types.IndexOf(TypePair)];
        }
    }
}