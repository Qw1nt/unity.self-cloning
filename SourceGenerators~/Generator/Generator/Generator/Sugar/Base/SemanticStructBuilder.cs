using System;
using System.Collections.Generic;
using Generator.Sugar.Pools;
using SourceGenerators.Sugar.Common;
using SourceGenerators.Sugar.Extensions;
using SourceGenerators.Sugar.Interfaces;

namespace Generator.Sugar.Base;

public abstract class SemanticStructBuilderBase : ISemanticStructBuilder
{
    private static readonly List<AttributeInfo> EmptyAttributes = new(Array.Empty<AttributeInfo>());

    private List<AttributeInfo>? _attributes = null!;
    private List<ISemanticStructBuilder>? _children = null!;

    private int _spacesCount = 0;

    public Guid ContextId { get; set; }

    protected virtual void ReturnToPool()
    {
        _spacesCount = 0;

        if (_children != null)
            CollectionPool.Return(_children);

        if (_attributes != null)
            CollectionPool.Return(_attributes);

        _children = null;
        _attributes = null;
    }

    protected abstract void BuildStruct(SemanticBuildingContext builder, ref int indentLevel);

    public IReadOnlyList<AttributeInfo> GetAttributes()
    {
        if (_attributes == null)
            return EmptyAttributes;

        return _attributes;
    }

    public IReadOnlyList<ISemanticStructBuilder>? GetChild()
    {
        return _children;
    }

    public int GetSpacesCount()
    {
        return _spacesCount;
    }

    public void AddAttribute(AttributeInfo attributeInfoData)
    {
        if (_attributes == null)
            _attributes = CollectionPool.GetAttributes();

        _attributes.Add(attributeInfoData);
    }

    public void AddSpace(int count = 1)
    {
        _spacesCount += count;
    }

    public void AddChild(ISemanticStructBuilder builder)
    {
        if (_children == null)
            _children = CollectionPool.GetListSemanticStructBuilder();

        _children.Add(builder);
    }

    public void Build(SemanticBuildingContext builder, ref int indentLevel)
    {
        BuildSpaces(builder, ref indentLevel);
        BuildStruct(builder, ref indentLevel);

        ReturnToPool();
    }

    private void BuildSpaces(SemanticBuildingContext buildingContext, ref int indentLevel)
    {
        for (int i = 0; i < _spacesCount; i++)
        {
            buildingContext
                .Indent(0)
                .Push();
        }
    }

    protected void BuildChild(SemanticBuildingContext builder, ref int indentLevel)
    {
        if (_children == null)
            return;

        foreach (var item in _children)
            item.Build(builder, ref indentLevel);
    }

    protected void BuildAttribute(SemanticBuildingContext builder, ref int indentLevel)
    {
        if (_attributes == null)
            return;

        var info = builder.Indent(indentLevel);

        for (int i = 0; i < _attributes.Count; i++)
        {
            var attribute = _attributes[i];

            info.Append('[');
            info.Append(attribute.AttributeName);

            if (string.IsNullOrEmpty(attribute.Args) == false)
            {
                info.Append('(');
                info.Append(attribute.Args!);
                info.Append(')');
            }

            info.Append(']');

            if (i < _attributes.Count - 1)
                info.NewLine();
        }

        info.Push();
    }
}