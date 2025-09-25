using System;
using System.Collections.Concurrent;
using Microsoft.CodeAnalysis;
using SourceGenerators.Sugar.Common;
using SourceGenerators.Sugar.SemanticBuilding;

namespace SourceGenerators.Sugar.Pools;

public static class SemanticBuilderPool
{
    private const int MaxSize = 32;

    private static readonly ConcurrentStack<NamespaceBuilder> Namespaces = new();
    private static readonly ConcurrentStack<LogicContainer> Containers = new();
    private static readonly ConcurrentStack<FieldBuilder> Fields = new();
    private static readonly ConcurrentStack<FieldAccessor> FieldAccessors = new();
    private static readonly ConcurrentStack<MethodStructBuilder> Methods = new();

    public static NamespaceBuilder GetNamespace()
    {
        return Namespaces.TryPop(out var result) ? result : new NamespaceBuilder();
    }

    public static LogicContainer GetContainer()
    {
        return Containers.TryPop(out var result) ? result : new LogicContainer();
    }

    public static FieldBuilder GetFieldBuilder()
    {
        return Fields.TryPop(out var result) ? result : new FieldBuilder();
    }

    public static FieldAccessor GetFieldAccessor()
    {
        return FieldAccessors.TryPop(out var result) ? result : new FieldAccessor();
    }

    public static MethodStructBuilder GetMethod()
    {
        return Methods.TryPop(out var result) ? result : new MethodStructBuilder();
    }

    public static void Return(NamespaceBuilder value)
    {
        if (Namespaces.Count >= MaxSize)
            return;

        value.Namespace = string.Empty;
        value.UsingsCount = 0;

        Namespaces.Push(value);
    }

    public static void Return(LogicContainer container)
    {
        if (Containers.Count >= MaxSize)
            return;

        container.Name = null!;
        container.Modifier = null!;
        container.Accessibility = Accessibility.Friend;
        container.ContextId = Guid.Empty;

        Containers.Push(container);
    }

    public static void Return(FieldBuilder value)
    {
        if (value.Get != null)
            Return(value.Get);

        if (value.Set != null)
            Return(value.Set);
        
        value.Get = null;
        value.Set = null;

        if (Fields.Count >= MaxSize)
            return;

        value.Value = string.Empty;
        value.ValueType = string.Empty;
        value.ContextId = Guid.Empty;
        
        Fields.Push(value);
    }

    public static void Return(FieldAccessor value)
    {
        if (FieldAccessors.Count >= MaxSize)
            return;

        value.Type = AccessorType.Get;
        value.Accessibility = Accessibility.Friend;
        value.ContextId = Guid.Empty;

        FieldAccessors.Push(value);
    }

    public static void Return(MethodStructBuilder method)
    {
        if (Methods.Count >= MaxSize)
            return;

        method.Modifier = null!;
        method.Name = null!;
        method.Args = null!;
        method.Body = null!;
        method.Type = null!;
        method.HasBody = true;    
        
        Methods.Push(method);
    }
}