using System.Reflection;

namespace Mappee.Configuration.Models;

public class CompilationResult
{
    public string GeneratedCode { get; set; }

    public Assembly Assembly { get; set; }

    public object Instance { get; set; }
}