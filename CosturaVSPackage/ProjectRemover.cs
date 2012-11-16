using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;

public class ProjectRemover
{
    XDocument xDocument;

    public ProjectRemover(string projectFile)
    {
        new FileInfo(projectFile).IsReadOnly = false;
        xDocument = XDocument.Load(projectFile);
        RemoveUsingTask();
        RemoveEmbedTask();
        RemovePostBuild();
        xDocument.Save(projectFile);
    }

    void RemoveEmbedTask()
    {
        xDocument.BuildDescendants("Target")
            .Where(x => string.Equals((string) x.Attribute("Name"), "AfterBuild", StringComparison.InvariantCultureIgnoreCase))
            .Descendants(XDocumentExtensions.BuildNamespace + "Costura.EmbedTask").Remove();
    }
    void RemovePostBuild()
    {
        xDocument.BuildDescendants("Target")
            .Where(x => string.Equals((string)x.Attribute("Name"), "AfterBuild", StringComparison.InvariantCultureIgnoreCase))
            .Descendants(XDocumentExtensions.BuildNamespace + "Exec").Remove();
    }


    void RemoveUsingTask()
    {
        xDocument.BuildDescendants("UsingTask")
            .Where(x => (string) x.Attribute("TaskName") == "Costura.EmbedTask").Remove();
    }

}