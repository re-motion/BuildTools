using System;
using System.IO;
using System.Linq;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using NUnit.Framework;
using Remotion.BuildTools.MSBuildTasks;

namespace BuildTools.MSBuildTasks.UnitTests
{
  [TestFixture]
  public class BuildTestOutputFilesTest
  {
    [Test]
    public void ValidConfiguration_CorrectBrowser ()
    {
      var taskItem = new TaskItem("MyTest.dll");
      taskItem.SetMetadata("TestingConfiguration", "Chrome+NoDb+x86+dockerNet45+release");
      var task = new BuildTestOutputFiles { Input = new ITaskItem[] { taskItem } };

      task.Execute();
      
      Assert.That (task.Output.Single().GetMetadata("Browser"), Is.EqualTo ("Chrome"));
    }
    
    [Test]
    public void ValidConfiguration_CorrectDatabase ()
    {
      var taskItem = new TaskItem("MyTest.dll");
      taskItem.SetMetadata("TestingConfiguration", "Chrome+NoDb+x86+dockerNet45+release");
      var task = new BuildTestOutputFiles { Input = new ITaskItem[] { taskItem } };

      task.Execute();
      
      Assert.That (task.Output.Single().GetMetadata("Database"), Is.EqualTo ("NoDb"));
    }
    
    [Test]
    public void ValidConfiguration_CorrectCpuArchitecture ()
    {
      var taskItem = new TaskItem("MyTest.dll");
      taskItem.SetMetadata("TestingConfiguration", "Chrome+NoDb+x86+dockerNet45+release");
      var task = new BuildTestOutputFiles { Input = new ITaskItem[] { taskItem } };

      task.Execute();
      
      Assert.That (task.Output.Single().GetMetadata("CpuArchitecture"), Is.EqualTo ("x86"));
    }
    
    [Test]
    public void ValidConfiguration_CorrectDockerConfiguration ()
    {
      var taskItem = new TaskItem("MyTest.dll");
      taskItem.SetMetadata("TestingConfiguration", "Chrome+NoDb+x86+dockerNet45+release");
      var task = new BuildTestOutputFiles { Input = new ITaskItem[] { taskItem } };

      task.Execute();
      
      Assert.That (task.Output.Single().GetMetadata("DockerConfiguration"), Is.EqualTo ("dockerNet45"));
    }
    
    [Test]
    public void ValidConfiguration_CorrectBuildConfiguration ()
    {
      var taskItem = new TaskItem("MyTest.dll");
      taskItem.SetMetadata("TestingConfiguration", "Chrome+NoDb+x86+dockerNet45+release");
      var task = new BuildTestOutputFiles { Input = new ITaskItem[] { taskItem } };

      task.Execute();
      
      Assert.That (task.Output.Single().GetMetadata("BuildConfiguration"), Is.EqualTo ("release"));
    }
    
    [Test]
    public void MultipleConfigurations_CorrectParsing ()
    {
      var taskItem = new TaskItem("MyTest.dll");
      taskItem.SetMetadata("TestingConfiguration", "Chrome+NoDb+x86+dockerNet45+release;Firefox+SqlServer2012+x64+dockerNet45+debug");
      var task = new BuildTestOutputFiles { Input = new ITaskItem[] { taskItem } };

      task.Execute();
      
      Assert.That (task.Output[1].GetMetadata("Browser"), Is.EqualTo ("Firefox"));
      Assert.That (task.Output[1].GetMetadata("Database"), Is.EqualTo ("SqlServer2012"));
      Assert.That (task.Output[1].GetMetadata("CpuArchitecture"), Is.EqualTo ("x64"));
      Assert.That (task.Output[1].GetMetadata("DockerConfiguration"), Is.EqualTo ("dockerNet45"));
      Assert.That (task.Output[1].GetMetadata("BuildConfiguration"), Is.EqualTo ("debug"));
    }
    
    [Test]
    public void ValidConfiguration_CopiesIdentifierWithIndex ()
    {
      var taskItem = new TaskItem("MyTest.dll");
      taskItem.SetMetadata("TestingConfiguration", "Chrome+NoDb+x86+dockerNet45+release");
      var task = new BuildTestOutputFiles { Input = new ITaskItem[] { taskItem } };

      task.Execute();
      
      Assert.That (task.Output.Single().ItemSpec, Is.EqualTo("MyTest.dll_0"));
    }
  }
}