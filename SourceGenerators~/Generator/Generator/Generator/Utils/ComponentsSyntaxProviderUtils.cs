using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Generator.Generators;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Generator.Utils;

public static class ComponentsSyntaxProviderUtils
{
    public static IncrementalValuesProvider<ClonedSelfInfo> CollectCloneInfos(this SyntaxValueProvider provider)
    {
        return provider.ForAttributeWithMetadataName(Constants.Attributes.SelfCloneableAttribute,
                predicate: static (syntaxNode, _) => syntaxNode is StructDeclarationSyntax or ClassDeclarationSyntax,
                transform: static (syntaxContext, cancellationToken) => TryExtractInfo(syntaxContext, cancellationToken)
            )
            .Where(static x => x.HasValue == true)
            .Select(static (candidate, _) => candidate!.Value);
    }

    private static ClonedSelfInfo? TryExtractInfo(GeneratorAttributeSyntaxContext context, CancellationToken ct)
    {
        if (context.TargetSymbol is INamedTypeSymbol namedTypeSymbol == false)
            return null;

        return new ClonedSelfInfo
        {
            TypeName = namedTypeSymbol.Name,
            NamedTypeSymbol = namedTypeSymbol,
            
            FieldsAndProperties = namedTypeSymbol
                .GetMembers()
                .Where(x => x.Kind is SymbolKind.Field or SymbolKind.Property && x.IsStatic == false)
                .ToImmutableArray()
        };
    }
}