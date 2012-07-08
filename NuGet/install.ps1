param($installPath, $toolsPath, $package, $project)

$path_to_vsix = $Env:LOCALAPPDATA + "\Microsoft\VisualStudio\10.0\Extensions\Simon Cropp\Costura\9.9"
$vsix_exist = test-path -path $path_to_vsix



If (!$vsix_exist) {
	$DirInfo = New-Object System.IO.DirectoryInfo($Env:VS100COMNTOOLS)
	$path = [io.path]::Combine($DirInfo.Parent.FullName, "IDE")
	$path = [io.path]::Combine($path, "VSIXInstaller.exe")
	[Array]$arguments = $toolsPath + "\CosturaVsPackage.vsix"
	
	&$path $arguments | out-null

}
uninstall-package Costura -ProjectName $project.Name
$project.Save()
$assemblyPath= $path_to_vsix + "\CosturaVsPackage.dll"
Add-Type -Path $assemblyPath


$buildTaskDir = [System.IO.Path]::Combine($project.Object.DTE.Solution.FullName,"..\Tools")

$resourceExporter = New-Object CosturaFileExporter
$resourceExporter.ExportTask($buildTaskDir)

$projectInjector = New-Object CosturaProjectInjector
$projectInjector.ToolsDirectory = "`$(SolutionDir)Tools"
$projectInjector.ProjectFile = $project.FullName 
$projectInjector.Execute()



If (!$vsix_exist) {
	"You must restart Microsoft Visual Studio in order for the changes to take effect"
}
