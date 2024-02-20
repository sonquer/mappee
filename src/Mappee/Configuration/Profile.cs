using Mappee.Configuration.Attributes;
using Mappee.Configuration.Models;
using Mappee.Store;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Mappee.Configuration;

/// <summary>
/// Mapping profile, used to configure mappings between source and destination types.
/// </summary>
public class Profile : IProfile
{
    private Type _sourceType = typeof(object);

    private Type _destinationType = typeof(object);

    private readonly Dictionary<string, TypeMapping> _mappings = new();

    private const string Namespace = $"{nameof(Mappee)}GeneratedCode";

    private const string ClassName = $"__{nameof(Mappee)}_{nameof(Profile)}_Mapping";

    private static Dictionary<string, List<Delegate>> Actions { get; } = new();

    /// <summary>
    /// Executes the actions associated with the specified key.
    /// </summary>
    /// <typeparam name="TSource">The type of the source object.</typeparam>
    /// <typeparam name="TDestination">The type of the destination object.</typeparam>
    /// <param name="key">The key associated with the actions.</param>
    /// <param name="source">The source object.</param>
    /// <param name="destination">The destination object.</param>
    public static void ExecuteActions<TSource, TDestination>(string key, TSource source, TDestination destination)
    {
        if (Actions.ContainsKey(key) == false) return;
        foreach (var action in Actions[key].OfType<Action<TSource, TDestination>>())
        {
            action(source, destination);
        }
    }

    /// <summary>
    /// Generates the method name for mapping between the source and destination types.
    /// </summary>
    /// <param name="source">The source type.</param>
    /// <param name="destination">The destination type.</param>
    /// <returns>The generated method name.</returns>
    private static string GenerateMethodName(Type source, Type destination) => $"__{source.Namespace}_{source.Name}_{destination.Namespace}_{destination.Name}".Replace(".", "_");

    /// <summary>
    /// Binds the source and destination types.
    /// </summary>
    /// <typeparam name="TSource">The source type.</typeparam>
    /// <typeparam name="TDestination">The destination type.</typeparam>
    /// <returns>The profile methodExecution.</returns>
    public Profile Bind<TSource, TDestination>()
    {
        _sourceType = typeof(TSource);
        _destinationType = typeof(TDestination);

        CreateMappings(typeof(TSource), typeof(TDestination));

        return this;
    }

    /// <summary>
    /// Adds a function to be executed before the mapping of objects of type <typeparamref name="TSource"/> to <typeparamref name="TDestination"/>.
    /// </summary>
    /// <typeparam name="TSource">The source type.</typeparam>
    /// <typeparam name="TDestination">The destination type.</typeparam>
    /// <param name="function">The function to be executed.</param>
    /// <returns>The updated profile.</returns>
    public Profile BeforeMap<TSource, TDestination>(Action<TSource, TDestination> function)
    {
        return AddAction(nameof(BeforeMap), function);
    }

    /// <summary>
    /// Adds a function to be executed after the mapping of objects of type <typeparamref name="TSource"/> to <typeparamref name="TDestination"/>.
    /// </summary>
    /// <typeparam name="TSource">The source type.</typeparam>
    /// <typeparam name="TDestination">The destination type.</typeparam>
    /// <param name="function">The function to be executed.</param>
    /// <returns>The updated profile.</returns>
    public Profile AfterMap<TSource, TDestination>(Action<TSource, TDestination> function)
    {
        return AddAction(nameof(AfterMap), function);
    }

    private Profile AddAction<TSource, TDestination>(string key, Action<TSource, TDestination> function)
    {
        if (Actions.ContainsKey(key) == false)
        {
            Actions.Add(key, new List<Delegate>());
        }

        Actions[key].Add(function);

        return this;
    }   

    /// <summary>
    /// Ignores a member during mapping.
    /// </summary>
    /// <typeparam name="TSource">The source type.</typeparam>
    /// <typeparam name="TDestination">The destination type.</typeparam>
    /// <param name="source">The source member expression.</param>
    /// <returns>The profile methodExecution.</returns>
    public Profile IgnoreMember<TSource, TDestination>(Expression<Func<TSource, object>> source)
    {
        var methodName = GenerateMethodName(typeof(TSource), typeof(TDestination));
        var sourceMember = GetMemberInfo(source);

        AddMapping(typeof(TSource), typeof(TDestination), methodName, MappingType.IgnoreMapping, sourceMember.Name);

        return this;
    }

