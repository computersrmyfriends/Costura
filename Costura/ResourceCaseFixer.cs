using System;
using System.ComponentModel.Composition;
using Costura;

[Export, PartCreationPolicy(CreationPolicy.Shared)]
public class ResourceCaseFixer
{
    ModuleReader moduleReader;

    [ImportingConstructor]
    public ResourceCaseFixer(DependencyFinder dependencyFinder, ModuleReader moduleReader, EmbedTask embedTask, Logger logger)
    {
        this.moduleReader = moduleReader;
    }

    public void Execute()
    {
        foreach (var resource in moduleReader.Module.Resources)
        {
            if (resource.Name.StartsWith("costura.", StringComparison.InvariantCultureIgnoreCase))
            {
                resource.Name = resource.Name.ToLowerInvariant();
            }
        }
    }
}