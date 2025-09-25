using Generator.Utils;
using Microsoft.CodeAnalysis;

namespace Generator.Generators;

[Generator]
public class CloneSelfGeneratorPipeline : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var values = context.SyntaxProvider.CollectCloneInfos();

        context.RegisterSourceOutput(values, static (sourceProductionContext, info) =>
            CloneSelfSourceGenerator.Generate(sourceProductionContext, info)
        );
    }
}