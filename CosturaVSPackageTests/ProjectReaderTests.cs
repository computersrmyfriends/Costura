using System.IO;
using Microsoft.Build.Framework;
using NUnit.Framework;


[TestFixture]
public class ProjectReaderTests
{

	[Test]
	public void WithNoWeaving()
	{
		var sourceProjectFile = new FileInfo(@"TestProjects\ProjectWithNoWeaving.csproj");
		var targetFileInfo = sourceProjectFile.CopyTo(sourceProjectFile.FullName + "ProjectReaderTest", true);
		try
		{

			var reader = new ProjectReader(targetFileInfo.FullName);

			Assert.IsNull(reader.Overwrite);
			Assert.IsNull(reader.IncludeDebugSymbols);
			Assert.IsNull(reader.DeleteReferences);
			Assert.IsNull(reader.ToolsDirectory);
			Assert.IsNull(reader.MessageImportance);
			Assert.IsNull(reader.TargetPath);
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
		var targetFileInfo = sourceProjectFile.CopyTo(sourceProjectFile.FullName + "ProjectReaderTest", true);
		try
		{
			var reader = new ProjectReader(targetFileInfo.FullName);
			Assert.IsTrue(reader.Overwrite.Value);
			Assert.IsTrue(reader.IncludeDebugSymbols.Value);
			Assert.IsTrue(reader.DeleteReferences.Value);
			Assert.AreEqual("@(TargetPath)", reader.TargetPath);
			Assert.AreEqual("$(SolutionDir)Tools\\", reader.ToolsDirectory);
			Assert.AreEqual(MessageImportance.High, reader.MessageImportance);
		}
		finally
		{
			targetFileInfo.Delete();
		}
	}


	[Test]
	public void WithMinimalWeaving()
	{

		var sourceProjectFile = new FileInfo(@"TestProjects\ProjectWithMinimalWeaving.csproj");
		var targetFileInfo = sourceProjectFile.CopyTo(sourceProjectFile.FullName + "ProjectReaderTest", true);
		try
		{
			var reader = new ProjectReader(targetFileInfo.FullName);
			Assert.IsNull(reader.Overwrite);
			Assert.IsNull(reader.IncludeDebugSymbols);
			Assert.IsNull(reader.DeleteReferences);
			Assert.IsNull(reader.TargetPath);
			Assert.AreEqual(@"$(SolutionDir)Tools\", reader.ToolsDirectory);
			Assert.IsNull(reader.MessageImportance);
		}
		finally
		{
			targetFileInfo.Delete();
		}

	}
}