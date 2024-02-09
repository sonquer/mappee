using Mappee.Store.Models;
using System;
using System.Reflection;

namespace Mappee.Store;

public static class InstanceStore
{
    private static readonly TypeList List = new();

    public static void Store(Type src, Type dst, object instance, string methodName)
    {
        var functionPointer = instance.GetType().GetMethod(methodName, BindingFlags.Public | BindingFlags.Static)?.MethodHandle.GetFunctionPointer();
        TypeList.Add(new TypePair(src, dst), new InstancePointer(instance, functionPointer));
    }

    public static unsafe T2 Execute<T2>(Type src, Type dest, object instance)
    {
        return (T2)((delegate*<object, object>)List[src, dest].FunctionPointer)(instance);
    }
}