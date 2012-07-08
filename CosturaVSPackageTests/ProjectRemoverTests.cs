using System.IO;
using NUnit.Framework;


[TestFixture]
public class ProjectRemoverTests
{
	[Test]
	public void WithNoWeavingNotChanged()
	{
		var sourceProjectFile = new FileInfo(@"TestProjects\ProjectWithNoWeaving.csproj");
		var targetFileInfo = sourceProjectFile.CopyTo(sourceProjectFile.FullName + "ProjectRemoverTests", true);

		try
		{
			new ProjectRemover(targetFileInfo.FullName);
			var reader = new ProjectReader(targetFileInfo.FullName);
			Assert.IsNull(reader.Overwrite);
			Assert.IsNull(reader.IncludeDebugSymbols);
			Assert.IsNull(reader.DeleteReferences);
			Assert.IsNull(reader.TargetPath);
			Assert.IsNull(reader.ToolsDirectory);
		}
		finally
		{
			targetFileInfo.Delete();
		}
	}


	[Test]
	public void WeavingRemoved()
	{
		var sourceProjectFile = new FileInfo(@"TestProjects\ProjectWithWeaving.csproj");
		var targetFileInfo = sourceProjectFile.CopyTo(sourceProjectFile.FullName + "ProjectRemoverTests", true);

		try
		{
			new ProjectRemover(targetFileInfo.FullName);

			var reader = new ProjectReader(targetFileInfo.FullName);

			Assert.IsNull(reader.Overwrite);
			Assert.IsNull(reader.IncludeDebugSymbols);
			Assert.IsNull(reader.DeleteReferences);
			Assert.IsNull(reader.TargetPath);
			Assert.IsNull(reader.ToolsDirectory);
		}
		finally
		{
			targetFileInfo.Delete();
		}
	}
}