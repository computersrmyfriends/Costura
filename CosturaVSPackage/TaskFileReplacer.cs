using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Threading;

[Export, PartCreationPolicy(CreationPolicy.Shared)]
public class TaskFileReplacer
{
    ErrorDisplayer errorDisplayer;
    CosturaFileExporter fileExporter;
    public string taskFilePath;

    [ImportingConstructor]
    public TaskFileReplacer(ErrorDisplayer errorDisplayer, CosturaFileExporter fileExporter)
    {
        this.errorDisplayer = errorDisplayer;
        this.fileExporter = fileExporter;
        Directory.CreateDirectory(Environment.ExpandEnvironmentVariables(@"%appdata%\Costura"));
        taskFilePath = Environment.ExpandEnvironmentVariables(@"%appdata%\Costura\TaskAssembliesToUpdate.txt");
        if (!File.Exists(taskFilePath))
        {
            using (File.Create(taskFilePath))
            {
            }
        }
    }

    public void ClearFile()
    {
        File.Delete(taskFilePath);
        using (File.Create(taskFilePath))
        {
        }
    }

    public void CheckForFilesToUpdate()
    {
        WrapInMutex(() =>
                        {
                            var newStrings = new List<string>();
                            foreach (var targetDirectory in File.ReadAllLines(taskFilePath))
                            {
                                var trimmed = targetDirectory.Trim();
                                if (trimmed.Length == 0)
                                {
                                    continue;
                                }
                                var directoryInfo = new DirectoryInfo(trimmed);
                                if (!directoryInfo.Exists)
                                {
                                    continue;
                                }
                                if (fileExporter.ExportTask(directoryInfo))
                                {
                                    var path = Path.Combine(trimmed, "Costura.dll");
                                    errorDisplayer.ShowInfo(string.Format("Costura: Updated '{0}' to version {1}.", path, CurrentVersion.Version));
                                }
                                else
                                {
                                    newStrings.Add(trimmed);
                                }
                            }
                            File.WriteAllLines(taskFilePath, newStrings);
                        });
    }

    public void AddFile(DirectoryInfo directoryInfo)
    {
        WrapInMutex(() =>
                        {
                            var allText = File.ReadAllLines(taskFilePath);
                            var fileContainsDirectory = allText.Any(x => string.Equals(x, directoryInfo.FullName, StringComparison.InvariantCultureIgnoreCase));
                            if (!fileContainsDirectory)
                            {
                                errorDisplayer.ShowInfo(string.Format("Costura: Restart of Visual Studio required to update '{0}'.", Path.Combine(directoryInfo.FullName, "Costura.dll")));
                                File.AppendAllText(taskFilePath, directoryInfo.FullName + "\r\n");
                            }
                        });
    }

    static void WrapInMutex(Action action)
    {
        ThreadPool.QueueUserWorkItem(x =>
                                         {
                                             bool createdNew;
                                             using (new Mutex(true, typeof (TaskFileReplacer).FullName, out createdNew))
                                             {
                                                 if (!createdNew)
                                                 {
                                                     //already being used;
                                                     return;
                                                 }
                                                 action();
                                             }
                                         });
    }
}