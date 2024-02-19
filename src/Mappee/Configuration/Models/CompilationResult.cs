using System.Reflection;

namespace Mappee.Configuration.Models;

/// <summary>
/// Compilation result, contains generated code, compiled assembly and methodExecution of compiled type.
/// </summary>
public class CompilationResult
{
    /// <summary>
    /// Generated source code.
    /// </summary>
    public string GeneratedCode { get; set; }

    /// <summary>
    /// Compiled assembly.
    /// </summary>
    public Assembly Assembly { get; set; }

    /// <summary>
    /// Instance of compiled type.
    /// </summary>
    public object Instance { get; set; }
}