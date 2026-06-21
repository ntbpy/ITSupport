using MIT.Modules.Diagnostics.Contracts.Dtos;
using MIT.Modules.Diagnostics.Infrastructure;

namespace Diagnostics.Tests.Infrastructure;

public sealed class DiagnosticResultParserTests
{
    [Fact]
    public void Parse_ValidJson_ReturnsDiagnosticResult()
    {
        const string json = """
        {
          "severity": "high",
          "issues": [{"title":"High CPU","description":"CPU > 90%","impact":"Slow performance","category":"performance"}],
          "fixes": [{"title":"Kill process","description":"...","steps":["Open Task Manager","Kill Chrome"],"auto_fixable":false,"priority":"immediate"}],
          "summary": "Máy đang dùng CPU rất cao"
        }
        """;

        var result = DiagnosticResultParser.Parse(json);

        result.Severity.ShouldBe(DiagnosticSeverity.High);
        result.Issues.Count.ShouldBe(1);
        result.Issues[0].Title.ShouldBe("High CPU");
        result.Fixes[0].AutoFixable.ShouldBeFalse();
        result.Summary.ShouldBe("Máy đang dùng CPU rất cao");
    }

    [Fact]
    public void Parse_MalformedJson_ThrowsDiagnosticParseException()
    {
        var act = () => DiagnosticResultParser.Parse("not-json");
        act.ShouldThrow<DiagnosticParseException>();
    }

    [Fact]
    public void Parse_CriticalSeverity_MapCorrectly()
    {
        const string json = """
        {
          "severity": "critical",
          "issues": [],
          "fixes": [],
          "summary": "Hệ thống nguy hiểm"
        }
        """;

        var result = DiagnosticResultParser.Parse(json);
        result.Severity.ShouldBe(DiagnosticSeverity.Critical);
    }

    [Fact]
    public void Parse_AutoFixableWithCommand_MapsFixCommand()
    {
        const string json = """
        {
          "severity": "medium",
          "issues": [],
          "fixes": [{"title":"Clear Temp","description":"...","steps":[],"auto_fixable":true,"fix_command":"clear temp","priority":"scheduled"}],
          "summary": "Cần dọn dẹp"
        }
        """;

        var result = DiagnosticResultParser.Parse(json);
        result.Fixes[0].AutoFixable.ShouldBeTrue();
        result.Fixes[0].FixCommand.ShouldBe("clear temp");
    }
}
