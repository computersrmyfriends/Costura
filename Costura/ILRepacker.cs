using System.ComponentModel.Composition;
using ILRepacking;

namespace MergeTask
{
    [Export, PartCreationPolicy(CreationPolicy.Shared)]
    public class ILRepacker
    {
        ILRepackTask ilRepackTask;

        [ImportingConstructor]
        public ILRepacker(ILRepackTask ilRepackTask)
        {
            this.ilRepackTask = ilRepackTask;
        }

        public void Execute()
        {
            var ilRepack = new ILRepack();
//            ilRepack.InputAssemblies = 
        }
    }
}