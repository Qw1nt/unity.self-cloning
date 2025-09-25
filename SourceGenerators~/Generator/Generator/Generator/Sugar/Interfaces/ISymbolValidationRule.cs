using Microsoft.CodeAnalysis;

namespace SourceGenerators.Sugar.Interfaces;

public interface ISymbolValidationRule
{
    public abstract bool IsValid(ISymbol declarationSymbol, INamedTypeSymbol type, GeneratorSyntaxContext context);
}