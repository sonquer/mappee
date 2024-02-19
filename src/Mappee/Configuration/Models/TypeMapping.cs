using System;
using System.Collections.Generic;

namespace Mappee.Configuration.Models;

/// <summary>
/// Type mapping, contains information about source and target types and mappings between them.
/// </summary>
internal class TypeMapping
{
    /// <summary>
    /// Method name, used to generate method name for compiled mapping.
    /// </summary>
    public string MethodName { get; set; }

    /// <summary>
    /// Source type.
    /// </summary>
    public Type SourceType { get; set; }

    /// <summary>
    /// Target type.
    /// </summary>
    public Type TargetType { get; set; }

    /// <summary>
    /// Mapping entries, contains information about how to map source to destination.
    /// </summary>
    public List<TypeMappingEntry> Mappings { get; set; } = new();
}