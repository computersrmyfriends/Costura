using System.ComponentModel.Composition;
using System.IO;



[Export, PartCreationPolicy(CreationPolicy.Shared)]
public class CosturaFileExporter
{
    ResourceExporter resourceExporter;

    public CosturaFileExporter()
    {
    }

    [ImportingConstructor]
    public CosturaFileExporter(ResourceExporter resourceExporter)
    {
        this.resourceExporter = resourceExporter;
    }

    public virtual bool ExportTask(string directory)
    {
        return resourceExporter.Export("Costura.dll", new FileInfo(Path.Combine(directory, "Costura.dll")));
    }

    public virtual bool ExportTask(DirectoryInfo directory)
    {
        return resourceExporter.Export("Costura.dll", new FileInfo(Path.Combine(directory.FullName, "Costura.dll")));
    }

}