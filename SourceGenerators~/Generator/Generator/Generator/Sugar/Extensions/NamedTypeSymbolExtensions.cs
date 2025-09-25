using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using SourceGenerators.Sugar.Common;
using SourceGenerators.Sugar.Pools;

namespace SourceGenerators.Sugar.Extensions;

public static class NamedTypeSymbolExtensions
{
    public static string AccessibilityAsString(this INamedTypeSymbol typeSymbol)
    {
        return typeSymbol.DeclaredAccessibility switch
        {
            Accessibility.Public => "public",
            Accessibility.Internal => "internal",
            Accessibility.Protected => "protected",
            Accessibility.Private => "private",
            _ => "public"
        };
    }   
    
    public static string AsString(this Accessibility accessibility)
    {
        return accessibility switch
        {
            Accessibility.Public => "public",
            Accessibility.Internal => "internal",
            Accessibility.Protected => "protected",
            Accessibility.Private => "private",
            _ => "public"
        };
    }
    
    /// <summary>
    /// Получить полное имя типа: Пространство имён + имя
    /// </summary>
    /// <param name="typeSymbol"></param>
    /// <returns></returns>
    public static string GetFullName(this ITypeSymbol typeSymbol)
    {
        var builder = StringBuilderPool.Get()
            .Append(typeSymbol.ContainingNamespace.ToDisplayString())
            .Append('.')
            .Append(typeSymbol.Name);

        return builder.ToStringAndReturn();
    }

    /// <summary>
    /// Получить глобальное имя: global::Пространство имён + имя
    /// </summary>
    /// <param name="typeSymbol"></param>
    /// <returns></returns>
    public static string GetGlobal(this ITypeSymbol typeSymbol)
    {
        var builder = StringBuilderPool.Get()
            .Append(typeSymbol.ContainingNamespace.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat))
            .Append('.')
            .Append(typeSymbol.Name);

        return StringBuilderPool.ToStringAndReturn(builder);
    }

    /// <summary>
    /// Получить все поля
    /// </summary>
    /// <param name="typeSymbol"></param>
    /// <returns></returns>
    public static IEnumerable<IFieldSymbol> GetAllFields(this INamedTypeSymbol typeSymbol)
    {
        return typeSymbol
            .GetMembers()
            .OfType<IFieldSymbol>();
    }
}