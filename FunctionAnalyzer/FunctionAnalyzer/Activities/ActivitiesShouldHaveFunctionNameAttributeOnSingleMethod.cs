using Microsoft.Azure.WebJobs;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Linq;

namespace FunctionAnalyzer.Activities
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ActivitiesShouldHaveFunctionNameAttributeOnSingleMethod : DiagnosticAnalyzer
    {
        public const string DiagnosticId = nameof(ActivitiesShouldHaveFunctionNameAttributeOnSingleMethod);

        // You can change these strings in the Resources.resx file. If you do not want your analyzer to be localize-able, you can use regular strings for Title and MessageFormat.
        // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/Localizing%20Analyzers.md for more on localization
        private static readonly LocalizableString Title = new LocalizableResourceString(
            nameof(Resources.ActivitiesShouldHaveFunctionNameAttributeOnSingleMethodTitle), Resources.ResourceManager, typeof(Resources));

        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(
            nameof(Resources.ActivitiesShouldHaveFunctionNameAttributeOnSingleMethodMessageFormat), Resources.ResourceManager, typeof(Resources));

        private static readonly LocalizableString Description = new LocalizableResourceString(
            nameof(Resources.ActivitiesShouldHaveFunctionNameAttributeOnSingleMethodDescription), Resources.ResourceManager, typeof(Resources));
        
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
            context.RegisterSyntaxNodeAction(AnalyzeClass, SyntaxKind.ClassDeclaration);
        }

        private static void AnalyzeClass(SyntaxNodeAnalysisContext context)
        {
            var clazz = (ClassDeclarationSyntax) context.Node;
            var members = clazz.Members;
            if (!members.Any() || members.OfType<MethodDeclarationSyntax>().Count(m => MethodHasFunctionNameAttribute(context, m)) != 1)
            {
                var diagnostic = Diagnostic.Create(Rule, clazz.Identifier.GetLocation(), clazz.Identifier.ValueText);
                
                context.ReportDiagnostic(diagnostic);
            }
        }

        private static bool MethodHasFunctionNameAttribute(SyntaxNodeAnalysisContext context, MethodDeclarationSyntax method) =>
            method.AttributeLists.Any(l => l.Attributes.Any(a =>
                GetAttributeTypeFullName(context, a) == typeof(FunctionNameAttribute).FullName));

        private static string GetAttributeTypeFullName(SyntaxNodeAnalysisContext context, AttributeSyntax attribute) =>
            context.SemanticModel.GetTypeInfo(attribute).Type.ToDisplayString();
    }
}
