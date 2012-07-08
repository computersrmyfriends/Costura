using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.Shell;

[ProvideAutoLoad("F1536EF8-92EC-443C-9ED7-FDADF150DA82")] //SolutionExists
[ProvideAutoLoad("ADFC4E64-0397-11D1-9F4E-00A0C911004F ")] //NoSolution
[ProvideAutoLoad("4d7a79c7-e2e3-4140-93cc-f0e68a6cae56")]
[PackageRegistration(UseManagedResourcesOnly = true)]
[InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
[ProvideMenuResource("Menus.ctmenu", 1)]
[Guid("1e18710b-e9fc-4b7c-9c27-56237c08a165")]
public sealed class CosturaVSPackagePackage : Package
{
    protected override void Initialize()
    {
        base.Initialize();
        var exceptionDialog = new ExceptionDialog();
        try
        {
            using (var catalog = new AssemblyCatalog(GetType().Assembly))
            using (var container = new CompositionContainer(catalog))
            {
                var menuCommandService = (IMenuCommandService) GetService(typeof (IMenuCommandService));
                var errorListProvider = new ErrorListProvider(ServiceProvider.GlobalProvider);

                container.ComposeExportedValue(exceptionDialog);
                container.ComposeExportedValue(menuCommandService);
                container.ComposeExportedValue(errorListProvider);

                container.GetExportedValue<MenuConfigure>().RegisterMenus();
                container.GetExportedValue<SolutionEvents>().RegisterSolutionEvents();
                container.GetExportedValue<TaskFileReplacer>().CheckForFilesToUpdate();
            }
        }
        catch (Exception exception)
        {
            exceptionDialog.HandleException(exception);
        }
    }

}