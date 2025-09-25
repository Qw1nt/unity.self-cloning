using Generator.Sugar.Base;
using SourceGenerators.Sugar.Common;
using SourceGenerators.Sugar.Extensions;
using SourceGenerators.Sugar.Pools;

namespace SourceGenerators.Sugar.SemanticBuilding;

public class MethodStructBuilder : SemanticStructBuilderBase
{
    public string Modifier;
    public string Type;
    public string Name;
    public string Args;
    public string Body;
    public bool HasBody = true;

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
            .AppendSpaceEnd(Type, string.IsNullOrEmpty(Type))
            .Append(Name)
            .Append('(').Append(Args).Append(')').Append(HasBody == true ? ' ' : ';')
            .Push();

        if (HasBody == true)
        {
            builder.Indent(indentLevel)
                .Append('{')
                .Push();

            indentLevel++;

            builder.Indent(0)
                .AppendBlock(Body, indentLevel);

            indentLevel--;

            builder.Indent(indentLevel)
                .Append('}')
                .Push();
        }

        indentLevel--;
    }
    
    public override int GetHashCode()
    {
        return ContextId.GetHashCode();
    }
}