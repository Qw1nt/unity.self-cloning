using System.Text;

namespace SourceGenerators.Sugar.Common;

public struct SemanticBuildingContext
{
    public readonly StringBuilder Builder;

    public SemanticBuildingContext(StringBuilder builder)
    {
        Builder = builder;
    }
    
    public static implicit operator StringBuilder(SemanticBuildingContext context)
    {
        return context.Builder;
    }
}