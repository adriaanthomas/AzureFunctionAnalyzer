using FunctionAnalyzer.Activities;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using TestHelper;
using Xunit;

namespace FunctionAnalyzer.Test.Activities
{
    public class ActivitiesShouldHaveFunctionNameAttributeOnSingleMethodTests : DiagnosticVerifier
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
        public void ActivityWithoutAnyMethods_ShouldBeFlagged()
        {
            // Arrange
            const string test = @"
namespace TestFunctions.Activities
{
    public class TestActivity
    {
    }
}
";
            var expected = new DiagnosticResult
            {
                Id = ActivitiesShouldHaveFunctionNameAttributeOnSingleMethod.DiagnosticId,
                Message = "Type name 'TestActivity' should a single method marked with '[FunctionName]'.",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[] {
                        new DiagnosticResultLocation("Test0.cs", 4, 18)
                    }
            };
            
            // Act & assert
            VerifyCSharpDiagnostic(test, expected);
        }

        [Fact]
        public void ActivityWithMethodButNoFunctionNameAttribute_ShouldBeFlagged()
        {
            // Arrange
            const string test = @"
namespace TestFunctions.Activities
{
    public class TestActivity
    {
        public void Run()
        {
        }
    }
}
";
            var expected = new DiagnosticResult
            {
                Id = ActivitiesShouldHaveFunctionNameAttributeOnSingleMethod.DiagnosticId,
                Message = "Type name 'TestActivity' should a single method marked with '[FunctionName]'.",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[] {
                        new DiagnosticResultLocation("Test0.cs", 4, 18)
                    }
            };
            
            // Act & assert
            VerifyCSharpDiagnostic(test, expected);
        }

        [Fact]
        public void CorrectActivity_ShouldNotBeFlagged()
        {
            // Arrange
            const string test = @"
using Microsoft.Azure.WebJobs;

namespace TestFunctions.Activities
{
    public class TestActivity
    {
        [FunctionName]
        public void Run()
        {
        }
    }
}
";
            
            // Act & assert
            VerifyCSharpDiagnostic(test);
        }

        [Fact]
        public void NonMethodMembers_ShouldNotInterfere()
        {
            // Arrange
            const string test = @"
using Microsoft.Azure.WebJobs;

namespace TestFunctions.Activities
{
    public class TestActivity
    {
        private const string Name = ""Testing"";

        [FunctionName]
        public void Run()
        {
        }
    }
}
";
            
            // Act & assert
            VerifyCSharpDiagnostic(test);
        }

        [Fact]
        public void TwoMethodsWithAttribute_ShouldFire()
        {
            // Arrange
            const string test = @"
using Microsoft.Azure.WebJobs;

namespace MySuperCoolLibrary.Activities
{
    public class MyActivity
    {
        [FunctionName]
        public void Run1()
        {
        }

        [FunctionName]
        public void Run2()
        {
        }
    }
}
";
            var expected = new DiagnosticResult
            {
                Id = ActivitiesShouldHaveFunctionNameAttributeOnSingleMethod.DiagnosticId,
                Message = "Type name 'MyActivity' should a single method marked with '[FunctionName]'.",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[] {
                        new DiagnosticResultLocation("Test0.cs", 6, 18)
                    }
            };
            
            // Act & assert
            VerifyCSharpDiagnostic(test, expected);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer() =>
            new ActivitiesShouldHaveFunctionNameAttributeOnSingleMethod();
    }
}