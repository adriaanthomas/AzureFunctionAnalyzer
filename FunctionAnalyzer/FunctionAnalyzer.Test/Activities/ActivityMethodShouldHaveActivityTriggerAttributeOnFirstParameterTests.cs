using FunctionAnalyzer.Activities;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using TestHelper;
using Xunit;

namespace FunctionAnalyzer.Test.Activities
{
    public class ActivityMethodShouldHaveActivityTriggerAttributeOnFirstParameterTests : DiagnosticVerifier
    {
        [Fact]
        public void EmptyCode_ShouldHaveNoFindings()
        {
            // Arrange
            const string test = @"";

            // Act & assert
            VerifyCSharpDiagnostic(test);
        }

        [Fact]
        public void CorrectCode_ShouldNotTrigger()
        {
            // Arrange
            const string test = @"
using Microsoft.Azure.WebJobs;

namespace TestFunctions.Activities
{
    public class TestActivity
    {
        [FunctionName]
        public void Run([ActivityTrigger] DurableActivityContextBase context)
        {
        }
    }
}
";
            
            // Act & assert
            VerifyCSharpDiagnostic(test);
        }

        [Fact]
        public void MethodWithArgumentButNoAttribute_ShouldTrigger()
        {
            // Arrange
            const string test = @"
using Microsoft.Azure.WebJobs;

namespace TestFunctions.Activities
{
    public class TestActivity
    {
        [FunctionName]
        public void Run(DurableActivityContextBase context)
        {
        }
    }
}
";
            var expected = new DiagnosticResult
            {
                Id = ActivityMethodShouldHaveActivityTriggerAttributeOnFirstParameter.DiagnosticId,
                Message = "The method 'Run' should have an 'ActivityTriggerAttribute' on its first parameter.",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[] {
                        new DiagnosticResultLocation("Test0.cs", 9, 25)
                    }
            };

            // Act & assert
            VerifyCSharpDiagnostic(test, expected);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer() =>
            new ActivityMethodShouldHaveActivityTriggerAttributeOnFirstParameter();
    }
}