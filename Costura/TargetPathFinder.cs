using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using Costura;

[Export, PartCreationPolicy(CreationPolicy.Shared)]
public class TargetPathFinder
{
    EmbedTask weavingTask;
    BuildEnginePropertyExtractor buildEnginePropertyExtractor;
    Logger logger;

    [ImportingConstructor]
    public TargetPathFinder(EmbedTask weavingTask, BuildEnginePropertyExtractor buildEnginePropertyExtractor, Logger logger)
    {
        this.weavingTask = weavingTask;
        this.buildEnginePropertyExtractor = buildEnginePropertyExtractor;
        this.logger = logger;
    }

    public void Execute()
    {
        if (string.IsNullOrWhiteSpace(weavingTask.TargetPath))
        {
            try
            {
                weavingTask.TargetPath = buildEnginePropertyExtractor.GetEnvironmentVariable("TargetPath", true).First();
                logger.LogMessage("\tYou did not define the WeavingTask.TargetPath. So it was extracted from the BuildEngine.");
            }
            catch (Exception exception)
            {
                throw new WeavingException(string.Format(
                    @"Failed to extract target assembly path from the BuildEngine. 
Please raise a bug here http://code.google.com/p/costura/issues/list with the below exception text.
The temporary work-around is to change the weaving task as follows 
<EmbedTask ... TargetPath=""@(TargetPath)"" />
Exception details: {0}", exception));
            }
        }
        if (!File.Exists(weavingTask.TargetPath))
        {
            throw new WeavingException(string.Format("TargetPath \"{0}\" does not exists. If you have not done a build you can ignore this error.", weavingTask.TargetPath));
        }

    }
}