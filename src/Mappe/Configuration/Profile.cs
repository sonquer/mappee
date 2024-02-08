using Mappe.Configuration.Interfaces;
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

namespace Mappe.Configuration
{
    internal class GeneratedCodeMapping
    {
        public string MethodName { get; set; }

        public Type SourceType { get; set; }

        public Type TargetType { get; set; }

        public List<string> Mappings { get; set; } = new();
    }

    public class Profile : IProfile
    {
        private Type _sourceType = typeof(object);

        private Type _destinationType = typeof(object);

        private Dictionary<string, GeneratedCodeMapping> _mappings = new();

        private string MethodName => GenerateMethodName(_sourceType, _destinationType);

        private string GenerateMethodName(Type t1, Type t2) => $"__{t1.Namespace}_{t1.Name}_to_{t2.Namespace}_{t2.Name}".Replace(".", "_");

        public void Initialize(Type source, Type target)
        {
            _sourceType = source;
            _destinationType = target;

            CreateMappings(source, target);
        }

        private void CreateMappings(Type src, Type dest)
        {
            var sourceProperties = src.GetProperties();
            var destinationProperties = dest.GetProperties();

            var methodName = GenerateMethodName(src, dest);

            foreach (var sourceProperty in sourceProperties)
            {
                var destinationProperty = destinationProperties.FirstOrDefault(p => p.Name == sourceProperty.Name);
                if (destinationProperty != null)
                {
                    string mapping;
                    if (sourceProperty.PropertyType.IsClass && sourceProperty.PropertyType != typeof(string))
                    {
                        if (sourceProperty.PropertyType.GetInterface(nameof(IEnumerable)) != null)
                        {
                            var genericSourceType = sourceProperty.PropertyType.GetGenericArguments()[0];
                            var genericDestinationType = destinationProperty.PropertyType.GetGenericArguments()[0];
                            mapping = $"source.{sourceProperty.Name}?.ForEach(x => classInstance.{destinationProperty.Name}.Add(__Mappe_Quick_Mapping.{GenerateMethodName(genericSourceType, genericDestinationType)}(x)));";

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
                        _mappings.Add(methodName, new GeneratedCodeMapping
                        {
                            MethodName = methodName,
                            SourceType = src,
                            TargetType = dest
                        });
                    }

                    _mappings[methodName].Mappings.Add(mapping);
                }
            }
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

        private string GenerateCode()
        {
            var sb = new StringBuilder();
            sb.AppendLine("using System;");
            sb.AppendLine("using System.Collections;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using System.Linq;");
            sb.AppendLine("using System.Linq.Expressions;");
            sb.AppendLine($"using {_sourceType.Namespace};");
            sb.AppendLine("using Mappe;");
            sb.AppendLine("using System.Runtime.CompilerServices;");
            sb.AppendLine("namespace Mappe");
            sb.AppendLine("{");
            sb.AppendLine("    public class __Mappe_Quick_Mapping");
            sb.AppendLine("    {");
            foreach (var mapping in _mappings)
            {
                sb.AppendLine("        [MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]");
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

        public void Compile()
        {
            var code = GenerateCode();

            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var assemblyName = $"__{_sourceType.Namespace}_{_sourceType.Name}_to_{_destinationType.Namespace}_{_destinationType.Name}";
            var compilation = CSharpCompilation.Create(assemblyName)
                .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, optimizationLevel: OptimizationLevel.Release, concurrentBuild: true))
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
            var type = assembly.GetType($"Mappe.__Mappe_Quick_Mapping");
            if (type == null)
            {
                throw new Exception("Type not found");
            }

            var instance = Activator.CreateInstance(type);

            foreach (var mapping in _mappings)
            {
                InstanceStore.Store(mapping.Value.SourceType, mapping.Value.TargetType, instance, mapping.Value.MethodName);
            }

            var x = new List<string>();
            x.AsParallel().Select(e => e).ToList();
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
}