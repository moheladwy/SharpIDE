namespace SharpIDE.Application.Features.Analysis.Razor;

public record struct SharpIdeRazorClassifiedSpan(SharpIdeRazorSourceSpan Span, SharpIdeRazorSpanKind Kind, string? CodeClassificationType = null, string? VsSemanticRangeType = null);

public enum SharpIdeRazorSpanKind
{
	Transition,
	MetaCode,
	Comment,
	Code,
	Markup,
	None,
}