    /// <summary>
    /// Specifies a mapping for a member from the source type to the destination type.
    /// </summary>
    /// <typeparam name="TSource">The source type.</typeparam>
    /// <typeparam name="TDestination">The destination type.</typeparam>
    /// <param name="source">The source member expression.</param>
    /// <param name="destination">The destination member expression.</param>
    /// <returns>The profile methodExecution.</returns>
    public Profile ForMember<TSource, TDestination>(Expression<Func<TSource, object>> source, Expression<Func<TDestination, object>> destination)
    {
        var methodName = GenerateMethodName(typeof(TSource), typeof(TDestination));
        var sourceMember = GetMemberInfo(source);
        var destinationMember = GetMemberInfo(destination);

        AddMapping(typeof(TSource), typeof(TDestination), methodName, MappingType.PropertyToProperty, sourceMember.Name, destinationMember.Name);

        return this;
    }

    /// <summary>
    /// Member mapping, can be used to map specific member to constant value.
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TDestination"></typeparam>
    /// <param name="source"></param>
    /// <param name="destination"></param>
    /// <returns>The profile methodExecution.</returns>
    public Profile ForMember<TSource, TDestination>(string source, Expression<Func<TDestination, object>> destination)
    {
        var methodName = GenerateMethodName(typeof(TSource), typeof(TDestination));
        var destinationMember = GetMemberInfo(destination);

        AddMapping(typeof(TSource), typeof(TDestination), methodName, MappingType.PropertyToConst, sourceAsString: source, destinationMemberName: destinationMember.Name);

        return this;
    }

    /// <summary>
    /// Gets the member information from the expression.
    /// </summary>
    /// <param name="expression"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    private static MemberInfo GetMemberInfo(Expression expression)
    {
        if (expression is LambdaExpression lambdaExpression)
        {
            if (lambdaExpression.Body is MemberExpression memberExpression)
            {
                return memberExpression.Member;
            }

            if (lambdaExpression.Body is UnaryExpression unaryExpression)
            {
                if (unaryExpression.Operand is MemberExpression operand)
                {
                    return operand.Member;
                }
            }
        }

        throw new ArgumentException("Invalid expression");
    }

    /// <summary>
    /// Creates mappings between the source and destination types.
    /// </summary>
    /// <param name="source">The source type.</param>
    /// <param name="destination">The destination type.</param>
    private void CreateMappings(Type source, Type destination)
    {
        var sourceProperties = source.GetProperties();
        var destinationProperties = destination.GetProperties();

        foreach (var sourceProperty in sourceProperties)
        {
            var destinationProperty = destinationProperties.FirstOrDefault(p => p.Name == sourceProperty.Name);
            if (destinationProperty != null)
            {
                CreateMappingForField(source, destination, sourceProperty, destinationProperty);
            }
        }
    }

    /// <summary>
    /// Creates a mapping for a field between the source and destination types.
    /// </summary>
    /// <param name="source">The source type.</param>
    /// <param name="destination">The destination type.</param>
    /// <param name="sourceProperty">The source property.</param>
    /// <param name="destinationProperty">The destination property.</param>
    private void CreateMappingForField(Type source, Type destination, PropertyInfo sourceProperty, PropertyInfo destinationProperty)
    {
        var methodName = GenerateMethodName(source, destination);

        if (sourceProperty.GetCustomAttributes(typeof(IgnoreMemberAttribute), true).Any())
        {
            AddMapping(source, destination, methodName, MappingType.IgnoreMapping, sourceProperty.Name);
            return;
        }

        string mapping;
        if ((sourceProperty.PropertyType.IsClass || sourceProperty.PropertyType.IsAbstract || sourceProperty.PropertyType.IsInterface) && sourceProperty.PropertyType != typeof(string))
        {
            if (sourceProperty.PropertyType.GetInterface(nameof(IEnumerable)) != null || sourceProperty.PropertyType.GetInterface(nameof(ICollection)) != null)
            {
                var genericSourceType = sourceProperty.PropertyType.GetGenericArguments()[0];
                var genericDestinationType = destinationProperty.PropertyType.GetGenericArguments()[0];
                mapping = $"classInstance.{destinationProperty.Name} = new List<{genericDestinationType.Namespace}.{genericDestinationType.Name}>();\r\n((List<{genericSourceType.Namespace}.{genericSourceType.Name}>)source.{sourceProperty.Name})?.ForEach(x => classInstance.{destinationProperty.Name}.Add({ClassName}.{GenerateMethodName(genericSourceType, genericDestinationType)}(x)));";
            }
            else
            {
                mapping = $"classInstance.{destinationProperty.Name} = source.{sourceProperty.Name} != null ? {methodName}(source.{sourceProperty.Name}) : null;";
            }
        }
        else
        {
            mapping = $"classInstance.{destinationProperty.Name} = source.{sourceProperty.Name};";
        }

        AddMapping(source, destination, methodName, MappingType.Plain, sourceProperty.Name, destinationProperty.Name, code: mapping);
    }

