using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SourceGenerators.Sugar.Interfaces;

public interface ITypeDeclarationRule
{
    bool IsValid(TypeDeclarationSyntax declaration, GeneratorSyntaxContext context);
}