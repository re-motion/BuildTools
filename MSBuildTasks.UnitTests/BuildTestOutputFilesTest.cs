// Copyright (c) rubicon IT GmbH, www.rubicon.eu
//
// See the NOTICE file distributed with this work for additional information
// regarding copyright ownership.  rubicon licenses this file to you under 
// the Apache License, Version 2.0 (the "License"); you may not use this 
// file except in compliance with the License.  You may obtain a copy of the 
// License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT 
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.  See the 
// License for the specific language governing permissions and limitations
// under the License.
// 

using System;
using System.IO.Abstractions;
using System.Linq;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using NUnit.Framework;
using Remotion.BuildTools.MSBuildTasks;
using Rhino.Mocks;

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

      Assert.That (task.Output.Single().GetMetadata (TestingConfigurationMetadata.Browser), Is.EqualTo ("Chrome"));
    }

    [Test]
    public void ValidConfiguration_CorrectDatabase ()
    {
      var taskItem = new TaskItem ("MyTest.dll");
      taskItem.SetMetadata ("TestingConfiguration", "Chrome+NoDb+x86+dockerNet45+release");
      var task = new BuildTestOutputFiles { Input = new ITaskItem[] { taskItem } };

      task.Execute();

      Assert.That (task.Output.Single().GetMetadata (TestingConfigurationMetadata.DatabaseSystem), Is.EqualTo ("NoDb"));
    }

    [Test]
    public void ValidConfiguration_CorrectPlatform ()
    {
      var taskItem = new TaskItem ("MyTest.dll");
      taskItem.SetMetadata ("TestingConfiguration", "Chrome+NoDb+x86+dockerNet45+release");
      var task = new BuildTestOutputFiles { Input = new ITaskItem[] { taskItem } };

      task.Execute();

      Assert.That (task.Output.Single().GetMetadata (TestingConfigurationMetadata.Platform), Is.EqualTo ("x86"));
    }

    [Test]
    public void ValidConfiguration_CorrectDockerConfiguration ()
    {
      var taskItem = new TaskItem ("MyTest.dll");
      taskItem.SetMetadata ("TestingConfiguration", "Chrome+NoDb+x86+dockerNet45+release");
      var task = new BuildTestOutputFiles { Input = new ITaskItem[] { taskItem } };

      task.Execute();

      Assert.That (task.Output.Single().GetMetadata (TestingConfigurationMetadata.ExecutionRuntime), Is.EqualTo ("dockerNet45"));
    }

    [Test]
    public void ValidConfiguration_CorrectBuildConfiguration ()
    {
      var taskItem = new TaskItem ("MyTest.dll");
      taskItem.SetMetadata ("TestingConfiguration", "Chrome+NoDb+x86+dockerNet45+release");
      var task = new BuildTestOutputFiles { Input = new ITaskItem[] { taskItem } };

      task.Execute();

      Assert.That (task.Output.Single().GetMetadata (TestingConfigurationMetadata.ConfigurationID), Is.EqualTo ("release"));
    }

    [Test]
    public void MultipleConfigurations_CorrectParsing ()
    {
      var taskItem = new TaskItem ("MyTest.dll");
      taskItem.SetMetadata ("TestingConfiguration", "Chrome+NoDb+x86+dockerNet45+release;Firefox+SqlServer2012+x64+dockerNet45+debug");
      var task = new BuildTestOutputFiles { Input = new ITaskItem[] { taskItem } };

      task.Execute();

      Assert.That (task.Output[1].GetMetadata (TestingConfigurationMetadata.Browser), Is.EqualTo ("Firefox"));
      Assert.That (task.Output[1].GetMetadata (TestingConfigurationMetadata.DatabaseSystem), Is.EqualTo ("SqlServer2012"));
      Assert.That (task.Output[1].GetMetadata (TestingConfigurationMetadata.Platform), Is.EqualTo ("x64"));
      Assert.That (task.Output[1].GetMetadata (TestingConfigurationMetadata.ExecutionRuntime), Is.EqualTo ("dockerNet45"));
      Assert.That (task.Output[1].GetMetadata (TestingConfigurationMetadata.ConfigurationID), Is.EqualTo ("debug"));
    }

    [Test]
    public void ConfigurationsWithTrailingWhitespaces_CorrectParsing ()
    {
      var taskItem = new TaskItem ("MyTest.dll");
      var config = $"{Environment.NewLine}    Firefox+SqlServer2012+x64+dockerNet45+debug{Environment.NewLine}    ";
      taskItem.SetMetadata ("TestingConfiguration", config);
      var task = new BuildTestOutputFiles { Input = new ITaskItem[] { taskItem } };

      task.Execute();

      Assert.That (task.Output.Single().GetMetadata (TestingConfigurationMetadata.Browser), Is.EqualTo ("Firefox"));
      Assert.That (task.Output.Single().GetMetadata (TestingConfigurationMetadata.DatabaseSystem), Is.EqualTo ("SqlServer2012"));
      Assert.That (task.Output.Single().GetMetadata (TestingConfigurationMetadata.Platform), Is.EqualTo ("x64"));
      Assert.That (task.Output.Single().GetMetadata (TestingConfigurationMetadata.ExecutionRuntime), Is.EqualTo ("dockerNet45"));
      Assert.That (task.Output.Single().GetMetadata (TestingConfigurationMetadata.ConfigurationID), Is.EqualTo ("debug"));
    }

    [Test]
    public void ConfigurationOrder_Irrelevant ()
    {
      var taskItem = new TaskItem ("MyTest.dll");
      const string config = "debug+Firefox+dockerNet45+SqlServer2012+x64";
      taskItem.SetMetadata ("TestingConfiguration", config);
      var task = new BuildTestOutputFiles
                 {
                     Input = new ITaskItem[] { taskItem },
                     SupportedDatabaseSystems = new ITaskItem[]{new TaskItem ("SqlServer2012")},
                     SupportedBrowsers = new ITaskItem[]{new TaskItem ("Firefox")},
                     SupportedPlatforms = new ITaskItem[]{new TaskItem ("x64")},
                     SupportedConfigurationIDs = new ITaskItem[]{new TaskItem ("debug")},
                     SupportedExecutionRuntimes = new ITaskItem[]{new TaskItem ("dockerNet45")}
                 };

      task.Execute();

      Assert.That (task.Output.Single().GetMetadata (TestingConfigurationMetadata.Browser), Is.EqualTo ("Firefox"));
      Assert.That (task.Output.Single().GetMetadata (TestingConfigurationMetadata.DatabaseSystem), Is.EqualTo ("SqlServer2012"));
      Assert.That (task.Output.Single().GetMetadata (TestingConfigurationMetadata.Platform), Is.EqualTo ("x64"));
      Assert.That (task.Output.Single().GetMetadata (TestingConfigurationMetadata.ExecutionRuntime), Is.EqualTo ("dockerNet45"));
      Assert.That (task.Output.Single().GetMetadata (TestingConfigurationMetadata.ConfigurationID), Is.EqualTo ("debug"));
    }

    [Test]
    public void MultipleConfigurationsWithTrailingWhitespaces_CorrectParsing ()
    {
      var taskItem = new TaskItem ("MyTest.dll");
      var config =
          $"{Environment.NewLine}    Chrome+NoDb+x86+dockerNet45+release;  {Environment.NewLine}  Firefox+SqlServer2012+x64+dockerNet45+debug{Environment.NewLine}    ";
      taskItem.SetMetadata ("TestingConfiguration", config);
      var task = new BuildTestOutputFiles { Input = new ITaskItem[] { taskItem } };

      task.Execute();

      Assert.That (task.Output[1].GetMetadata (TestingConfigurationMetadata.Browser), Is.EqualTo ("Firefox"));
      Assert.That (task.Output[1].GetMetadata (TestingConfigurationMetadata.DatabaseSystem), Is.EqualTo ("SqlServer2012"));
      Assert.That (task.Output[1].GetMetadata (TestingConfigurationMetadata.Platform), Is.EqualTo ("x64"));
      Assert.That (task.Output[1].GetMetadata (TestingConfigurationMetadata.ExecutionRuntime), Is.EqualTo ("dockerNet45"));
      Assert.That (task.Output[1].GetMetadata (TestingConfigurationMetadata.ConfigurationID), Is.EqualTo ("debug"));
    }

    [Test]
    public void MultipleConfigurationsWithTrailingSemiColon_CorrectParsing ()
    {
      var taskItem = new TaskItem ("MyTest.dll");
      const string config = "Chrome+NoDb+x86+dockerNet45+release;Firefox+SqlServer2012+x64+dockerNet45+debug;";
      taskItem.SetMetadata ("TestingConfiguration", config);
      var task = new BuildTestOutputFiles { Input = new ITaskItem[] { taskItem } };

      task.Execute();

      Assert.That (task.Output[1].GetMetadata (TestingConfigurationMetadata.Browser), Is.EqualTo ("Firefox"));
      Assert.That (task.Output[1].GetMetadata (TestingConfigurationMetadata.DatabaseSystem), Is.EqualTo ("SqlServer2012"));
      Assert.That (task.Output[1].GetMetadata (TestingConfigurationMetadata.Platform), Is.EqualTo ("x64"));
      Assert.That (task.Output[1].GetMetadata (TestingConfigurationMetadata.ExecutionRuntime), Is.EqualTo ("dockerNet45"));
      Assert.That (task.Output[1].GetMetadata (TestingConfigurationMetadata.ConfigurationID), Is.EqualTo ("debug"));
    }

    [Test]
    public void ItemSpec_IsAssemblyNameAndConfiguration ()
    {
      const string itemSpec = "MyTest.dll";
      var taskItem = new TaskItem (itemSpec);
      const string config = "Chrome+NoDb+x86+dockerNet45+release";
      taskItem.SetMetadata ("TestingConfiguration", config);
      var task = new BuildTestOutputFiles { Input = new ITaskItem[] { taskItem } };

      task.Execute();

      Assert.That (task.Output.Single().ItemSpec, Is.EqualTo (itemSpec + "_" + config));
    }

    [Test]
    public void ItemSpecWithFullPath_IsAssemblyNameAndConfiguration ()
    {
      const string assemblyName = "MyTest.dll";
      var assemblyFullPath = $"C:\\Development\\{assemblyName}";
      var taskItem = new TaskItem (assemblyFullPath);
      const string config = "Chrome+NoDb+x86+dockerNet45+release";
      taskItem.SetMetadata ("TestingConfiguration", config);
      var task = new BuildTestOutputFiles { Input = new ITaskItem[] { taskItem } };

      task.Execute();

      Assert.That (task.Output.Single().ItemSpec, Is.EqualTo (assemblyName + "_" + config));
    }

    [Test]
    public void ValidConfiguration_CopiesMultipleIdentifiers ()
    {
      const string itemSpec = "MyTest.csproj";
      var taskItem = new TaskItem (itemSpec);
      const string config1 = "Chrome+NoDb+x86+dockerNet45+release";
      const string config2 = "Firefox+SqlServer2012+x64+dockerNet45+debug";
      taskItem.SetMetadata ("TestingConfiguration", config1 + ";" + config2);
      var task = new BuildTestOutputFiles { Input = new ITaskItem[] { taskItem } };

      task.Execute();

      Assert.That (task.Output[1].ItemSpec, Is.EqualTo (itemSpec + "_" + config2));
    }

    [Test]
    public void Use32Bit_x86_True ()
    {
      var taskItem = new TaskItem ("MyTest.dll");
      taskItem.SetMetadata ("TestingConfiguration", "Chrome+NoDb+x86+dockerNet45+release");
      var task = new BuildTestOutputFiles { Input = new ITaskItem[] { taskItem } };

      task.Execute();

      Assert.That (task.Output.Single().GetMetadata (TestingConfigurationMetadata.Use32Bit), Is.EqualTo ("True"));
    }

    [Test]
    public void Use32Bit_x86CaseInsensitive_True ()
    {
      var taskItem = new TaskItem ("MyTest.dll");
      taskItem.SetMetadata ("TestingConfiguration", "Chrome+NoDb+X86+dockerNet45+release");
      var task = new BuildTestOutputFiles { Input = new ITaskItem[] { taskItem } };

      task.Execute();

      Assert.That (task.Output.Single().GetMetadata (TestingConfigurationMetadata.Use32Bit), Is.EqualTo ("True"));
    }

    [Test]
    public void Use32Bit_x64_False ()
    {
      var taskItem = new TaskItem ("MyTest.dll");
      taskItem.SetMetadata ("TestingConfiguration", "Chrome+NoDb+x64+dockerNet45+release");
      var task = new BuildTestOutputFiles { Input = new ITaskItem[] { taskItem } };

      task.Execute();

      Assert.That (task.Output.Single().GetMetadata (TestingConfigurationMetadata.Use32Bit), Is.EqualTo ("False"));
    }

    [Test]
    public void IsDatabaseTest_NoDb_False ()
    {
      var taskItem = new TaskItem ("MyTest.dll");
      taskItem.SetMetadata ("TestingConfiguration", "Chrome+NoDb+x64+dockerNet45+release");
      var task = new BuildTestOutputFiles { Input = new ITaskItem[] { taskItem } };

      task.Execute();

      Assert.That (task.Output.Single().GetMetadata (TestingConfigurationMetadata.IsDatabaseTest), Is.EqualTo ("False"));
    }

    [Test]
    public void IsDatabaseTest_NotNoDb_True ()
    {
      var taskItem = new TaskItem ("MyTest.dll");
      taskItem.SetMetadata ("TestingConfiguration", "Chrome+SqlServer2014+x64+dockerNet45+release");
      var task = new BuildTestOutputFiles { Input = new ITaskItem[] { taskItem } };

      task.Execute();

      Assert.That (task.Output.Single().GetMetadata (TestingConfigurationMetadata.IsDatabaseTest), Is.EqualTo ("True"));
    }

    [Test]
    public void IsDatabaseTest_NoDbIgnoreCase_False ()
    {
      var taskItem = new TaskItem ("MyTest.dll");
      taskItem.SetMetadata ("TestingConfiguration", "Chrome+nodb+x64+dockerNet45+release");
      var task = new BuildTestOutputFiles { Input = new ITaskItem[] { taskItem } };

      task.Execute();

      Assert.That (task.Output.Single().GetMetadata (TestingConfigurationMetadata.IsDatabaseTest), Is.EqualTo ("False"));
    }

    [Test]
    public void IsWebTest_NoBrowser_False ()
    {
      var taskItem = new TaskItem ("MyTest.dll");
      taskItem.SetMetadata ("TestingConfiguration", "NoBrowser+SqlServer2014+x64+dockerNet45+release");
      var task = new BuildTestOutputFiles { Input = new ITaskItem[] { taskItem } };

      task.Execute();

      Assert.That (task.Output.Single().GetMetadata (TestingConfigurationMetadata.IsWebTest), Is.EqualTo ("False"));
    }

    [Test]
    public void IsWebTest_NoBrowserIgnoreCase_False ()
    {
      var taskItem = new TaskItem ("MyTest.dll");
      taskItem.SetMetadata ("TestingConfiguration", "nobrowser+SqlServer2014+x64+dockerNet45+release");
      var task = new BuildTestOutputFiles { Input = new ITaskItem[] { taskItem } };

      task.Execute();

      Assert.That (task.Output.Single().GetMetadata (TestingConfigurationMetadata.IsWebTest), Is.EqualTo ("False"));
    }

    [Test]
    public void IsWebTest_NotNoBrowser_True ()
    {
      var taskItem = new TaskItem ("MyTest.dll");
      taskItem.SetMetadata ("TestingConfiguration", "Chrome+SqlServer2014+x64+dockerNet45+release");
      var task = new BuildTestOutputFiles { Input = new ITaskItem[] { taskItem } };

      task.Execute();

      Assert.That (task.Output.Single().GetMetadata (TestingConfigurationMetadata.IsWebTest), Is.EqualTo ("True"));
    }

    [Test]
    public void TestAssemblyFileName_CopiesOriginalItemSpec ()
    {
      const string testAssemblyFileName = "MyTest.dll";
      var taskItem = new TaskItem (testAssemblyFileName);
      taskItem.SetMetadata ("TestingConfiguration", "Chrome+SqlServer2014+x64+dockerNet45+release");
      var task = new BuildTestOutputFiles { Input = new ITaskItem[] { taskItem } };

      task.Execute();

      Assert.That (task.Output.Single().GetMetadata (TestingConfigurationMetadata.TestAssemblyFileName), Is.EqualTo (testAssemblyFileName));
    }

    [Test]
    public void TestAssemblyFileName_GetsFileNameFromFullPath ()
    {
      const string testAssemblyFileName = "MyTest.dll";
      const string testAssemblyDirectoryName = "Development";
      var testAssemblyFullPath = $"C:\\{testAssemblyDirectoryName}\\{testAssemblyFileName}";
      var taskItem = new TaskItem (testAssemblyFullPath);
      taskItem.SetMetadata ("TestingConfiguration", "Chrome+SqlServer2014+x64+dockerNet45+release");
      var pathStub = MockRepository.Mock<IPath>();
      pathStub.Stub (_ => _.GetFileName (testAssemblyFullPath)).Return (testAssemblyFileName);
      pathStub.Stub (_ => _.GetFullPath (testAssemblyFullPath)).Return (testAssemblyFullPath);
      pathStub.Stub (_ => _.GetDirectoryName (testAssemblyFullPath)).Return (testAssemblyDirectoryName);
      var task = new BuildTestOutputFiles (pathStub) { Input = new ITaskItem[] { taskItem } };

      task.Execute();

      Assert.That (task.Output.Single().GetMetadata (TestingConfigurationMetadata.TestAssemblyFileName), Is.EqualTo (testAssemblyFileName));
    }

    [Test]
    public void TestAssemblyFullPath_IsMetadata ()
    {
      const string testAssemblyFileName = "MyTest.dll";
      const string testAssemblyDirectoryName = "Development";
      var testAssemblyFullPath = $"C:\\{testAssemblyDirectoryName}\\{testAssemblyFileName}";
      var pathStub = MockRepository.Mock<IPath>();
      pathStub.Stub (_ => _.GetFullPath (testAssemblyFileName)).Return (testAssemblyFullPath);
      pathStub.Stub (_ => _.GetFileName (testAssemblyFileName)).Return (testAssemblyFileName);
      pathStub.Stub (_ => _.GetDirectoryName (testAssemblyFullPath)).Return (testAssemblyDirectoryName);
      var taskItem = new TaskItem (testAssemblyFileName);
      taskItem.SetMetadata ("TestingConfiguration", "Chrome+SqlServer2014+x64+dockerNet45+release");
      var task = new BuildTestOutputFiles (pathStub)
                 {
                     Input = new ITaskItem[] { taskItem }
                 };

      task.Execute();

      Assert.That (task.Output.Single().GetMetadata (TestingConfigurationMetadata.TestAssemblyFullPath), Is.EqualTo (testAssemblyFullPath));
    }

    [Test]
    public void TestAssemblyDirectoryName_IsMetadata ()
    {
      const string testAssemblyFileName = "MyTest.dll";
      const string testAssemblyDirectoryName = "Development";
      var testAssemblyFullPath = $"C:\\{testAssemblyDirectoryName}\\{testAssemblyFileName}";
      var pathStub = MockRepository.Mock<IPath>();
      pathStub.Stub (_ => _.GetFullPath (testAssemblyFileName)).Return (testAssemblyFullPath);
      pathStub.Stub (_ => _.GetFileName (testAssemblyFileName)).Return (testAssemblyFileName);
      pathStub.Stub (_ => _.GetDirectoryName (testAssemblyFullPath)).Return (testAssemblyDirectoryName);
      var taskItem = new TaskItem (testAssemblyFileName);
      taskItem.SetMetadata ("TestingConfiguration", "Chrome+SqlServer2014+x64+dockerNet45+release");
      var task = new BuildTestOutputFiles (pathStub)
                 {
                     Input = new ITaskItem[] { taskItem }
                 };

      task.Execute();

      Assert.That (task.Output.Single().GetMetadata (TestingConfigurationMetadata.TestAssemblyDirectoryName), Is.EqualTo (testAssemblyDirectoryName));
    }

    [Test]
    public void TestingSetupBuildFile_IsMetadata ()
    {
      var taskItem = new TaskItem ("Tests.dll");
      taskItem.SetMetadata ("TestingConfiguration", "Chrome+SqlServer2014+x64+dockerNet45+release");
      const string testingSetupBuildFile = "MyTestingSetupBuildFile";
      taskItem.SetMetadata ("TestingSetupBuildFile", testingSetupBuildFile);
      var task = new BuildTestOutputFiles { Input = new ITaskItem[] { taskItem } };

      task.Execute();

      Assert.That (task.Output.Single().GetMetadata (TestingConfigurationMetadata.TestingSetupBuildFile), Is.EqualTo (testingSetupBuildFile));
    }
  }
}