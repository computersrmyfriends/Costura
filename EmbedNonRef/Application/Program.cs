using System;
using System.Reflection;

class Program
{
    static void Main(string[] args)
    {
        //This is contrived. in reality you would have code that determins which assembly and type to load here
        var type = Assembly.Load("ImplementationLibrary").GetType("Foo");
        var instance = (IFoo) Activator.CreateInstance(type);
        Console.WriteLine(instance.Bar());
    }
}