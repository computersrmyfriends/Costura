using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics;
using System.IO;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Costura
{
    public class EmbedTask : Task
    {
        public bool Overwrite { set; get; }
        public bool IncludeDebugSymbols { set; get; }
        public bool DeleteReferences { set; get; }
        public bool CreateTemporaryAssemblies { get; set; }

        public string TargetPath { set; get; }
        public string MessageImportance { set; get; }
        public string References { get; set; }
        public Exception Exception { get; set; }
        public string KeyFilePath { get; set; }
        //Hack:
        public List<string> ReferenceCopyLocalPaths { get; set; }
        Logger logger;
        static Version version;

        static EmbedTask()
        {
            version = typeof(EmbedTask).Assembly.GetName().Version;
        }

        public EmbedTask()
        {
            MessageImportance = "Low";
            Overwrite = true;
            DeleteReferences = true;
            IncludeDebugSymbols = true;
        }

        public override bool Execute()
        {
            var message = string.Format("Costura.EmbedTask v{0} Executing (Change MessageImportance to get more or less info)", version);
            var buildMessageEventArgs = new BuildMessageEventArgs(message, "", "EmbedTask", Microsoft.Build.Framework.MessageImportance.High);
            BuildEngine.LogMessageEvent(buildMessageEventArgs);

            var stopwatch = Stopwatch.StartNew();

            try
            {
                logger = new Logger
                {
                    BuildEngine = BuildEngine,
                };
                logger.Initialise(MessageImportance);
                Inner();
            }
            catch (Exception exception)
            {
                HandleException(exception);
            }
            finally
            {
                stopwatch.Stop();
                logger.Flush();
                BuildEngine.LogMessageEvent(new BuildMessageEventArgs(string.Format("\tFinished ({0}ms)", stopwatch.ElapsedMilliseconds), "", "Costura.EmbedTask", Microsoft.Build.Framework.MessageImportance.High));
            }
            return !logger.ErrorHasBeenRaised;
        }

        void HandleException(Exception exception)
        {
            Exception = exception;
            if (exception is WeavingException)
            {
                logger.LogError(exception.Message);
                return;
            }

            logger.LogError(string.Format("Unhandled exception occurred {0}", exception));
        }


        void Inner()
        {
            using (var catalog = new AssemblyCatalog(GetType().Assembly))
            using (var container = new CompositionContainer(catalog))
            {
                container.ComposeExportedValue(this);
                container.ComposeExportedValue(BuildEngine);
                container.ComposeExportedValue(logger);
                CheckForInvalidConfig();
                container.GetExportedValue<TargetPathFinder>().Execute();

                logger.LogMessage(string.Format("\tTargetPath: {0}", TargetPath));


                container.GetExportedValue<AssemblyResolver>().Execute();
                var moduleReader = container.GetExportedValue<ModuleReader>();
                moduleReader.Execute();

                var fileChangedChecker = container.GetExportedValue<FileChangedChecker>();
                if (!fileChangedChecker.ShouldStart())
                {
                    return;
                }

                container.GetExportedValue<MsCoreReferenceFinder>().Execute();

                container.GetExportedValue<AssemblyLoaderImporter>().Execute();
                container.GetExportedValue<ModuleLoaderImporter>().Execute();
                container.GetExportedValue<DependencyFinder>().Execute();
                container.GetExportedValue<ProjectKeyReader>().Execute();
                container.GetExportedValue<ResourceCaseFixer>().Execute();
                using (var resourceEmbedder = container.GetExportedValue<ResourceEmbedder>())
                {
                    resourceEmbedder.Execute();
                    var savePath = GetSavePath();
                    container.GetExportedValue<ModuleWriter>().Execute(savePath);
                }
                container.GetExportedValue<ReferenceDeleter>().Execute();
            }
        }

        void CheckForInvalidConfig()
        {
            if (!Overwrite && DeleteReferences)
            {
                throw new WeavingException("Overwrite=false and DeleteReferences=true is invalid because if the new file is copied to a different directory it serves no purpose deleting references.");
            }
        }

        string GetSavePath()
        {
            var fileInfo = new FileInfo(TargetPath);
            var directoryPath = Path.Combine(fileInfo.DirectoryName, "CosturaMerged");
            //Try to delete directory for cleanup purposes. 
            try
            {
                if (Directory.Exists(directoryPath))
                {
                    Directory.Delete(directoryPath, true);
                }
            }
            catch (Exception)
            {
            }
            if (Overwrite)
            {
                return TargetPath;
            }
            Directory.CreateDirectory(directoryPath);

            return Path.Combine(directoryPath , fileInfo.Name);
        }
    }

}