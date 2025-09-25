using Generator.Sugar.Base;
using SourceGenerators.Sugar.Common;
using SourceGenerators.Sugar.Extensions;
using SourceGenerators.Sugar.Pools;

namespace SourceGenerators.Sugar.SemanticBuilding;

public class NamespaceBuilder : SemanticStructBuilderBase
{
    public string Namespace;
    public int UsingsCount;

    protected override void ReturnToPool()
    {
        base.ReturnToPool();
        SemanticBuilderPool.Return(this);
    }

    protected override void BuildStruct(SemanticBuildingContext builder, ref int indentLevel)
    {
        builder
            .Indent(indentLevel)
            .Append(UsingsCount == 0 ? string.Empty : "\n")
            .Append("namespace ")
            .Append(Namespace)
            .Push();

        builder.Indent(indentLevel)
            .Append('{')
            .Push();

        BuildChild(builder, ref indentLevel);
        
        builder.Indent(indentLevel)
            .Append('}')
            .Push();
    }
}