using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Linq;

namespace FunctionAnalyzer.Core
{
    internal static class AttributeChecks
    {
        public static bool MethodHasFunctionNameAttribute(SyntaxNodeAnalysisContext context,
            MethodDeclarationSyntax method) =>
                AListContainsAttribute(context, method.AttributeLists, TypeNames.FunctionNameAttribute);

        public static bool MethodParameterHasActivityTriggerAttribute(SyntaxNodeAnalysisContext context,
            ParameterSyntax parameter) =>
                MethodParameterHasAttribute(context, parameter, TypeNames.ActivityTriggerAttribute);

        private static bool MethodParameterHasAttribute(SyntaxNodeAnalysisContext context, ParameterSyntax parameter,
            string attributeName) =>
                AListContainsAttribute(context, parameter.AttributeLists, attributeName);

        private static bool AListContainsAttribute(SyntaxNodeAnalysisContext context,
            SyntaxList<AttributeListSyntax> attributeLists, string attributeTypeName)
        {
            var attributeType = GetAttributeType(context, attributeTypeName);
            
            return attributeLists.Any(l => l.Attributes.Any(a => GetAttributeType(context, a).Equals(attributeType)));
        }

        private static ITypeSymbol GetAttributeType(SyntaxNodeAnalysisContext context,
            AttributeSyntax attributeSyntax) => context.SemanticModel.GetTypeInfo(attributeSyntax).Type;

        private static INamedTypeSymbol GetAttributeType(SyntaxNodeAnalysisContext context, string attributeTypeName) =>
            context.Compilation.GetTypeByMetadataName(attributeTypeName);
    }
}