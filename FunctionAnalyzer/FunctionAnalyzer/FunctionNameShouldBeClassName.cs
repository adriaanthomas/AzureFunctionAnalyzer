using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace FunctionAnalyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class FunctionNameShouldBeClassName : DiagnosticAnalyzer
    {
        public const string DiagnosticId = nameof(FunctionNameShouldBeClassName);

        // You can change these strings in the Resources.resx file. If you do not want your analyzer to be localize-able, you can use regular strings for Title and MessageFormat.
        // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/Localizing%20Analyzers.md for more on localization
        private static readonly LocalizableString Title = new LocalizableResourceString(
            nameof(Resources.FunctionNameShouldBeClassNameTitle), Resources.ResourceManager, typeof(Resources));

        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(
            nameof(Resources.FunctionNameShouldBeClassNameMessageFormat), Resources.ResourceManager, typeof(Resources));

        private static readonly LocalizableString Description = new LocalizableResourceString(
            nameof(Resources.FunctionNameShouldBeClassNameDescription), Resources.ResourceManager, typeof(Resources));
        
        private const string Category = DiagnosticCategory.Design;

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat,
            Category, DiagnosticSeverity.Warning, true, Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.ReportDiagnostics);

            // TODO: Consider registering other actions that act on syntax instead of or in addition to symbols
            // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/Analyzer%20Actions%20Semantics.md for more information
            context.RegisterSyntaxNodeAction(AnalyzeAttribute, SyntaxKind.Attribute);
        }

        private static void AnalyzeAttribute(SyntaxNodeAnalysisContext context)
        {
            var attribute = (AttributeSyntax) context.Node;
            var className = FindClass(attribute).Identifier.ValueText;

            if (!FunctionNameAttributeHasCorrectName(context, attribute, className))
            {
                var diagnostic = Diagnostic.Create(Rule, attribute.GetLocation(), className);
                
                context.ReportDiagnostic(diagnostic);
            }
        }

        private static ClassDeclarationSyntax FindClass(SyntaxNode attribute)
        {
            for (var node = attribute; node != null; node = node.Parent)
            {
                if (node is ClassDeclarationSyntax clazz)
                    return clazz;
            }

            return null;
        }
        
        private static bool FunctionNameAttributeHasCorrectName(SyntaxNodeAnalysisContext context,
            AttributeSyntax attribute, string expectedName)
        {
            if (attribute.ArgumentList?.Arguments == null)
                return false;
            
            foreach (var argument in attribute.ArgumentList.Arguments)
            {
//                var argumentName = argument.NameEquals.Name.Identifier.ValueText;
                var expression = argument.Expression;
                var actualName = GetAttributeArgumentValue(context, expression);

                return actualName == expectedName;
            }

            return false;
        }

        private static string GetAttributeArgumentValue(SyntaxNodeAnalysisContext context, SyntaxNode expression) =>
            context.SemanticModel.GetConstantValue(expression).Value.ToString();
    }
}