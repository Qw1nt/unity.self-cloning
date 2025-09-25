using System.Runtime.CompilerServices;
using System.Text;
using SourceGenerators.Sugar.Common;

namespace SourceGenerators.Sugar.Extensions;

public static class StringBuilderExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void AppendIndent(this StringBuilderContentInfo info)
    {
        info.Source.Append('\t', info.IndentLevel);
    } 
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static StringBuilderContentInfo Indent(this StringBuilderContentInfo info)
    {
        info.AppendIndent();
        return info;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static StringBuilderContentInfo Indent(this SemanticBuildingContext builder, in int indent)
    {
        var info = new StringBuilderContentInfo
        {
            IndentLevel = indent,
            Source = builder
        };

        info.AppendIndent();
        return info;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static StringBuilderContentInfo Indent(this StringBuilder builder, in int indent)
    {
        var info = new StringBuilderContentInfo
        {
            IndentLevel = indent,
            Source = builder
        };

        info.AppendIndent();
        return info;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static StringBuilderContentInfo NewLine(this StringBuilderContentInfo info, int? indent = null)
    {
        if (indent != null)
            info.IndentLevel = indent.Value;

        info.Append('\n');
        info.AppendIndent();

        return info;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static StringBuilderContentInfo Append(this StringBuilderContentInfo info, char value)
    {
        info.Source.Append(value);
        return info;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static StringBuilderContentInfo Append(this StringBuilderContentInfo info, string value)
    {
        info.Source.Append(value);
        return info;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static StringBuilderContentInfo AppendLine(this StringBuilderContentInfo info, string value)
    {
        info.Source.AppendLine(value);
        return info;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static StringBuilderContentInfo AppendSpaceEnd(this StringBuilderContentInfo info, char value)
    {
        info.Append(value);
        info.Source.Append(' ');

        return info;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static StringBuilderContentInfo AppendSpaceEnd(this StringBuilderContentInfo info, string value)
    {
        info.Append(value);
        info.Source.Append(' ');

        return info;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static StringBuilderContentInfo AppendSpaceEnd(this StringBuilderContentInfo info, string value, bool ignore)
    {
        if (ignore == true)
            return info;

        info.Append(value);
        info.Source.Append(' ');

        return info;
    }

    public static StringBuilderContentInfo AppendBlock(this StringBuilderContentInfo info, string? content,
        int? indent = null)
    {
        if (content == null)
            return info;

        foreach (var line in content.TrimEnd().Split('\n'))
        {
            var indentLevel = indent ?? info.IndentLevel;

            info.Source.Indent(indentLevel)
                .Append(line)
                .Push();
        }

        return info;
    }

    public static void Push(this StringBuilderContentInfo info)
    {
        info.Source.Append('\n');
    }
}