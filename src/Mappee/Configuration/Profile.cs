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

public class Profile : IProfile
{
    private Type _sourceType = typeof(object);

    private Type _destinationType = typeof(object);

    private readonly Dictionary<string, TypeCodeMapping> _mappings = new();

    private const string Namespace = "Mappee";

    private const string ClassName = "__Mappee_Quick_Mapping";

    private string MethodName => GenerateMethodName(_sourceType, _destinationType);

    private static string GenerateMethodName(Type source, Type destination) => $"__{source.Namespace}_{source.Name}_to_{destination.Namespace}_{destination.Name}".Replace(".", "_");

    public Profile Bind<TSource, TDestination>()
    {
        _sourceType = typeof(TSource);
        _destinationType = typeof(TDestination);

        CreateMappings(typeof(TSource), typeof(TDestination));

        return this;
    }

    public Profile ForMember<TSource, TDestination>(Expression<Func<TSource, object>> source, Expression<Func<TDestination, object>> destination)
    {
        var methodName = GenerateMethodName(typeof(TSource), typeof(TDestination));
        var sourceMember = GetMemberInfo(source);
        var destinationMember = GetMemberInfo(destination);

        var mapping = $"classInstance.{destinationMember.Name} = {sourceMember.Name};";
        if (_mappings.ContainsKey(methodName) == false)
        {
            _mappings.Add(methodName, new TypeCodeMapping
            {
                MethodName = methodName,
                SourceType = typeof(TSource),
                TargetType = typeof(TDestination)
            });
        }

        _mappings[methodName].Mappings.Add(mapping);

        return this;
    }

    public Profile ForMember<TSource, TDestination>(string source, Expression<Func<TDestination, object>> destination)
    {
        var methodName = GenerateMethodName(typeof(TSource), typeof(TDestination));
        var destinationMember = GetMemberInfo(destination);

        var mapping = $"classInstance.{destinationMember.Name} = \"{source}\";";
        if (_mappings.ContainsKey(methodName) == false)
        {
            _mappings.Add(methodName, new TypeCodeMapping
            {
                MethodName = methodName,
                SourceType = typeof(TSource),
                TargetType = typeof(TDestination)
            });
        }

        _mappings[methodName].Mappings.Add(mapping);

        return this;
    }

    private static MemberInfo GetMemberInfo(Expression expression)
    {
        if (expression is LambdaExpression lambdaExpression)
        {
            if (lambdaExpression.Body is MemberExpression memberExpression)
            {
                return memberExpression.Member;
            }
        }

        throw new ArgumentException("Invalid expression");
    }

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

    private void CreateMappingForField(Type source, Type destination, PropertyInfo sourceProperty, PropertyInfo destinationProperty)
    {
        var methodName = GenerateMethodName(source, destination);

        string mapping;
        if ((sourceProperty.PropertyType.IsClass || sourceProperty.PropertyType.IsAbstract || sourceProperty.PropertyType.IsInterface) && sourceProperty.PropertyType != typeof(string))
        {
            if (sourceProperty.PropertyType.GetInterface(nameof(IEnumerable)) != null || sourceProperty.PropertyType.GetInterface(nameof(ICollection)) != null)
            {
                var genericSourceType = sourceProperty.PropertyType.GetGenericArguments()[0];
                var genericDestinationType = destinationProperty.PropertyType.GetGenericArguments()[0];
                mapping = $"classInstance.{destinationProperty.Name} = new List<{genericDestinationType.Namespace}.{genericDestinationType.Name}>();\r\n((List<{genericSourceType.Namespace}.{genericSourceType.Name}>)source.{sourceProperty.Name})?.ForEach(x => classInstance.{destinationProperty.Name}.Add(__Mappee_Quick_Mapping.{GenerateMethodName(genericSourceType, genericDestinationType)}(x)));";
            }
            else
            {
                mapping = $"classInstance.{destinationProperty.Name} = source.{sourceProperty.Name} != null ? {MethodName}(source.{sourceProperty.Name}) : null;";
            }
        }
        else
        {
            mapping = $"classInstance.{destinationProperty.Name} = source.{sourceProperty.Name};";
        }

        if (_mappings.ContainsKey(methodName) == false)
        {
            _mappings.Add(methodName, new TypeCodeMapping
            {
                MethodName = methodName,
                SourceType = source,
                TargetType = destination
            });
        }

        _mappings[methodName].Mappings.Add(mapping);
    }

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
        if (aggressiveOptimization) sb.AppendLine("using System.Runtime.CompilerServices;");
        sb.AppendLine($"namespace {Namespace}");
        sb.AppendLine("{");
        sb.AppendLine($"    public class {ClassName}");
        sb.AppendLine("    {");

        foreach (var mapping in _mappings)
        {
            if (aggressiveOptimization) sb.AppendLine("        [MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]");
            sb.AppendLine($"        public static {mapping.Value.TargetType.Namespace}.{mapping.Value.TargetType.Name} {mapping.Value.MethodName}({mapping.Value.SourceType.Namespace}.{mapping.Value.SourceType.Name} source)");
            sb.AppendLine("        {");
            sb.AppendLine($"            var classInstance = new {mapping.Value.TargetType.Namespace}.{mapping.Value.TargetType.Name}();");
            foreach (var map in mapping.Value.Mappings)
            {
                sb.AppendLine($"            {map}");
            }
            sb.AppendLine("            return classInstance;");
            sb.AppendLine("        }");
            sb.AppendLine();
        }

        sb.AppendLine("    }");
        sb.AppendLine("}");

        return sb.ToString();
    }

    public CompilationResult Compile(OptimizationLevel optimizationLevel = OptimizationLevel.Release, bool concurrentBuild = true, bool aggressiveOptimization = true, bool compilationResult = false)
    {
        var code = GenerateCode(aggressiveOptimization);

        var syntaxTree = CSharpSyntaxTree.ParseText(code);
        var assemblyName = $"__{_sourceType.Namespace}_{_sourceType.Name}_to_{_destinationType.Namespace}_{_destinationType.Name}";
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
            InstanceStore.Store(mapping.Value.SourceType, mapping.Value.TargetType, instance, mapping.Value.MethodName);
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

    public static MetadataReference[] GetReferences() => new MetadataReference[] {
        MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
        MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location),
        MetadataReference.CreateFromFile(typeof(Activator).Assembly.Location),
        MetadataReference.CreateFromFile(typeof(DateTime).Assembly.Location),
        MetadataReference.CreateFromFile(typeof(List<>).Assembly.Location),
        MetadataReference.CreateFromFile(Path.Combine(Path.GetDirectoryName(typeof(object).Assembly.Location) ?? throw new InvalidOperationException(), "mscorlib.dll")),
        MetadataReference.CreateFromFile(Path.Combine(Path.GetDirectoryName(typeof(object).Assembly.Location) ?? throw new InvalidOperationException(), "netstandard.dll")),
        MetadataReference.CreateFromFile(Path.Combine(Path.GetDirectoryName(typeof(object).Assembly.Location) ?? throw new InvalidOperationException(), "System.dll")),
        MetadataReference.CreateFromFile(Path.Combine(Path.GetDirectoryName(typeof(object).Assembly.Location) ?? throw new InvalidOperationException(), "System.Core.dll")),
        MetadataReference.CreateFromFile(Path.Combine(Path.GetDirectoryName(typeof(object).Assembly.Location) ?? throw new InvalidOperationException(), "System.Collections.dll")),
        MetadataReference.CreateFromFile(Path.Combine(Path.GetDirectoryName(typeof(object).Assembly.Location) ?? throw new InvalidOperationException(), "System.Runtime.dll"))
    };
}