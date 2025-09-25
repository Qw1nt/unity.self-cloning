using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SourceGenerators.Sugar.SyntaxWalkers;

namespace SourceGenerators.Sugar.Extensions;

public static class MethodSymbolExtensions
{
    public static void GetMethodCalls(this IMethodSymbol methodSymbol, List<InvocationExpressionSyntax> output)
    {
        output.Clear();
        
        var methodSyntax = methodSymbol.DeclaringSyntaxReferences
            .FirstOrDefault()?.GetSyntax() as MethodDeclarationSyntax;

        if (methodSyntax == null || methodSyntax.Body == null)
            return;

        var collector = new MethodInvocationCollector(output);
        collector.Visit(methodSyntax.Body);
    }
    
    public static void GetObjectCreations(this IMethodSymbol methodSymbol, List<ObjectCreationExpressionSyntax> output)
    {
        output.Clear();
        
        var methodSyntax = methodSymbol.DeclaringSyntaxReferences
            .FirstOrDefault()?.GetSyntax() as MethodDeclarationSyntax;

        if (methodSyntax == null || methodSyntax.Body == null)
            return;

        var collector = new ObjectCreationCollector(output);
        collector.Visit(methodSyntax.Body);
    }
}