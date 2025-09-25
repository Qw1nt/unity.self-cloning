using Microsoft.CodeAnalysis;

namespace SourceGenerators.Sugar.Extensions;

public static class TypesExtensions
{
    public static string AsString(this TypeKind typeKind)
    {
        return typeKind switch
        {
            TypeKind.Class => "class",
            TypeKind.Enum => "enum",
            TypeKind.Interface => "interface",
            TypeKind.Struct => "struct",
            _ => ""
        };
    }
}