using Mappee.Store.Models;
using System;
using System.Reflection;

namespace Mappee.Store;

public static class InstanceStore
{
    private static readonly TypeList List = new();

    public static void Store(Type sourceType, Type destinationType, object instance, string methodName)
    {
        var functionPointer = instance.GetType().GetMethod(methodName, BindingFlags.Public | BindingFlags.Static)?.MethodHandle.GetFunctionPointer();
        TypeList.Add(new TypePair(sourceType, destinationType), new InstancePointer(instance, functionPointer));
    }

    public static unsafe TDestination Execute<TDestination>(Type sourceType, Type destinationType, object instance)
    {
        return (TDestination)((delegate*<object, object>)List[sourceType, destinationType].FunctionPointer)(instance);
    }
}