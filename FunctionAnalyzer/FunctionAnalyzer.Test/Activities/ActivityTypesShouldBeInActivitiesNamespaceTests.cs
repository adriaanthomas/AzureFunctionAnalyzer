using FunctionAnalyzer.Activities;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using TestHelper;
using Xunit;

namespace FunctionAnalyzer.Test.Activities
{
    public class ActivityTypesShouldBeInActivitiesNamespaceTests : DiagnosticVerifier
    {
        [Fact]
        public void EmptyCode_ShouldNotResultInFindings()
        {
            const string test = @"";

            VerifyCSharpDiagnostic(test);
        }

        [Fact]
        public void ActivityTypeInActivitiesNamespace_ShouldNotFlag()
        {
            // Arrange
            const string test = @"
namespace MyFunction.Activities
{
    public class MyActivity
    {
    }
}
";

            // Act & assert
            VerifyCSharpDiagnostic(test);
        }

        [Fact]
        public void ActivityTypeNotInActivitiesNamespace_ShouldBeFlagged()
        {
            // Arrange
            const string test = @"
namespace MyFunction.Activity
{
    public class MyActivity
    {
    }
}
";
            var expected = new DiagnosticResult
            {
                Id = ActivityTypesShouldBeInActivitiesNamespace.DiagnosticId,
                Message = "Type name 'MyActivity' should be in a namespace 'Activities'.",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[] {
                        new DiagnosticResultLocation("Test0.cs", 4, 18)
                    }
            };
            
            // Act & assert
            VerifyCSharpDiagnostic(test, expected);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer() =>
            new ActivityTypesShouldBeInActivitiesNamespace();
    }
}