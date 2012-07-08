using System;
using System.Linq;
using System.Xml.Linq;
using Microsoft.Build.Framework;


public class ProjectReader
{
    string projectFile;
    public string ToolsDirectory;
    public string TargetPath;
    public MessageImportance? MessageImportance;
    public bool? Overwrite;
    public bool? IncludeDebugSymbols;
    public bool? DeleteReferences;

    public ProjectReader(string projectFile)
    {
        this.projectFile = projectFile;
        SetWeavingProps();
        ToolsDirectory = GetToolsDirectory(projectFile);
    }

    public static string GetToolsDirectory(string projectFile)
    {
        var xDocument = XDocument.Load(projectFile);
        var elements =
            from el in xDocument.BuildDescendants("UsingTask")
            where (string) el.Attribute("TaskName") == "Costura.EmbedTask"
            select el.Attribute("AssemblyFile");
        var firstOrDefault = elements.FirstOrDefault();
        if (firstOrDefault != null)
        {
            var value = firstOrDefault.Value;
            return value.Substring(0, value.IndexOf("Costura.dll", StringComparison.OrdinalIgnoreCase));
        }
        return null;
    }

    void SetWeavingProps()
    {
        var xDocument = ReadXDocument();
        var children =
            from target in xDocument.BuildDescendants("Target")
            let targetName = (string) target.Attribute("Name")
            where string.Equals(targetName, "AfterBuild", StringComparison.InvariantCultureIgnoreCase)

            from embedTask in target.BuildDescendants("Costura.EmbedTask")
            select new
                       {
                           TargetPath = (string) embedTask.Attribute("TargetPath"),
                           MessageImportance = ConvertToEnum((string) embedTask.Attribute("MessageImportance")),
                           Overwrite = ToBool(embedTask.Attribute("Overwrite")),
                           DeleteReferences = ToBool(embedTask.Attribute("DeleteReferences")),
                           IncludeDebugSymbols = ToBool(embedTask.Attribute("IncludeDebugSymbols")),
                       };

        var first = children.FirstOrDefault();
        if (first == null)
        {
            return;
        }
        TargetPath = first.TargetPath;
        MessageImportance = first.MessageImportance;
        Overwrite = first.Overwrite;
        IncludeDebugSymbols = first.IncludeDebugSymbols;
        DeleteReferences = first.DeleteReferences;
    }

    XDocument ReadXDocument()
    {
        try
        {
            return XDocument.Load(projectFile);
        }
        catch (Exception exception)
        {
            throw new Exception(string.Format("Could not load project file '{0}'.", projectFile),exception);
        }
    }

    public static bool? ToBool(XAttribute attribute)
    {
        if (attribute == null)
        {
            return null;
        }
        return bool.Parse(attribute.Value);
    }

    static MessageImportance? ConvertToEnum(string messageImportance)
    {
        if (!string.IsNullOrWhiteSpace(messageImportance))
        {
            MessageImportance messageImportanceEnum;
            if (Enum.TryParse(messageImportance, out messageImportanceEnum))
            {
                return messageImportanceEnum;
            }
        }
        return null;
    }
}