    /// <summary>
    /// Adds a mapping between the source and destination types.
    /// </summary>
    /// <param name="source">The source type.</param>
    /// <param name="destination">The destination type.</param>
    /// <param name="methodName">The method name.</param>
    /// <param name="type">The mapping type.</param>
    /// <param name="sourceMemberName">The source member name.</param>
    /// <param name="destinationMemberName">The destination member name.</param>
    /// <param name="sourceAsString">The source as string.</param>
    /// <param name="code">The mapping code.</param>
    private void AddMapping(Type source, Type destination, string methodName, MappingType type, string sourceMemberName = null, string destinationMemberName = null, string sourceAsString = null, string code = null)
    {
        if (_mappings.ContainsKey(methodName) == false)
        {
            _mappings.Add(methodName, new TypeMapping
            {
                MethodName = methodName,
                SourceType = source,
                TargetType = destination
            });
        }

        _mappings[methodName].Mappings.Add(new TypeMappingEntry
        {
            SourceMember = sourceMemberName,
            DestinationMember = destinationMemberName,
            SourceAsString = sourceAsString,
            Code = code,
            Type = type
        });
    }

    /// <summary>
    /// Generates the code for the profile.
    /// </summary>
    /// <param name="aggressiveOptimization">Specifies whether aggressive optimization should be applied.</param>
    /// <returns>The generated code.</returns>
    private string GenerateCode(bool aggressiveOptimization)
    {
        var sb = new StringBuilder();
        sb.AppendLine("using System;");
        sb.AppendLine("using System.Collections;");
        sb.AppendLine("using System.Collections.Generic;");
        sb.AppendLine("using System.Linq;");
        sb.AppendLine("using System.Linq.Expressions;");
        sb.AppendLine($"using {_sourceType.Namespace};");
        sb.AppendLine($"using {Namespace};");
        sb.AppendLine($"using {nameof(Mappee)};");
        sb.AppendLine();
        if (aggressiveOptimization) sb.AppendLine("using System.Runtime.CompilerServices;");
        sb.AppendLine($"namespace {Namespace}");
        sb.AppendLine();
        sb.AppendLine("{");
        sb.AppendLine($"    public class {ClassName}");
        sb.AppendLine("    {");

        foreach (var mapping in _mappings)
        {
            if (aggressiveOptimization) sb.AppendLine("        [MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]");
            sb.AppendLine($"        public static {mapping.Value.TargetType.Namespace}.{mapping.Value.TargetType.Name} {mapping.Value.MethodName}({mapping.Value.SourceType.Namespace}.{mapping.Value.SourceType.Name} source)");
            sb.AppendLine("        {");
            sb.AppendLine($"            var classInstance = new {mapping.Value.TargetType.Namespace}.{mapping.Value.TargetType.Name}();");

            if (Actions.ContainsKey(nameof(BeforeMap)) && Actions[nameof(BeforeMap)].Count > 0)
            {
                sb.AppendLine($"            Mappee.Configuration.{nameof(Profile)}.{nameof(ExecuteActions)}(\"{nameof(BeforeMap)}\",source, classInstance);");
            }

            foreach (var map in mapping.Value.Mappings)
            {
                if (mapping.Value.Mappings.Any(e => e.SourceMember == map.SourceMember && e.Type == MappingType.IgnoreMapping) == false)
                {
                    if (map.Type == MappingType.Plain)
                    {
                        sb.AppendLine($"            {map.Code}");
                    }
                    else if (map.Type == MappingType.PropertyToProperty)
                    {
                        sb.AppendLine($"            classInstance.{map.DestinationMember} = source.{map.SourceMember};");
                    }
                    else if (map.Type == MappingType.PropertyToConst)
                    {
                        sb.AppendLine($"            classInstance.{map.DestinationMember} = \"{map.SourceAsString}\";");
                    }
                }
                else
                {
                    sb.AppendLine($"            // Ignored mapping for '{map.SourceMember}'");
                }
            }

            if (Actions.ContainsKey(nameof(AfterMap)) && Actions[nameof(AfterMap)].Count > 0)
            {
                sb.AppendLine($"            Mappee.Configuration.{nameof(Profile)}.{nameof(ExecuteActions)}(\"{nameof(AfterMap)}\",source, classInstance);");
            }

            sb.AppendLine("            return classInstance;");
            sb.AppendLine("        }");
            sb.AppendLine();
        }

        sb.AppendLine("    }");
        sb.AppendLine("}");

        return sb.ToString();
    }

