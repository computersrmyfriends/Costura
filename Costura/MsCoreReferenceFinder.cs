using System.ComponentModel.Composition;
using System.Linq;
using Mono.Cecil;

[Export, PartCreationPolicy(CreationPolicy.Shared)]
public class MsCoreReferenceFinder
{
    ModuleReader moduleReader;
    IAssemblyResolver assemblyResolver;
    public TypeReference VoidTypeReference;

    [ImportingConstructor]
    public MsCoreReferenceFinder(ModuleReader moduleReader, IAssemblyResolver assemblyResolver)
    {
        this.moduleReader = moduleReader;
        this.assemblyResolver = assemblyResolver;
    }


    public void Execute()
    {
        var msCoreLibDefinition = assemblyResolver.Resolve("mscorlib");
        var msCoreTypes = msCoreLibDefinition.MainModule.Types;

        var objectDefinition = msCoreTypes.FirstOrDefault(x => x.Name == "Object");
        if (objectDefinition == null)
        {
            ExecuteWinRT();
            return;
        }
        var module = moduleReader.Module;

        var voidDefinition = msCoreTypes.First(x => x.Name == "Void");
        VoidTypeReference = module.Import(voidDefinition);


    }

    public void ExecuteWinRT()
    {
        var systemRuntime = assemblyResolver.Resolve("System.Runtime");
        var systemRuntimeTypes = systemRuntime.MainModule.Types;
        var voidDefinition = systemRuntimeTypes.First(x => x.Name == "Void");
        VoidTypeReference = moduleReader.Module.Import(voidDefinition);
    }


}