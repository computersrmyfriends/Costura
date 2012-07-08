using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using Costura;

[Export, PartCreationPolicy(CreationPolicy.Shared)]
public class ReferenceDeleter
{
    DependencyFinder dependencyFinder;
    EmbedTask embedTask;
    Logger logger;

    [ImportingConstructor]
    public ReferenceDeleter(DependencyFinder dependencyFinder, EmbedTask embedTask, Logger logger)
    {
        this.dependencyFinder = dependencyFinder;
        this.embedTask = embedTask;
        this.logger = logger;
    }

    public void Execute()
    {
        if (!embedTask.DeleteReferences)
        {
            return;
        }
        foreach (var fileToDelete in GetFileToDelete())
        {
            try
            {
                logger.LogMessage(string.Format("\tDeleting '{0}'", fileToDelete));
                File.Delete(fileToDelete);
            }
            catch (Exception exception)
            {
                logger.LogWarning(string.Format("\tTried to delete '{0}' but could not due to the following exception: {1}", fileToDelete, exception));
            }
        }
    }

    IEnumerable<string> GetFileToDelete()
    {
        var directoryName = Path.GetDirectoryName(embedTask.TargetPath);
        foreach (var dependency in dependencyFinder.Dependencies)
        {
            var dependencyBinary = Path.Combine(directoryName, Path.GetFileName(dependency));
            if (File.Exists(dependencyBinary))
            {
                yield return dependencyBinary;
            }

            var xmlFile = Path.ChangeExtension(dependencyBinary, "xml");
            if (File.Exists(xmlFile))
            {
                yield return xmlFile;
            }

            var pdbFile = Path.ChangeExtension(dependencyBinary, "pdb");
            if (File.Exists(pdbFile))
            {
                yield return pdbFile;
            }
        }
    }
}