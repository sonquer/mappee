using System;

namespace Mappee.Store.Models;

/// <summary>
/// Represents a pointer to a method execution.
/// </summary>
internal class MethodExecutionPointer
{
    /// <summary>
    /// Gets the instance on which the method will be executed.
    /// </summary>
    public object Instance { get; }

    /// <summary>
    /// Gets the function pointer to the method.
    /// </summary>
    public IntPtr? FunctionPointer { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MethodExecutionPointer"/> class.
    /// </summary>
    /// <param name="instance">The instance on which the method will be executed.</param>
    /// <param name="functionPointer">The function pointer to the method.</param>
    public MethodExecutionPointer(object instance, nint? functionPointer)
    {
        Instance = instance;
        FunctionPointer = functionPointer;
    }

    /// <summary>
    /// Executes the method with the specified instance and returns the result.
    /// </summary>
    /// <typeparam name="TDestination">The type of the result.</typeparam>
    /// <param name="instance">The instance on which the method will be executed.</param>
    /// <returns>The result of the method execution.</returns>
    public unsafe TDestination Execute<TDestination>(object instance) => ((delegate*<object, TDestination>)FunctionPointer)(instance);
}