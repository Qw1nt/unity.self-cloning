using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SourceGenerators.Sugar.SyntaxWalkers;

public class MethodInvocationCollector : CSharpSyntaxWalker
{
    private readonly List<InvocationExpressionSyntax> _buffer;

    public MethodInvocationCollector(List<InvocationExpressionSyntax> buffer)
    {
        _buffer = buffer;
    }

    public override void VisitInvocationExpression(InvocationExpressionSyntax node)
    {
        _buffer.Add(node);
        base.VisitInvocationExpression(node);
    }
}