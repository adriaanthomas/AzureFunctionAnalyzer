using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using static FunctionAnalyzer.Core.AttributeChecks;

namespace FunctionAnalyzer.Activities
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ActivityMethodShouldHaveActivityTriggerAttributeOnFirstParameter : DiagnosticAnalyzer
    {
        public const string DiagnosticId = nameof(ActivityMethodShouldHaveActivityTriggerAttributeOnFirstParameter);

        // You can change these strings in the Resources.resx file. If you do not want your analyzer to be localize-able, you can use regular strings for Title and MessageFormat.
        // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/Localizing%20Analyzers.md for more on localization
        private static readonly LocalizableString Title = new LocalizableResourceString(
            nameof(Resources.ActivityMethodShouldHaveActivityTriggerAttributeOnFirstParameterTitle), Resources.ResourceManager, typeof(Resources));

        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(
            nameof(Resources.ActivityMethodShouldHaveActivityTriggerAttributeOnFirstParameterMessageFormat), Resources.ResourceManager, typeof(Resources));

        private static readonly LocalizableString Description = new LocalizableResourceString(
            nameof(Resources.ActivityMethodShouldHaveActivityTriggerAttributeOnFirstParameterDescription), Resources.ResourceManager, typeof(Resources));
        
        private const string Category = DiagnosticCategory.Design;

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat,
            Category, DiagnosticSeverity.Warning, true, Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);
        
        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.ReportDiagnostics);
            
            context.RegisterSyntaxNodeAction(AnalyzeMethod, SyntaxKind.MethodDeclaration);
        }

        private static void AnalyzeMethod(SyntaxNodeAnalysisContext context)
        {
            var method = (MethodDeclarationSyntax) context.Node;
            if (MethodHasFunctionNameAttribute(context, method))
            {
                var firstParam = method.ParameterList.Parameters.FirstOrDefault();
                if (!MethodParameterHasActivityTriggerAttribute(context, firstParam))
                {
                    var diagnostic = Diagnostic.Create(Rule, firstParam.GetLocation(), method.Identifier.ValueText);
                
                    context.ReportDiagnostic(diagnostic);
                }
            }
        }
    }
}