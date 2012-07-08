using System.ComponentModel.Composition;
using System.Linq;
using Mono.Cecil;


[Export, PartCreationPolicy(CreationPolicy.Shared)]
public class FileChangedChecker
{
    Logger logger;
    ModuleReader moduleReader;

    [ImportingConstructor]
    public FileChangedChecker(Logger logger, ModuleReader moduleReader)
    {
        this.logger = logger;
        this.moduleReader = moduleReader;
    }

    public bool ShouldStart()
    {
        if (moduleReader.Module.Types.Any(x => x.Name == "ProcessedByCostura"))
        {
            logger.LogMessage("\tDid not process because file has already been processed");
            return false;
        }
        moduleReader.Module.Types.Add(new TypeDefinition(null, "ProcessedByCostura", TypeAttributes.NotPublic | TypeAttributes.Abstract | TypeAttributes.Interface));
        return true;
    }
}