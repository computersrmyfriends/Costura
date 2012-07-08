using System;
using System.Reflection;
using NUnit.Framework;

[TestFixture]
public class AssemblyWithLoadNonExistentAssemblyTests
{
    string projectPath;

    public AssemblyWithLoadNonExistentAssemblyTests()
    {
        projectPath = @"EmbedTestAssemblies\AssemblyWithLoadNonExistentAssembly\AssemblyWithLoadNonExistentAssembly.csproj";
#if (!DEBUG)
        projectPath = projectPath.Replace("Debug", "Release");
#endif
    }

    [Test]
    public void EnsureNoExceptionTempFile()
    {
        var weaverHelper = new WeaverHelper(projectPath, true);
        Assert(weaverHelper.Assembly);
    }

    [Test]
    public void EnsureNoExceptionInMemory()
    {
        var weaverHelper = new WeaverHelper(projectPath, false);
        Assert(weaverHelper.Assembly);
    }

    void Assert(Assembly assembly)
    {
        var instance = assembly.GetInstance("ClassToTest");

        var expected = GetExceptionString(() => Assembly.Load("BadAssemblyName"));
        var actual = GetExceptionString(() => instance.MethodThatDoesLoading());
        NUnit.Framework.Assert.AreEqual(expected, actual);
    }

    string GetExceptionString(Action action)
    {
        try
        {
            action();
        }
        catch (Exception exception)
        {
            return exception.Message;
        }
        return null;
    }
}