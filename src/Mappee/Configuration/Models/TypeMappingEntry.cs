namespace Mappee.Configuration.Models;

/// <summary>
/// Type mapping entry, contains information about how to map source to destination.
/// </summary>
internal record TypeMappingEntry
{
    /// <summary>
    /// Mapping type, used to determine how to map source to destination.
    /// </summary>
    public MappingType Type { get; set; }

    /// <summary>
    /// Plain c# code, used to map source to destination, can be used with MappingType.Plain.
    /// </summary>
    public string Code { get; set; }

    /// <summary>
    /// Source member name, used to map source to destination, can be used with MappingType.PropertyToProperty and MappingType.ConstantToProperty.
    /// </summary>
    public string SourceMember { get; set; }

    /// <summary>
    /// Destination member name, used to map source to destination, can be used with MappingType.PropertyToProperty.
    /// </summary>
    public string DestinationMember { get; set; }

    /// <summary>
    /// Source constant value, used to map source to destination, can be used with MappingType.ConstantToProperty.
    /// </summary>
    public string SourceAsString { get; set; }
}