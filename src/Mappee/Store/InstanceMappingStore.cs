using Mappee.Store.Models;
using System;
using System.Reflection;

namespace Mappee.Store;

/// <summary>
/// Stores the mapping between source and destination types along with the corresponding methodExecution and method.
/// </summary>
public static class InstanceMappingStore
{
    private static readonly TypeMappingCollection MappingCollection = new();

    /// <summary>
    /// Stores the mapping between source and destination types along with the corresponding methodExecution and method.
    /// </summary>
    /// <param name="sourceType">The source type.</param>
    /// <param name="destinationType">The destination type.</param>
    /// <param name="instance">The methodExecution.</param>
    /// <param name="methodName">The name of the method.</param>
    public static void Append(Type sourceType, Type destinationType, object instance, string methodName)
    {
        var functionPointer = instance.GetType().GetMethod(methodName, BindingFlags.Public | BindingFlags.Static)?.MethodHandle.GetFunctionPointer();
        TypeMappingCollection.Add(new TypeAssociation(sourceType, destinationType), new MethodExecutionPointer(instance, functionPointer));
    }

    /// <summary>
    /// Executes the mapping for the specified source and destination types and returns the result.
    /// </summary>
    /// <typeparam name="TDestination">The destination type.</typeparam>
    /// <param name="sourceType">The source type.</param>
    /// <param name="destinationType">The destination type.</param>
    /// <param name="instance">The methodExecution.</param>
    /// <returns>The result of the mapping.</returns>
    public static TDestination Execute<TDestination>(Type sourceType, Type destinationType, object instance) => MappingCollection[sourceType, destinationType].Execute<TDestination>(instance);
}
