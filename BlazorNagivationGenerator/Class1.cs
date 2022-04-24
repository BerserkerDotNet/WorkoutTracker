using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace BlazorNagivationGenerator
{
    [Generator]
    public class BlazorNagivationGenerator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            if (!Debugger.IsAttached) 
            {
                // Debugger.Launch();
            }

            context.Compilation.f

            var compilation = context.Compilation;
            var routes = compilation.SyntaxTrees
                .SelectMany(s => s.GetRoot().DescendantNodes())
                .Where(n => n.IsKind(SyntaxKind.ClassDeclaration))
                .OfType<ClassDeclarationSyntax>()
                .Select(c => GetPages(compilation, c))
                .Where(c => c is object)
                .Cast<RouteDefinition>()
                .ToArray();

            var sourceCode = new StringBuilder();
            sourceCode.AppendLine("public static class NavigationManagerExtensions");
            sourceCode.AppendLine("{");

            foreach (var route in routes)
            {
                var arguments = string.Join(",", route.Arguments.Select(a => $"{a.Type.FullName} {a.DisplayName}"));
                var argumentsParams = string.Join(",", route.Arguments.Select(a => a.DisplayName));
                sourceCode.AppendLine($"public static void NavigateTo{route.DisplayName}(this Microsoft.AspNetCore.Components.NavigationManager navigationManager, {arguments})");
                sourceCode.AppendLine("{");
                sourceCode.AppendLine($"var url = string.Format(\"{route.Path}\", argumentsParams);");
                sourceCode.AppendLine("navigationManager.NavigateTo(url);");
                sourceCode.AppendLine("}");
            }   

            sourceCode.AppendLine("}");

            context.AddSource("BlazorNagivationHelper.cs", SourceText.From(sourceCode.ToString(), Encoding.UTF8));
        }

		private RouteDefinition GetPages(Compilation compilation, ClassDeclarationSyntax @class)
		{
            var attributes = @class.AttributeLists
                .SelectMany(a => a.Attributes)
                .Where(attr => attr.Name.ToString() == "Microsoft.AspNetCore.Components.RouteAttribute")
                .ToArray();

            if (!attributes.Any()) 
            {
                return null;
            }

            var routeAttribute = attributes.First();
            var routeArgs = routeAttribute.ArgumentList?.Arguments;
            if (routeArgs is null || routeArgs.Value.Count == 0) 
            {
                return null;
            }

            var semanticModel = compilation.GetSemanticModel(@class.SyntaxTree);
            var routePathArg = routeArgs.Value[0];
            var routePathTemplate = semanticModel.GetConstantValue(routePathArg.Expression).ToString();

            var parts = routePathTemplate.Split(new[] { '/' });
            var args = parts.Where(p => p.StartsWith("{") && p.EndsWith("}")).Select(p => ParseArgument(p));

            var idx = 0;
            var template = Regex.Replace(routePathTemplate, "{[^}]+}", (idx++).ToString());

            return new RouteDefinition
            {
                Path = template,
                DisplayName = @class.Identifier.ValueText,
                Arguments = args
            };
		}

        private RouteArgumentDefinition ParseArgument(string template)
        {
            var sections = template.Split(':');
            var argumentType = sections.Length > 1 ? ParseArgumentType(sections[1]) : typeof(string);

            return new RouteArgumentDefinition
            {
                PathName = sections[0],
                DisplayName = sections[1],
                Type = argumentType
            };
        }

        private Type ParseArgumentType(string constraint) 
        {
            switch (constraint)
            {
                case "bool":
                    return typeof(bool);
                case "datetime":
                    return typeof(DateTime);
                case "decimal":
                    return typeof(decimal);
                case "double":
                    return typeof(double);
                case "float":
                    return typeof(float);
                case "guid":
                    return typeof(Guid);
                case "int":
                    return typeof(int);
                case "long":
                    return typeof(long);
                default:
                    return typeof(string);
            }
        }

		public void Initialize(GeneratorInitializationContext context)
        {
            // No initialization required for this one
        }
    }

    public class RouteDefinition
    {
		public string DisplayName { get; set; }

		public string Path { get; set; }

		public IEnumerable<RouteArgumentDefinition> Arguments { get; set; }
	}

    public class RouteArgumentDefinition 
    {
		public int Index { get; set; }

		public string DisplayName { get; set; }

		public string PathName { get; set; }

        public Type Type { get; set; } 
	}
}
