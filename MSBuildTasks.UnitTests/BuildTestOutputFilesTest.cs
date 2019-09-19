using System;
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
      var taskItem = new TaskItem ("MyTest.dll");
      taskItem.SetMetadata ("TestingConfiguration", "Chrome+NoDb+x86+dockerNet45+release");
      var task = new BuildTestOutputFiles { Input = new ITaskItem[] { taskItem } };

      task.Execute();

      Assert.That (task.Output.Single().GetMetadata ("Browser"), Is.EqualTo ("Chrome"));
    }

    [Test]
    public void ValidConfiguration_CorrectDatabase ()
    {
      var taskItem = new TaskItem ("MyTest.dll");
      taskItem.SetMetadata ("TestingConfiguration", "Chrome+NoDb+x86+dockerNet45+release");
      var task = new BuildTestOutputFiles { Input = new ITaskItem[] { taskItem } };

      task.Execute();

      Assert.That (task.Output.Single().GetMetadata ("DatabaseSystem"), Is.EqualTo ("NoDb"));
    }

    [Test]
    public void ValidConfiguration_CorrectPlatform ()
    {
      var taskItem = new TaskItem ("MyTest.dll");
      taskItem.SetMetadata ("TestingConfiguration", "Chrome+NoDb+x86+dockerNet45+release");
      var task = new BuildTestOutputFiles { Input = new ITaskItem[] { taskItem } };

      task.Execute();

      Assert.That (task.Output.Single().GetMetadata ("Platform"), Is.EqualTo ("x86"));
    }

    [Test]
    public void ValidConfiguration_CorrectDockerConfiguration ()
    {
      var taskItem = new TaskItem ("MyTest.dll");
      taskItem.SetMetadata ("TestingConfiguration", "Chrome+NoDb+x86+dockerNet45+release");
      var task = new BuildTestOutputFiles { Input = new ITaskItem[] { taskItem } };

      task.Execute();

      Assert.That (task.Output.Single().GetMetadata ("ExecutionRuntime"), Is.EqualTo ("dockerNet45"));
    }

    [Test]
    public void ValidConfiguration_CorrectBuildConfiguration ()
    {
      var taskItem = new TaskItem ("MyTest.dll");
      taskItem.SetMetadata ("TestingConfiguration", "Chrome+NoDb+x86+dockerNet45+release");
      var task = new BuildTestOutputFiles { Input = new ITaskItem[] { taskItem } };

      task.Execute();

      Assert.That (task.Output.Single().GetMetadata ("ConfigurationID"), Is.EqualTo ("release"));
    }

    [Test]
    public void MultipleConfigurations_CorrectParsing ()
    {
      var taskItem = new TaskItem ("MyTest.dll");
      taskItem.SetMetadata ("TestingConfiguration", "Chrome+NoDb+x86+dockerNet45+release;Firefox+SqlServer2012+x64+dockerNet45+debug");
      var task = new BuildTestOutputFiles { Input = new ITaskItem[] { taskItem } };

      task.Execute();

      Assert.That (task.Output[1].GetMetadata ("Browser"), Is.EqualTo ("Firefox"));
      Assert.That (task.Output[1].GetMetadata ("DatabaseSystem"), Is.EqualTo ("SqlServer2012"));
      Assert.That (task.Output[1].GetMetadata ("Platform"), Is.EqualTo ("x64"));
      Assert.That (task.Output[1].GetMetadata ("ExecutionRuntime"), Is.EqualTo ("dockerNet45"));
      Assert.That (task.Output[1].GetMetadata ("ConfigurationID"), Is.EqualTo ("debug"));
    }

    [Test]
    public void ValidConfiguration_CopiesItemSpec ()
    {
      var taskItem = new TaskItem ("MyTest.dll");
      taskItem.SetMetadata ("TestingConfiguration", "Chrome+NoDb+x86+dockerNet45+release");
      var task = new BuildTestOutputFiles { Input = new ITaskItem[] { taskItem } };

      task.Execute();

      Assert.That (task.Output.Single().ItemSpec, Is.EqualTo ("MyTest.dll"));
    }

    [Test]
    public void ValidConfiguration_CopiesMultipleIdentifiersWithIndex ()
    {
      const string itemSpec = "MyTest.dll";
      var taskItem = new TaskItem (itemSpec);
      taskItem.SetMetadata ("TestingConfiguration", "Chrome+NoDb+x86+dockerNet45+release;Firefox+SqlServer2012+x64+dockerNet45+debug");
      var task = new BuildTestOutputFiles { Input = new ITaskItem[] { taskItem } };

      task.Execute();

      Assert.That (task.Output[1].ItemSpec, Is.EqualTo (itemSpec));
    }

    [Test]
    public void Use32Bit_x86_True ()
    {
      const string itemSpec = "MyTest.dll";
      var taskItem = new TaskItem (itemSpec);
      taskItem.SetMetadata ("TestingConfiguration", "Chrome+NoDb+x86+dockerNet45+release");
      var task = new BuildTestOutputFiles { Input = new ITaskItem[] { taskItem } };

      task.Execute();

      Assert.That (task.Output.Single().GetMetadata ("Use32Bit"), Is.EqualTo ("true"));
    }

    [Test]
    public void Use32Bit_x64_False ()
    {
      const string itemSpec = "MyTest.dll";
      var taskItem = new TaskItem (itemSpec);
      taskItem.SetMetadata ("TestingConfiguration", "Chrome+NoDb+x64+dockerNet45+release");
      var task = new BuildTestOutputFiles { Input = new ITaskItem[] { taskItem } };

      task.Execute();

      Assert.That (task.Output.Single().GetMetadata ("Use32Bit"), Is.EqualTo ("false"));
    }

    [Test]
    public void IsDatabaseTest_NoDb_False ()
    {
      const string itemSpec = "MyTest.dll";
      var taskItem = new TaskItem (itemSpec);
      taskItem.SetMetadata ("TestingConfiguration", "Chrome+NoDb+x64+dockerNet45+release");
      var task = new BuildTestOutputFiles { Input = new ITaskItem[] { taskItem } };

      task.Execute();

      Assert.That (task.Output.Single().GetMetadata ("IsDatabaseTest"), Is.EqualTo ("false"));
    }

    [Test]
    public void IsDatabaseTest_NotNoDb_True ()
    {
      const string itemSpec = "MyTest.dll";
      var taskItem = new TaskItem (itemSpec);
      taskItem.SetMetadata ("TestingConfiguration", "Chrome+SqlServer2014+x64+dockerNet45+release");
      var task = new BuildTestOutputFiles { Input = new ITaskItem[] { taskItem } };

      task.Execute();

      Assert.That (task.Output.Single().GetMetadata ("IsDatabaseTest"), Is.EqualTo ("true"));
    }

    [Test]
    public void IsWebTest_NoBrowser_False ()
    {
      const string itemSpec = "MyTest.dll";
      var taskItem = new TaskItem (itemSpec);
      taskItem.SetMetadata ("TestingConfiguration", "NoBrowser+SqlServer2014+x64+dockerNet45+release");
      var task = new BuildTestOutputFiles { Input = new ITaskItem[] { taskItem } };

      task.Execute();

      Assert.That (task.Output.Single().GetMetadata ("IsWebTest"), Is.EqualTo ("false"));
    }
  }
}