using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Generator.Common;
using SourceGenerators.Sugar.Pools;

namespace SourceGenerators.Sugar.Common;

[StructLayout(LayoutKind.Auto)]
public struct AttributeInfo
{
    public string Namespace;
    public string AttributeName;
    public string? Args;

    public static AttributeInfo Serializable()
    {
        return new AttributeInfo
        {
            Namespace = "System",
            AttributeName = nameof(Serializable),
        };
    }

    public static AttributeInfo StructLayout(LayoutKind kind)
    {
        var kindName = kind switch
        {
            LayoutKind.Auto => "Auto",
            LayoutKind.Explicit => "Explicit",
            LayoutKind.Sequential => "Sequential",
            _ => string.Empty
        };

        return new AttributeInfo
        {
            Namespace = typeof(StructLayoutAttribute).Namespace!,
            AttributeName = nameof(StructLayout),
            Args = StringBuilderPool.Get().Append(nameof(LayoutKind)).Append('.').Append(kindName).ToStringAndReturn()
        };
    }

    public static AttributeInfo MethodImpl(MethodImplOptions options)
    {
        var displayOptions = options switch
        {
            MethodImplOptions.AggressiveInlining => "AggressiveInlining",
            MethodImplOptions.ForwardRef => "ForwardRef",
            MethodImplOptions.InternalCall => "InternalCall",
            MethodImplOptions.NoInlining => "NoInlining",
            MethodImplOptions.NoOptimization => "NoOptimization",
            MethodImplOptions.PreserveSig => "PreserveSig",
            MethodImplOptions.Synchronized => "Synchronized",
            MethodImplOptions.Unmanaged => "Unmanaged",
            _ => throw new ArgumentOutOfRangeException(nameof(options), options, null)
        };

        return new AttributeInfo
        {
            Namespace = typeof(MethodImplAttribute).Namespace!,
            AttributeName = nameof(MethodImpl),
            
            Args = StringBuilderPool.Get()
                .Append(nameof(MethodImplOptions))
                .Append('.')
                .Append(displayOptions)
                .ToStringAndReturn()
        };
    }

    public static AttributeInfo Il2CppAttribute(Il2CppOption option)
    {
        const string attributesNamespace = "Unity.IL2CPP.CompilerServices";

        var displayOptions = option switch
        {
            Il2CppOption.NullChecks => "NullChecks",
            Il2CppOption.ArrayBoundsChecks => "ArrayBoundsChecks",
            Il2CppOption.DivideByZeroChecks => "DivideByZeroChecks",
            _ => throw new ArgumentOutOfRangeException(nameof(option), option, null)
        };

        return new AttributeInfo
        {
            Namespace = attributesNamespace,
            AttributeName = "Il2CppSetOption",
            Args = StringBuilderPool.Get()
                .Append(attributesNamespace).Append('.').Append("Option").Append('.').Append(displayOptions)
                .Append(", ")
                .Append("false")
                .ToStringAndReturn()
        };
    }
}