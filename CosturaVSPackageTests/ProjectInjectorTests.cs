using System.IO;
using Microsoft.Build.Framework;
using NUnit.Framework;


[TestFixture]
public class ProjectInjectorTests
{
	[Test]
	public void WithNoWeaving()
	{
		var sourceProjectFile = new FileInfo(@"TestProjects\ProjectWithNoWeaving.csproj");
		var targetFileInfo = sourceProjectFile.CopyTo(sourceProjectFile.FullName + "ProjectInjectorTests", true);
		try
		{

            var injector = new CosturaProjectInjector
			               	{
			               		ToolsDirectory = @"Tools\",
			               		ProjectFile = targetFileInfo.FullName,
			               		TargetPath = "Foo.dll",
			               		Overwrite = false,
			               		IncludeDebugSymbols = false,
			               		DeleteReferences = false,
			               		MessageImportance = MessageImportance.High,
			               	};
			injector.Execute();

			var reader = new ProjectReader(targetFileInfo.FullName);

			Assert.IsFalse(reader.Overwrite.Value);
			Assert.IsFalse(reader.IncludeDebugSymbols.Value);
			Assert.IsFalse(reader.DeleteReferences.Value);
			Assert.AreEqual("Foo.dll", reader.TargetPath);
			Assert.AreEqual(@"Tools\", reader.ToolsDirectory);
			Assert.AreEqual(MessageImportance.High, reader.MessageImportance);
		}
		finally
		{
			targetFileInfo.Delete();
		}
	}

	[Test]
	public void WithExistingWeaving()
	{
		var sourceProjectFile = new FileInfo(@"TestProjects\ProjectWithWeaving.csproj");
		var targetFileInfo = sourceProjectFile.CopyTo(sourceProjectFile.FullName + "ProjectInjectorTests", true);

		try
		{
            var injector = new CosturaProjectInjector
			               	{
			               		ToolsDirectory = @"Tools2\",
			               		ProjectFile = targetFileInfo.FullName,
			               		TargetPath = "Foo2.dll",
			               		Overwrite = false,
			               		IncludeDebugSymbols = false,
			               		DeleteReferences = false,
			               		MessageImportance = MessageImportance.High,
			               	};
			injector.Execute();

			var reader = new ProjectReader(targetFileInfo.FullName);

			Assert.IsFalse(reader.IncludeDebugSymbols.Value);
			Assert.IsFalse(reader.Overwrite.Value);
			Assert.IsFalse(reader.DeleteReferences.Value);
			Assert.AreEqual("Foo2.dll", reader.TargetPath);
			Assert.AreEqual(@"Tools2\", reader.ToolsDirectory);
			Assert.AreEqual(MessageImportance.High, reader.MessageImportance);

		}
		finally
		{
			targetFileInfo.Delete();
		}

	}

}