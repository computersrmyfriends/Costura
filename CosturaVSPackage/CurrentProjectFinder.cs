using System.ComponentModel.Composition;
using System.Linq;
using System.Runtime.InteropServices;
using EnvDTE;
using Microsoft.VisualStudio.Shell;

[Export, PartCreationPolicy(CreationPolicy.Shared)]
public class CurrentProjectFinder
{


    public Project GetCurrentProject()
    {
        var dte = (DTE) ServiceProvider.GlobalProvider.GetService(typeof (DTE));
        if (dte.Solution == null)
        {
            return null;
        }
        if (string.IsNullOrEmpty(dte.Solution.FullName))
        {
            return null;
        }
        try
        {
            var objects = (object[]) dte.ActiveSolutionProjects;
            return (Project) objects.FirstOrDefault();
        }
        catch (COMException)
        {
            return null;
        }
    }

//        internal static EnvDTE.Project GetCurrentProject()
//        {
//            var dte = (DTE)ServiceProvider.GlobalProvider.GetService(typeof(DTE));
//            Project proj = null;

//            //check if this dte object contains a solution
//            if (dte == null || dte.Solution.Count <= 0)
//            {
//                return null;
//            }
//            //check if the solution has at least one project
//            if (dte.Solution.Projects.Count <= 0)
//            {
//                return null;
//            }
////try to find the project containing the active document,
//            //which will be the form that the component is being added
//            //If found then this is the current project
//            Document activeDoc = dte.ActiveDocument;
//            Project tempProj;
//            ProjectItem projItem;
//            string itemFullName;
//            for (int i = 1; i <= dte.Solution.Projects.Count; i++)
//            {
//                tempProj = dte.Solution.Projects.Item(i);
//                for (int j = 1; j <= tempProj.ProjectItems.Count; j++)
//                {
//                    projItem = tempProj.ProjectItems.Item(j);
//                    itemFullName = projItem.get_FileNames(1);
//                    if (itemFullName.Equals(activeDoc.FullName))
//                    {
//                        proj = tempProj;
//                        return proj;
//                    }
//                }
//            }
//            return proj;
//        }
}