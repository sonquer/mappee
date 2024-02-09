using System;
using System.Collections.Generic;

namespace Mappee.Configuration.Models;

internal class TypeCodeMapping
{
    public string MethodName { get; set; }

    public Type SourceType { get; set; }

    public Type TargetType { get; set; }

    public List<string> Mappings { get; set; } = new();
}