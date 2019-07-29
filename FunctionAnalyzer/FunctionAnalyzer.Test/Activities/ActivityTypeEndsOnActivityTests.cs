using FunctionAnalyzer.Activities;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using TestHelper;
using Xunit;

namespace FunctionAnalyzer.Test.Activities
{
    public class ActivityTypeEndsOnActivityTests : DiagnosticVerifier
    {

        [Fact]
        public void EmptyCode_ShouldNotResultInFindings()
        {
            const string test = @"";

            VerifyCSharpDiagnostic(test);
        }

        [Fact]
        public void NonActivityType_ShouldNotBeFlagged()
        {
            // Arrange
            const string test = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace MyFunctionApp.Functions
    {
        class MyFunction
        {   
        }
    }";

            // Act & assert
            VerifyCSharpDiagnostic(test);
        }

        //Diagnostic and CodeFix both triggered and checked for
        [Fact]
        public void ActivityTypes_ShouldEndOnActivity()
        {
            const string test = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace MyFunctionApp.Activities
    {
        class MyFunction
        {   
        }
    }";
            var expected = new DiagnosticResult
            {
                Id = ActivityTypeEndsOnActivity.DiagnosticId,
                Message = "Type name 'MyFunction' should end on 'Activity'.",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 11, 15)
                        }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer() =>
            new ActivityTypeEndsOnActivity();
    }
}
