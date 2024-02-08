using Mappe.Configuration.Interfaces;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Mappe.Configuration
{
    public class Profile<T1, T2> : IProfile
    {
        private readonly Type _sourceType = typeof(T1);

        private readonly Type _destinationType = typeof(T2);

        private readonly List<string> _mappings = new List<string>();

        public Profile()
        {
            var sourceProperties = _sourceType.GetProperties();
            var destinationProperties = _destinationType.GetProperties();

            foreach (var sourceProperty in sourceProperties)
            {
                var destinationProperty = destinationProperties.FirstOrDefault(p => p.Name == sourceProperty.Name);
                if (destinationProperty != null)
                {
                    var mapping = $"{destinationProperty.Name} = source.{sourceProperty.Name},";
                    if (_mappings.Contains(mapping) == false)
                    {
                        _mappings.Add(mapping);
                    }
                }
            }
        }

        public Profile<T1, T2> MapFrom<TMember>(Expression<Func<T1, TMember>> src, Expression<Func<T2, TMember>> dest)
        {
            var sourceMember = GetMemberInfo(src);
            var destinationMember = GetMemberInfo(dest);

            var sourceMemberName = sourceMember.Name;
            var destinationMemberName = destinationMember.Name;

            var mapping = $"instance.{destinationMemberName} = source.{sourceMemberName};";
            if (_mappings.Contains(mapping) == false)
            {
                _mappings.Add(mapping);
            }

            return this;
        }

        private MemberInfo GetMemberInfo(Expression expression)
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

        private (string className, string code) GenerateCode()
        {
            var className = $"__{_sourceType.Namespace}_{_sourceType.Name}_to_{_destinationType.Namespace}_{_destinationType.Name}";

            var sb = new StringBuilder();
            sb.AppendLine("using System;");
            sb.AppendLine($"using {_sourceType.Namespace};");
            sb.AppendLine("namespace Mappe");
            sb.AppendLine("{");
            sb.AppendLine($"    public class {className} : Mappe.Configuration.Interfaces.IMapper<{_sourceType.Namespace}.{_sourceType.Name}, {_destinationType.Namespace}.{_destinationType.Name}>");
            sb.AppendLine("    {");
            sb.AppendLine($"        public {_destinationType.Namespace}.{_destinationType.Name} Map({_sourceType.Namespace}.{_sourceType.Name} source)");
            sb.AppendLine("        {");
            sb.AppendLine($"            return new {_destinationType.Namespace}.{_destinationType.Name}()");
            sb.AppendLine("            {");
            foreach (var mapping in _mappings)
            {
                sb.AppendLine($"                {mapping}");
            }
            sb.AppendLine("            };");
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");

            return (className, sb.ToString());
        }

        public void Compile()
        {
            var (className, code) = GenerateCode();

            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var assemblyName = $"__{_sourceType.Namespace}_{_sourceType.Name}_to_{_destinationType.Namespace}_{_destinationType.Name}";
            var compilation = CSharpCompilation.Create(assemblyName)
                .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, optimizationLevel: OptimizationLevel.Release))
                .AddReferences(GetReferences())
                .AddSyntaxTrees(syntaxTree);

            using (var ms = new MemoryStream())
            {
                var result = compilation.Emit(ms);
                if (!result.Success)
                {
                    var failures = result.Diagnostics.Select(diagnostic => diagnostic.GetMessage()).ToList();
                    throw new Exception(string.Join(Environment.NewLine, failures));
                }

                ms.Seek(0, SeekOrigin.Begin);

                var assembly = Assembly.Load(ms.ToArray());
                var type = assembly.GetType($"Mappe.{className}");
                if (type == null)
                {
                    throw new Exception("Type not found");
                }

                var instance = Activator.CreateInstance(type);

                InstanceStore.Store(_sourceType, _destinationType, instance);
            }
        }

        public static MetadataReference[] GetReferences() => new MetadataReference[] {
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(T1).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(T2).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(Activator).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(IMapper<,>).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(DateTime).Assembly.Location),
            MetadataReference.CreateFromFile(Path.Combine(Path.GetDirectoryName(typeof(object).Assembly.Location) ?? throw new InvalidOperationException(), "mscorlib.dll")),
            MetadataReference.CreateFromFile(Path.Combine(Path.GetDirectoryName(typeof(object).Assembly.Location) ?? throw new InvalidOperationException(), "netstandard.dll")),
            MetadataReference.CreateFromFile(Path.Combine(Path.GetDirectoryName(typeof(object).Assembly.Location) ?? throw new InvalidOperationException(), "System.dll")),
            MetadataReference.CreateFromFile(Path.Combine(Path.GetDirectoryName(typeof(object).Assembly.Location) ?? throw new InvalidOperationException(), "System.Core.dll")),
            MetadataReference.CreateFromFile(Path.Combine(Path.GetDirectoryName(typeof(object).Assembly.Location) ?? throw new InvalidOperationException(), "System.Collections.dll")),
            MetadataReference.CreateFromFile(Path.Combine(Path.GetDirectoryName(typeof(object).Assembly.Location) ?? throw new InvalidOperationException(), "System.Runtime.dll"))
        };
    }
}