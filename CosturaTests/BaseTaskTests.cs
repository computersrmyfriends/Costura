using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Mono.Cecil;
using NUnit.Framework;

public abstract class BaseTaskTests
{
	string projectPath;
	Assembly inMemoryTemplateAssembly;
	Assembly tempFileTemplateAssembly;

	protected BaseTaskTests(string projectPath)
	{

#if (!DEBUG)
            projectPath = projectPath.Replace("Debug", "Release");
#endif
		this.projectPath = projectPath;
	}

	[TestFixtureSetUp]
	public void Setup()
	{
		tempFileTemplateAssembly = new WeaverHelper(projectPath,true).Assembly;
		inMemoryTemplateAssembly = new WeaverHelper(projectPath,false).Assembly;
	}


	[Test]
	public void SimpleInMemory()
	{
        var instance2 = inMemoryTemplateAssembly.GetInstance("ClassToTest");
		Assert.AreEqual("Hello", instance2.Foo());
	}
	[Test]
	public void SimpleTempFile()
	{
        var instance1 = tempFileTemplateAssembly.GetInstance("ClassToTest");
		Assert.AreEqual("Hello", instance1.Foo());
	}
	[Test]
	public void SimpleInMemoryPreEmbed()
	{
        var instance2 = inMemoryTemplateAssembly.GetInstance("ClassToTest");
		Assert.AreEqual("Hello", instance2.Foo2());
	}
	[Test]
    public void SimpleTempFilePreEmbed()
	{
        var instance1 = tempFileTemplateAssembly.GetInstance("ClassToTest");
		Assert.AreEqual("Hello", instance1.Foo2());
	}

	[Test]
	public void ThrowExceptionTempFile()
	{
		try
		{
			var instance = tempFileTemplateAssembly.GetInstance("ClassToTest");
			instance.ThrowException();
		}
		catch (Exception exception)
		{
			Debug.WriteLine(exception.StackTrace);
			Assert.IsTrue(exception.StackTrace.Contains("ClassToReference.cs:line"));	
		}
	}
	[Test]
	public void ThrowExceptionInMemory()
	{
		try
		{
			var instance = inMemoryTemplateAssembly.GetInstance("ClassToTest");
			instance.ThrowException();
		}
		catch (Exception exception)
		{
			Debug.WriteLine(exception.StackTrace);
			Assert.IsTrue(exception.StackTrace.Contains("ClassToReference.cs:line"));	
		}
	}

#if(DEBUG)
	[Test]
	public void PeVerifyTempFile()
	{
		Verifier.Verify(tempFileTemplateAssembly.CodeBase.Remove(0, 8));
	}
	[Test]
	public void PeVerifyInMemory()
	{
		Verifier.Verify(inMemoryTemplateAssembly.CodeBase.Remove(0, 8));
	}

	[Test]
	public void EnsureOnly1RefToMscorLibInMemory()
	{
		var moduleDefinition = ModuleDefinition.ReadModule(inMemoryTemplateAssembly.CodeBase.Remove(0, 8));
		Assert.AreEqual(1, moduleDefinition.AssemblyReferences.Count(x => x.Name == "mscorlib"));
	}
	[Test]
	public void EnsureOnly1RefToMscorLibTempFile()
	{
		var moduleDefinition = ModuleDefinition.ReadModule(tempFileTemplateAssembly.CodeBase.Remove(0, 8));
		Assert.AreEqual(1, moduleDefinition.AssemblyReferences.Count(x => x.Name == "mscorlib"));
	}
#endif
}