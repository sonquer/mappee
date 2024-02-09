using System;

namespace Mappee.Store.Models;

internal record InstancePointer
{
    public object Instance { get; set; }

    public IntPtr? FunctionPointer;

    public InstancePointer(object instance, nint? functionPointer)
    {
        Instance = instance;
        FunctionPointer = functionPointer;
    }
};