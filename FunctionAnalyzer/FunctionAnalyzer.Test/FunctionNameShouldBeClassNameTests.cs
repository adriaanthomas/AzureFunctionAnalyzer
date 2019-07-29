using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using TestHelper;
using Xunit;

namespace FunctionAnalyzer.Test
{
    public class FunctionNameShouldBeClassNameTests : DiagnosticVerifier
    {
        [Fact]
        public void EmptyCode_ShouldNotFlagAnything()
        {
            // Arrange
            const string test = @"";
            
            // Act & assert
            VerifyCSharpDiagnostic(test);
        }

        [Fact]
        public void FunctionNameAttributeWithCorrectName_ShouldNotFire()
        {
            // Arrange
            const string test = @"
using Microsoft.Azure.WebJobs;

namespace TestFunctions.Activities
{
    public class TestActivity
    {
        [FunctionName(""TestActivity"")]
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
        public void FunctionNameAttributeWithConcatenatedName_ShouldNotFire()
        {
            // Arrange
            const string test = @"
using Microsoft.Azure.WebJobs;

namespace TestFunctions.Activities
{
    public class TestActivity
    {
        [FunctionName(""Test"" + ""Activity"")]
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
        public void FunctionNameAttributeWithNameOfClass_ShouldNotFire()
        {
            // Arrange
            const string test = @"
using Microsoft.Azure.WebJobs;

namespace TestFunctions.Activities
{
    public class TestActivity
    {
        [FunctionName(nameof(TestActivity))]
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
        public void FunctionNameAttributeWithNameFromConstant_ShouldNotFire()
        {
            // Arrange
            const string test = @"
using Microsoft.Azure.WebJobs;

namespace TestFunctions.Activities
{
    public class TestActivity
    {
        [FunctionName(FunctionNames.Name)]
        public void Run()
        {
        }

        private static class FunctionNames
        {
            public const string Name = ""TestActivity"";
        }
    }
}
";
            
            // Act & assert
            VerifyCSharpDiagnostic(test);
        }

        [Fact]
        public void FunctionNameAttributeWithoutValue_ShouldFire()
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

            var expected = new DiagnosticResult
            {
                Id = FunctionNameShouldBeClassName.DiagnosticId,
                Message = "The 'FunctionNameAttribute' should have a name equal to the class name ('TestActivity').",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[] {
                        new DiagnosticResultLocation("Test0.cs", 8, 10)
                    }
            };
            
            // Act & assert
            VerifyCSharpDiagnostic(test, expected);
        }

        [Fact]
        public void FunctionNameAttributeWithIncorrectValue_ShouldFire()
        {
            // Arrange
            const string test = @"
using Microsoft.Azure.WebJobs;

namespace TestFunctions.Activities
{
    public class MyActivity
    {
        [FunctionName(""IncorrectName"")]
        public void Run()
        {
        }
    }
}
";

            var expected = new DiagnosticResult
            {
                Id = FunctionNameShouldBeClassName.DiagnosticId,
                Message = "The 'FunctionNameAttribute' should have a name equal to the class name ('MyActivity').",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[] {
                        new DiagnosticResultLocation("Test0.cs", 8, 10)
                    }
            };
            
            // Act & assert
            VerifyCSharpDiagnostic(test, expected);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new FunctionNameShouldBeClassName();
        }
    }
}