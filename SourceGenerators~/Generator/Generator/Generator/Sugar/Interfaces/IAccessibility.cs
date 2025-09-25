using Microsoft.CodeAnalysis;

namespace SourceGenerators.Sugar.Interfaces;

public interface IAccessibility
{
    public Accessibility Accessibility { get; set; }
}