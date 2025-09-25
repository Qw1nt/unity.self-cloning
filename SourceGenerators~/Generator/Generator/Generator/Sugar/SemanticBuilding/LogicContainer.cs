using System;
using Generator.Sugar.Base;
using Microsoft.CodeAnalysis;
using SourceGenerators.Sugar.Common;
using SourceGenerators.Sugar.Extensions;
using SourceGenerators.Sugar.Interfaces;
using SourceGenerators.Sugar.Pools;

namespace SourceGenerators.Sugar.SemanticBuilding;

public class LogicContainer : SemanticStructBuilderBase, IAccessibility
{
    public string Modifier;
    public string Name;
    public MethodContainerType Type;

    public Accessibility Accessibility { get; set; }

    protected override void ReturnToPool()
    {
        base.ReturnToPool();
        SemanticBuilderPool.Return(this);
    }

    protected override void BuildStruct(SemanticBuildingContext builder, ref int indentLevel)
    {
        indentLevel++;
        
        BuildAttribute(builder, ref indentLevel);

        builder.Indent(indentLevel)
            .AppendSpaceEnd(Modifier)
            .AppendSpaceEnd(GetTypeName())
            .Append(Name)
            .Push();

        builder.Indent(indentLevel)
            .Append('{')
            .Push();

        BuildChild(builder, ref indentLevel);

        builder.Indent(indentLevel)
            .Append('}')
            .Push();

        indentLevel--;
    }

    private string GetTypeName()
    {
        switch (Type)
        {
            case MethodContainerType.Class:
                return "class";

            case MethodContainerType.Struct:
                return "struct";

            default:
                throw new ArgumentOutOfRangeException();
        }

        return string.Empty;
    }

    public override int GetHashCode()
    {
        return ContextId.GetHashCode();
    }
}