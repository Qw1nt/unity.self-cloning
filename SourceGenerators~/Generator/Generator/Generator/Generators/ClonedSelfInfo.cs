using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Generator.Generators;

public struct ClonedSelfInfo
{
    public string TypeName;
    public INamedTypeSymbol NamedTypeSymbol;
    public ImmutableArray<ISymbol> FieldsAndProperties;
}