    /// <summary>
    /// Compiles the profile and returns the compilation result.
    /// </summary>
    /// <param name="optimizationLevel">The optimization level.</param>
    /// <param name="concurrentBuild">Specifies whether concurrent build should be enabled.</param>
    /// <param name="aggressiveOptimization">Specifies whether aggressive optimization should be applied.</param>
    /// <param name="compilationResult">Specifies whether the compilation result should be returned.</param>
    /// <returns>The compilation result.</returns>
    /// <exception cref="Exception"></exception>
    public CompilationResult Compile(OptimizationLevel optimizationLevel = OptimizationLevel.Release, bool concurrentBuild = true, bool aggressiveOptimization = true, bool compilationResult = false)
    {
        var code = GenerateCode(aggressiveOptimization);

        var syntaxTree = CSharpSyntaxTree.ParseText(code);
        var assemblyName = $"__{_sourceType.Namespace}_{_sourceType.Name}_{_destinationType.Namespace}_{_destinationType.Name}";
        var compilation = CSharpCompilation.Create(assemblyName)
            .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, optimizationLevel: optimizationLevel, concurrentBuild: concurrentBuild))
            .AddReferences(GetReferences())
            .AddReferences(_mappings.Select(e => MetadataReference.CreateFromFile(e.Value.SourceType.Assembly.Location)))
            .AddReferences(_mappings.Select(e => MetadataReference.CreateFromFile(e.Value.TargetType.Assembly.Location)))
            .AddSyntaxTrees(syntaxTree);

        using var ms = new MemoryStream();
        var result = compilation.Emit(ms);
        if (!result.Success)
        {
            var failures = result.Diagnostics.Select(diagnostic => diagnostic.GetMessage()).ToList();
            throw new Exception(string.Join(Environment.NewLine, failures));
        }

        ms.Seek(0, SeekOrigin.Begin);

        var assembly = Assembly.Load(ms.ToArray());
        var type = assembly.GetType($"{Namespace}.{ClassName}");
        if (type == null)
        {
            throw new Exception("Type not found");
        }

        var instance = Activator.CreateInstance(type);

        foreach (var mapping in _mappings)
        {
            InstanceMappingStore.Append(mapping.Value.SourceType, mapping.Value.TargetType, instance, mapping.Value.MethodName);
        }

        if (compilationResult)
        {
            return new CompilationResult
            {
                Assembly = assembly,
                Instance = instance,
                GeneratedCode = code
            };
        }

        return null;
    }

    /// <summary>
    /// Gets the metadata references for the compilation.
    /// </summary>
    /// <returns>The metadata references.</returns>
    private static MetadataReference[] GetReferences() => new MetadataReference[] {
        MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
        MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location),
        MetadataReference.CreateFromFile(typeof(Activator).Assembly.Location),
        MetadataReference.CreateFromFile(typeof(DateTime).Assembly.Location),
        MetadataReference.CreateFromFile(typeof(List<>).Assembly.Location),
        MetadataReference.CreateFromFile(typeof(Profile).Assembly.Location),
        MetadataReference.CreateFromFile(Path.Combine(Path.GetDirectoryName(typeof(object).Assembly.Location) ?? throw new InvalidOperationException(), "mscorlib.dll")),
        MetadataReference.CreateFromFile(Path.Combine(Path.GetDirectoryName(typeof(object).Assembly.Location) ?? throw new InvalidOperationException(), "netstandard.dll")),
        MetadataReference.CreateFromFile(Path.Combine(Path.GetDirectoryName(typeof(object).Assembly.Location) ?? throw new InvalidOperationException(), "System.dll")),
        MetadataReference.CreateFromFile(Path.Combine(Path.GetDirectoryName(typeof(object).Assembly.Location) ?? throw new InvalidOperationException(), "System.Core.dll")),
        MetadataReference.CreateFromFile(Path.Combine(Path.GetDirectoryName(typeof(object).Assembly.Location) ?? throw new InvalidOperationException(), "System.Collections.dll")),
        MetadataReference.CreateFromFile(Path.Combine(Path.GetDirectoryName(typeof(object).Assembly.Location) ?? throw new InvalidOperationException(), "System.Runtime.dll"))
    };
}