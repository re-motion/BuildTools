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
      taskItem.SetMetadata ("TestingConfiguration", "Chrome+SqlServer2014+x64+Win_NET46+release+net45");
      var items = new ITaskItem[] { taskItem };
      var task = CreateBuildTestOutputFiles (items);

      task.Execute();

      Assert.That (task.Output.Single().GetMetadata (TestingConfigurationMetadata.Browser), Is.EqualTo ("Chrome"));
    }

    [Test]
    public void ValidConfiguration_CorrectDatabase ()
    {
      var taskItem = new TaskItem ("MyTest.dll");
      taskItem.SetMetadata ("TestingConfiguration", "Chrome+SqlServer2014+x64+Win_NET46+release+net45");
      var items = new ITaskItem[] { taskItem };
      var task = CreateBuildTestOutputFiles (items);

      task.Execute();

      Assert.That (task.Output.Single().GetMetadata (TestingConfigurationMetadata.DatabaseSystem), Is.EqualTo ("SqlServer2014"));
    }

    [Test]
    public void ValidConfiguration_CorrectPlatform ()
    {
      var taskItem = new TaskItem ("MyTest.dll");
      taskItem.SetMetadata ("TestingConfiguration", "Chrome+SqlServer2014+x64+Win_NET46+release+net45");
      var items = new ITaskItem[] { taskItem };
      var task = CreateBuildTestOutputFiles (items);

      task.Execute();

      Assert.That (task.Output.Single().GetMetadata (TestingConfigurationMetadata.Platform), Is.EqualTo ("x64"));
    }

    [Test]
    public void ValidConfiguration_CorrectBuildConfiguration ()
    {
      var taskItem = new TaskItem ("MyTest.dll");
      taskItem.SetMetadata ("TestingConfiguration", "Chrome+SqlServer2014+x64+Win_NET46+release+net45");
      var items = new ITaskItem[] { taskItem };
      var task = CreateBuildTestOutputFiles (items);

      task.Execute();

      Assert.That (task.Output.Single().GetMetadata (TestingConfigurationMetadata.ConfigurationID), Is.EqualTo ("release"));
    }

    [Test]
    public void MultipleConfigurations_CorrectParsing ()
    {
      var taskItem = new TaskItem ("MyTest.dll");
      taskItem.SetMetadata ("TestingConfiguration", "Chrome+SqlServer2014+x64+Win_NET46+release+net45;Firefox+SqlServer2014+x64+Win_NET46+release+net45");
      var items = new ITaskItem[] { taskItem };
      var supportedBrowsers = new ITaskItem[] { new TaskItem ("Chrome"), new TaskItem ("Firefox") };
      var task = CreateBuildTestOutputFiles (items, supportedBrowsers: supportedBrowsers);
      task.Execute();

      Assert.That (task.Output[1].GetMetadata (TestingConfigurationMetadata.Browser), Is.EqualTo ("Firefox"));
      Assert.That (task.Output[1].GetMetadata (TestingConfigurationMetadata.DatabaseSystem), Is.EqualTo ("SqlServer2014"));
      Assert.That (task.Output[1].GetMetadata (TestingConfigurationMetadata.Platform), Is.EqualTo ("x64"));
      Assert.That (task.Output[1].GetMetadata (TestingConfigurationMetadata.ExecutionRuntime), Is.EqualTo ("DockerImageName"));
      Assert.That (task.Output[1].GetMetadata (TestingConfigurationMetadata.ConfigurationID), Is.EqualTo ("release"));
    }

    [Test]
    public void ConfigurationsWithTrailingWhitespaces_CorrectParsing ()
    {
      var taskItem = new TaskItem ("MyTest.dll");
      var config = $"{Environment.NewLine}    Chrome+SqlServer2014+x64+Win_NET46+release+net45{Environment.NewLine}    ";
      taskItem.SetMetadata ("TestingConfiguration", config);
      var items = new ITaskItem[] { taskItem };
      var task = CreateBuildTestOutputFiles (items);

      task.Execute();

      Assert.That (task.Output.Single().GetMetadata (TestingConfigurationMetadata.Browser), Is.EqualTo ("Chrome"));
      Assert.That (task.Output.Single().GetMetadata (TestingConfigurationMetadata.DatabaseSystem), Is.EqualTo ("SqlServer2014"));
      Assert.That (task.Output.Single().GetMetadata (TestingConfigurationMetadata.Platform), Is.EqualTo ("x64"));
      Assert.That (task.Output.Single().GetMetadata (TestingConfigurationMetadata.ExecutionRuntime), Is.EqualTo ("DockerImageName"));
      Assert.That (task.Output.Single().GetMetadata (TestingConfigurationMetadata.ConfigurationID), Is.EqualTo ("release"));
    }

    [Test]
    public void ConfigurationOrder_Irrelevant ()
    {
      var taskItem = new TaskItem ("MyTest.dll");
      const string config = "debug+Firefox+Win_NET46+SqlServer2012+x64+net45";
      taskItem.SetMetadata ("TestingConfiguration", config);
      var items = new ITaskItem[] { taskItem };

      var task = CreateBuildTestOutputFiles (
          items,
          new ITaskItem[] { new TaskItem ("x64") },
          new ITaskItem[] { new TaskItem ("SqlServer2012") },
          new ITaskItem[] { new TaskItem ("Firefox") },
          new ITaskItem[] { new TaskItem ("Win_NET46:DockerImageName") },
          new ITaskItem[] { new TaskItem ("debug") },
          new ITaskItem[] { new TaskItem ("NET45") });

      task.Execute();

      Assert.That (task.Output.Single().GetMetadata (TestingConfigurationMetadata.Browser), Is.EqualTo ("Firefox"));
      Assert.That (task.Output.Single().GetMetadata (TestingConfigurationMetadata.DatabaseSystem), Is.EqualTo ("SqlServer2012"));
      Assert.That (task.Output.Single().GetMetadata (TestingConfigurationMetadata.Platform), Is.EqualTo ("x64"));
      Assert.That (task.Output.Single().GetMetadata (TestingConfigurationMetadata.ExecutionRuntime), Is.EqualTo ("DockerImageName"));
      Assert.That (task.Output.Single().GetMetadata (TestingConfigurationMetadata.ConfigurationID), Is.EqualTo ("debug"));
    }

    [Test]
    public void ConfigurationOrder_DoubleConfiguration_Error ()
    {
      var taskItem = new TaskItem ("MyTest.dll");
      const string config = "debug+debug+Win_NET46+SqlServer2012+x64+net45";
      taskItem.SetMetadata ("TestingConfiguration", config);
      var loggerMock = MockRepository.Mock<ITaskLogger>();
      loggerMock.Expect (_ => _.LogError ("The following configuration values were found multiple times: '{0}'", "debug")).Repeat.Once();

      var items = new ITaskItem[] { taskItem };
      var supportedDatabaseSystems = new ITaskItem[] { new TaskItem ("SqlServer2012") };
      var supportedBrowsers = new ITaskItem[] { new TaskItem ("Firefox") };
      var supportedPlatforms = new ITaskItem[] { new TaskItem ("x64") };
      var supportedConfigurationIDs = new ITaskItem[] { new TaskItem ("debug") };
      var supportedExecutionRuntimes = new ITaskItem[] { new TaskItem ("Win_NET46") };

      var task = CreateBuildTestOutputFiles (
          items,
          supportedDatabaseSystems: supportedDatabaseSystems,
          supportedBrowsers: supportedBrowsers,
          supportedPlatforms: supportedPlatforms,
          supportedConfigurationIDs: supportedConfigurationIDs,
          supportedExecutionruntimes: supportedExecutionRuntimes,
          logger: loggerMock);

      var result = task.Execute();

      Assert.That (result, Is.False);
      loggerMock.VerifyAllExpectations();
    }

    [Test]
    public void MultipleConfigurationsWithTrailingWhitespaces_CorrectParsing ()
    {
      var taskItem = new TaskItem ("MyTest.dll");
      var config =
          $"{Environment.NewLine}    Chrome+SqlServer2014+x64+Win_NET46+release+net45;  {Environment.NewLine}  Firefox+SqlServer2014+x64+Win_NET46+release+net45{Environment.NewLine}    ";
      taskItem.SetMetadata ("TestingConfiguration", config);
      var items = new ITaskItem[] { taskItem };
      var supportedBrowsers = new ITaskItem[] { new TaskItem ("Chrome"), new TaskItem ("Firefox") };
      var task = CreateBuildTestOutputFiles (items, supportedBrowsers: supportedBrowsers);

      task.Execute();

      Assert.That (task.Output[1].GetMetadata (TestingConfigurationMetadata.Browser), Is.EqualTo ("Firefox"));
      Assert.That (task.Output[1].GetMetadata (TestingConfigurationMetadata.DatabaseSystem), Is.EqualTo ("SqlServer2014"));
      Assert.That (task.Output[1].GetMetadata (TestingConfigurationMetadata.Platform), Is.EqualTo ("x64"));
      Assert.That (task.Output[1].GetMetadata (TestingConfigurationMetadata.ExecutionRuntime), Is.EqualTo ("DockerImageName"));
      Assert.That (task.Output[1].GetMetadata (TestingConfigurationMetadata.ConfigurationID), Is.EqualTo ("release"));
    }

    [Test]
    public void MultipleConfigurationsWithTrailingSemiColon_CorrectParsing ()
    {
      var taskItem = new TaskItem ("MyTest.dll");
      const string config = "Chrome+SqlServer2014+x64+Win_NET46+release+net45;Firefox+SqlServer2014+x64+Win_NET46+release+net45;";
      taskItem.SetMetadata ("TestingConfiguration", config);
      var supportedBrowsers = new ITaskItem[] { new TaskItem ("Chrome"), new TaskItem ("Firefox") };
      var items = new ITaskItem[] { taskItem };
      var task = CreateBuildTestOutputFiles (items, supportedBrowsers: supportedBrowsers);

      task.Execute();

      Assert.That (task.Output[1].GetMetadata (TestingConfigurationMetadata.Browser), Is.EqualTo ("Firefox"));
      Assert.That (task.Output[1].GetMetadata (TestingConfigurationMetadata.DatabaseSystem), Is.EqualTo ("SqlServer2014"));
      Assert.That (task.Output[1].GetMetadata (TestingConfigurationMetadata.Platform), Is.EqualTo ("x64"));
      Assert.That (task.Output[1].GetMetadata (TestingConfigurationMetadata.ExecutionRuntime), Is.EqualTo ("DockerImageName"));
      Assert.That (task.Output[1].GetMetadata (TestingConfigurationMetadata.ConfigurationID), Is.EqualTo ("release"));
    }

    [Test]
    public void ItemSpec_IsAssemblyNameAndConfiguration ()
    {
      const string testAssemblyFileName = "MyTest.dll";
      const string testAssemblyDirectoryName = "Development";
      var testAssemblyFullPath = $"C:\\{testAssemblyDirectoryName}\\{testAssemblyFileName}";
      var pathStub = MockRepository.Mock<IPath>();
      pathStub.Stub (_ => _.GetFileName (testAssemblyFullPath)).Return (testAssemblyFileName);
      pathStub.Stub (_ => _.GetDirectoryName (testAssemblyFullPath)).Return (testAssemblyDirectoryName);
      var taskItem = new TaskItem (testAssemblyFullPath);
      const string config = "Chrome+SqlServer2014+x64+Win_NET46+release+net45";
      taskItem.SetMetadata ("TestingConfiguration", config);
      var items = new ITaskItem[] { taskItem };
      var task = CreateBuildTestOutputFiles (items, pathHelper: pathStub);

      task.Execute();

      Assert.That (task.Output.Single().ItemSpec, Is.EqualTo (testAssemblyFileName + "_" + config));
    }

    [Test]
    public void ItemSpecWithFullPath_IsAssemblyNameAndConfiguration ()
    {
      const string testAssemblyFileName = "MyTest.dll";
      const string testAssemblyDirectoryName = "Development";
      var testAssemblyFullPath = $"C:\\{testAssemblyDirectoryName}\\{testAssemblyFileName}";
      var pathStub = MockRepository.Mock<IPath>();
      pathStub.Stub (_ => _.GetFileName (testAssemblyFullPath)).Return (testAssemblyFileName);
      pathStub.Stub (_ => _.GetFullPath (testAssemblyFileName)).Return (testAssemblyFullPath);
      pathStub.Stub (_ => _.GetDirectoryName (testAssemblyFullPath)).Return (testAssemblyDirectoryName);

      var taskItem = new TaskItem (testAssemblyFullPath);
      const string config = "Chrome+SqlServer2014+x64+Win_NET46+release+net45";
      taskItem.SetMetadata ("TestingConfiguration", config);
      var items = new ITaskItem[] { taskItem };
      var task = CreateBuildTestOutputFiles (items, pathHelper: pathStub);

      task.Execute();

      Assert.That (task.Output.Single().ItemSpec, Is.EqualTo (testAssemblyFileName + "_" + config));
    }

    [Test]
    public void ValidConfiguration_CopiesMultipleIdentifiers ()
    {
      const string testAssemblyFileName = "MyTest.dll";
      const string testAssemblyDirectoryName = "Development";
      var testAssemblyFullPath = $"C:\\{testAssemblyDirectoryName}\\{testAssemblyFileName}";
      var pathStub = MockRepository.Mock<IPath>();
      pathStub.Stub (_ => _.GetFileName (testAssemblyFullPath)).Return (testAssemblyFileName);
      pathStub.Stub (_ => _.GetDirectoryName (testAssemblyFullPath)).Return (testAssemblyDirectoryName);
      var taskItem = new TaskItem (testAssemblyFullPath);
      const string config1 = "Chrome+SqlServer2014+x86+Win_NET46+release+net45";
      const string config2 = "Firefox+SqlServer2012+x64+Win_NET46+debug+net45";
      taskItem.SetMetadata ("TestingConfiguration", config1 + ";" + config2);
      var items = new ITaskItem[] { taskItem };
      var supportedBrowsers = new ITaskItem[] { new TaskItem ("Firefox"), new TaskItem ("Chrome") };
      var supportedDatabaseSystems = new ITaskItem[] { new TaskItem ("SqlServer2012"), new TaskItem ("SqlServer2014") };
      var supportedPlatforms = new ITaskItem[] { new TaskItem ("x64"), new TaskItem ("x86") };
      var supportedConfigurationIDs = new ITaskItem[] { new TaskItem ("release"), new TaskItem ("debug") };
      var task = CreateBuildTestOutputFiles (
          items,
          supportedDatabaseSystems: supportedDatabaseSystems,
          supportedBrowsers: supportedBrowsers,
          supportedPlatforms: supportedPlatforms,
          supportedConfigurationIDs: supportedConfigurationIDs,
          pathHelper: pathStub);

      task.Execute();

      Assert.That (task.Output[1].ItemSpec, Is.EqualTo (testAssemblyFileName + "_" + config2));
    }

    [Test]
    public void Use32Bit_x86_True ()
    {
      var taskItem = new TaskItem ("MyTest.dll");
      taskItem.SetMetadata ("TestingConfiguration", "Chrome+SqlServer2014+x86+Win_NET46+release+net45");
      var items = new ITaskItem[] { taskItem };
      var task = CreateBuildTestOutputFiles (items);

      task.Execute();

      Assert.That (task.Output.Single().GetMetadata (TestingConfigurationMetadata.Use32Bit), Is.EqualTo ("True"));
    }

    [Test]
    public void Use32Bit_x86CaseInsensitive_True ()
    {
      var taskItem = new TaskItem ("MyTest.dll");
      taskItem.SetMetadata ("TestingConfiguration", "Chrome+SqlServer2014+X86+Win_NET46+release+net45");
      var items = new ITaskItem[] { taskItem };
      var task = CreateBuildTestOutputFiles (items);
      task.Execute();

      Assert.That (task.Output.Single().GetMetadata (TestingConfigurationMetadata.Use32Bit), Is.EqualTo ("True"));
    }

    [Test]
    public void Use32Bit_x64_False ()
    {
      var taskItem = new TaskItem ("MyTest.dll");
      taskItem.SetMetadata ("TestingConfiguration", "Chrome+SqlServer2014+x64+Win_NET46+release+net45");
      var items = new ITaskItem[] { taskItem };
      var task = CreateBuildTestOutputFiles (items);

      task.Execute();

      Assert.That (task.Output.Single().GetMetadata (TestingConfigurationMetadata.Use32Bit), Is.EqualTo ("False"));
    }

    [Test]
    public void IsDatabaseTest_NoDb_False ()
    {
      var taskItem = new TaskItem ("MyTest.dll");
      taskItem.SetMetadata ("TestingConfiguration", "Chrome+NoDb+x64+Win_NET46+release+net45");
      var items = new ITaskItem[] { taskItem };
      var task = CreateBuildTestOutputFiles (items);

      task.Execute();

      Assert.That (task.Output.Single().GetMetadata (TestingConfigurationMetadata.IsDatabaseTest), Is.EqualTo ("False"));
    }

    [Test]
    public void IsDatabaseTest_NotNoDb_True ()
    {
      var taskItem = new TaskItem ("MyTest.dll");
      taskItem.SetMetadata ("TestingConfiguration", "Chrome+SqlServer2014+x64+Win_NET46+release+net45");
      var items = new ITaskItem[] { taskItem };
      var task = CreateBuildTestOutputFiles (items);

      task.Execute();

      Assert.That (task.Output.Single().GetMetadata (TestingConfigurationMetadata.IsDatabaseTest), Is.EqualTo ("True"));
    }

    [Test]
    public void IsDatabaseTest_NoDbIgnoreCase_False ()
    {
      var taskItem = new TaskItem ("MyTest.dll");
      taskItem.SetMetadata ("TestingConfiguration", "Chrome+nodb+x64+Win_NET46+release+net45");
      var items = new ITaskItem[] { taskItem };
      var task = CreateBuildTestOutputFiles (items);

      task.Execute();

      Assert.That (task.Output.Single().GetMetadata (TestingConfigurationMetadata.IsDatabaseTest), Is.EqualTo ("False"));
    }

    [Test]
    public void IsWebTest_NoBrowser_False ()
    {
      var taskItem = new TaskItem ("MyTest.dll");
      taskItem.SetMetadata ("TestingConfiguration", "NoBrowser+SqlServer2014+x64+Win_NET46+release+net45");
      var items = new ITaskItem[] { taskItem };
      var task = CreateBuildTestOutputFiles (items);

      task.Execute();

      Assert.That (task.Output.Single().GetMetadata (TestingConfigurationMetadata.IsWebTest), Is.EqualTo ("False"));
    }

    [Test]
    public void IsWebTest_NoBrowserIgnoreCase_False ()
    {
      var taskItem = new TaskItem ("MyTest.dll");
      taskItem.SetMetadata ("TestingConfiguration", "nobrowser+SqlServer2014+x64+Win_NET46+release+net45");
      var items = new ITaskItem[] { taskItem };
      var task = CreateBuildTestOutputFiles (items);

      task.Execute();

      Assert.That (task.Output.Single().GetMetadata (TestingConfigurationMetadata.IsWebTest), Is.EqualTo ("False"));
    }

    [Test]
    public void SupportedBrowsers_NoBrowser_AlwaysSupported ()
    {
      var taskItem = new TaskItem ("MyTest.dll");
      taskItem.SetMetadata ("TestingConfiguration", "NoBrowser+SqlServer2014+x64+Win_NET46+release+net45");
      var items = new ITaskItem[] { taskItem };
      var task = CreateBuildTestOutputFiles (items, supportedBrowsers: new ITaskItem[0]);

      task.Execute();

      Assert.That (task.Output.Single().GetMetadata (TestingConfigurationMetadata.Browser), Is.EqualTo ("NoBrowser"));
    }

    [Test]
    public void SupportedDatabaseSystems_NoDB_AlwaysSupported ()
    {
      var taskItem = new TaskItem ("MyTest.dll");
      taskItem.SetMetadata ("TestingConfiguration", "Chrome+NoDB+x64+Win_NET46+release+net45");
      var items = new ITaskItem[] { taskItem };
      var task = CreateBuildTestOutputFiles (items, supportedDatabaseSystems: new ITaskItem[0]);

      task.Execute();

      Assert.That (task.Output.Single().GetMetadata (TestingConfigurationMetadata.DatabaseSystem), Is.EqualTo ("NoDB"));
    }

    [Test]
    public void IsWebTest_NotNoBrowser_True ()
    {
      var taskItem = new TaskItem ("MyTest.dll");
      taskItem.SetMetadata ("TestingConfiguration", "Chrome+SqlServer2014+x64+Win_NET46+release+net45");
      var items = new ITaskItem[] { taskItem };
      var task = CreateBuildTestOutputFiles (items);

      task.Execute();

      Assert.That (task.Output.Single().GetMetadata (TestingConfigurationMetadata.IsWebTest), Is.EqualTo ("True"));
    }

    [Test]
    public void TestAssemblyFileName_DerivesFromItemSpec ()
    {
      const string testAssemblyFileName = "MyTest.dll";
      const string testAssemblyDirectoryName = "Development";
      var testAssemblyFullPath = $"C:\\{testAssemblyDirectoryName}\\{testAssemblyFileName}";
      var pathStub = MockRepository.Mock<IPath>();
      pathStub.Stub (_ => _.GetFileName (testAssemblyFullPath)).Return (testAssemblyFileName);
      pathStub.Stub (_ => _.GetDirectoryName (testAssemblyFullPath)).Return (testAssemblyDirectoryName);
      var taskItem = new TaskItem (testAssemblyFullPath);
      taskItem.SetMetadata ("TestingConfiguration", "Chrome+SqlServer2014+x64+Win_NET46+release+net45");
      var items = new ITaskItem[] { taskItem };
      var task = CreateBuildTestOutputFiles (items, pathHelper: pathStub);

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
      taskItem.SetMetadata ("TestingConfiguration", "Chrome+SqlServer2014+x64+Win_NET46+release+net45");
      var pathStub = MockRepository.Mock<IPath>();
      pathStub.Stub (_ => _.GetFileName (testAssemblyFullPath)).Return (testAssemblyFileName);
      pathStub.Stub (_ => _.GetFullPath (testAssemblyFileName)).Return (testAssemblyFullPath);
      pathStub.Stub (_ => _.GetDirectoryName (testAssemblyFullPath)).Return (testAssemblyDirectoryName);
      var items = new ITaskItem[] { taskItem };
      var task = CreateBuildTestOutputFiles (items, pathHelper: pathStub);

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
      pathStub.Stub (_ => _.GetFileName (testAssemblyFullPath)).Return (testAssemblyFileName);
      pathStub.Stub (_ => _.GetDirectoryName (testAssemblyFullPath)).Return (testAssemblyDirectoryName);
      var taskItem = new TaskItem (testAssemblyFullPath);
      taskItem.SetMetadata ("TestingConfiguration", "Chrome+SqlServer2014+x64+Win_NET46+release+net45");
      var items = new ITaskItem[] { taskItem };

      var task = CreateBuildTestOutputFiles (items, pathHelper: pathStub);

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
      pathStub.Stub (_ => _.GetFileName (testAssemblyFullPath)).Return (testAssemblyFileName);
      pathStub.Stub (_ => _.GetDirectoryName (testAssemblyFullPath)).Return (testAssemblyDirectoryName);
      var taskItem = new TaskItem (testAssemblyFullPath);
      taskItem.SetMetadata ("TestingConfiguration", "Chrome+SqlServer2014+x64+Win_NET46+release+net45");
      var items = new ITaskItem[] { taskItem };
      var task = CreateBuildTestOutputFiles (items, pathHelper: pathStub);

      task.Execute();

      Assert.That (task.Output.Single().GetMetadata (TestingConfigurationMetadata.TestAssemblyDirectoryName), Is.EqualTo (testAssemblyDirectoryName));
    }

    [Test]
    public void TestingSetupBuildFile_IsMetadata ()
    {
      var taskItem = new TaskItem ("Tests.dll");
      taskItem.SetMetadata ("TestingConfiguration", "Chrome+SqlServer2014+x64+Win_NET46+release+net45");
      const string testingSetupBuildFile = "MyTestingSetupBuildFile";
      taskItem.SetMetadata ("TestingSetupBuildFile", testingSetupBuildFile);
      var items = new ITaskItem[] { taskItem };
      var task = CreateBuildTestOutputFiles (items);

      task.Execute();

      Assert.That (task.Output.Single().GetMetadata (TestingConfigurationMetadata.TestingSetupBuildFile), Is.EqualTo (testingSetupBuildFile));
    }

    [Test]
    public void UseDocker_ExecutionRuntimeLocalMachine_False ()
    {
      var taskItem = new TaskItem ("Tests.dll");
      taskItem.SetMetadata ("TestingConfiguration", "Chrome+SqlServer2014+x64+LocalMachine+release+net45");
      var items = new ITaskItem[] { taskItem };
      var supportedExecutionruntimes = new[] { new TaskItem ("LocalMachine") };
      var task = CreateBuildTestOutputFiles (items, supportedExecutionruntimes: supportedExecutionruntimes);

      task.Execute();

      Assert.That (task.Output.Single().GetMetadata (TestingConfigurationMetadata.UseDocker), Is.EqualTo ("False"));
    }

    [Test]
    public void UseDocker_ExecutionRuntimeLocalMachine_IsAlwaysSupported ()
    {
      var taskItem = new TaskItem ("Tests.dll");
      taskItem.SetMetadata ("TestingConfiguration", "Chrome+SqlServer2014+x64+LocalMachine+release+net45");
      var items = new ITaskItem[] { taskItem };
      var task = CreateBuildTestOutputFiles (items);

      task.Execute();

      Assert.That (task.Output.Single().GetMetadata (TestingConfigurationMetadata.UseDocker), Is.EqualTo ("False"));
    }

    [Test]
    public void ExecutionRuntime_CopiesLocalMachine ()
    {
      var taskItem = new TaskItem ("Tests.dll");
      taskItem.SetMetadata ("TestingConfiguration", "Chrome+SqlServer2014+x64+LocalMachine+release+net45");
      var items = new ITaskItem[] { taskItem };
      var task = CreateBuildTestOutputFiles (items);

      task.Execute();

      Assert.That (task.Output.Single().GetMetadata (TestingConfigurationMetadata.ExecutionRuntime), Is.EqualTo ("LocalMachine"));
    }

    [Test]
    public void UseDocker_ExecutionRuntimeLocalMachine_IsCaseInsensitive ()
    {
      var taskItem = new TaskItem ("Tests.dll");
      taskItem.SetMetadata ("TestingConfiguration", "Chrome+SqlServer2014+x64+lOcalmAchine+release+net45");
      var items = new ITaskItem[] { taskItem };
      var supportedExecutionruntimes = new[] { new TaskItem ("LocalMachine") };
      var task = CreateBuildTestOutputFiles (items, supportedExecutionruntimes: supportedExecutionruntimes);

      task.Execute();

      Assert.That (task.Output.Single().GetMetadata (TestingConfigurationMetadata.UseDocker), Is.EqualTo ("False"));
    }

    [Test]
    public void TargetFramework_IsMetadata ()
    {
      var taskItem = new TaskItem ("Tests.dll");
      taskItem.SetMetadata ("TestingConfiguration", "Chrome+SqlServer2014+x64+Win_NET46+release+NET45");
      var items = new ITaskItem[] { taskItem };
      var supportedTargetRuntimes = new[] { new TaskItem ("NET45") };
      var task = CreateBuildTestOutputFiles (items, supportedTargetRuntimes: supportedTargetRuntimes);

      task.Execute();

      Assert.That (task.Output.Single().GetMetadata (TestingConfigurationMetadata.TargetRuntime), Is.EqualTo ("NET-4.5"));
    }

    [Test]
    public void TargetFramework_IsCaseInsensitive ()
    {
      var taskItem = new TaskItem ("Tests.dll");
      taskItem.SetMetadata ("TestingConfiguration", "Chrome+SqlServer2014+x64+Win_NET46+release+nEt45");
      var items = new ITaskItem[] { taskItem };
      var supportedTargetRuntimes = new[] { new TaskItem ("net45") };
      var task = CreateBuildTestOutputFiles (items, supportedTargetRuntimes: supportedTargetRuntimes);

      task.Execute();

      Assert.That (task.Output.Single().GetMetadata (TestingConfigurationMetadata.TargetRuntime), Is.EqualTo ("NET-4.5"));
    }

    [Test]
    public void Browser_IsCaseInsensitive ()
    {
      var taskItem = new TaskItem ("Tests.dll");
      taskItem.SetMetadata ("TestingConfiguration", "chRome+SqlServer2014+x64+Win_NET46+release+NET45");
      var items = new ITaskItem[] { taskItem };
      var supportedBrowsers = new[] { new TaskItem ("Chrome") };
      var task = CreateBuildTestOutputFiles (items, supportedBrowsers: supportedBrowsers);

      task.Execute();

      Assert.That (task.Output.Single().GetMetadata (TestingConfigurationMetadata.Browser), Is.EqualTo ("Chrome"));
    }

    [Test]
    public void SupportedExecutionRuntimes_IsKeyValuePair ()
    {
      var taskItem = new TaskItem ("Tests.dll");
      const string executionRuntimeKey = "ExecutionRuntimeKey";
      const string executionRuntimeValue = "ExecutionRuntimeValue";
      taskItem.SetMetadata ("TestingConfiguration", $"Chrome+SqlServer2014+x64+{executionRuntimeKey}+release+NET45");
      var items = new ITaskItem[] { taskItem };
      var supportedExecutionRuntimes = new[] { new TaskItem ($"{executionRuntimeKey}:{executionRuntimeValue}") };
      var task = CreateBuildTestOutputFiles (items, supportedExecutionruntimes: supportedExecutionRuntimes);

      task.Execute();

      Assert.That (task.Output.Single().GetMetadata (TestingConfigurationMetadata.ExecutionRuntime), Is.EqualTo (executionRuntimeValue));
    }

    [Test]
    public void SupportedExecutionRuntimes_IgnoresCase ()
    {
      var taskItem = new TaskItem ("Tests.dll");
      const string executionRuntimeKey = "ExecutionRuntimeKey";
      const string executionRuntimeValue = "ExecutionRuntimeValue";
      taskItem.SetMetadata ("TestingConfiguration", "Chrome+SqlServer2014+x64+execuTionruntimekey+release+NET45");
      var items = new ITaskItem[] { taskItem };
      var supportedExecutionRuntimes = new[] { new TaskItem ($"{executionRuntimeKey}:{executionRuntimeValue}") };
      var task = CreateBuildTestOutputFiles (items, supportedExecutionruntimes: supportedExecutionRuntimes);

      task.Execute();

      Assert.That (task.Output.Single().GetMetadata (TestingConfigurationMetadata.ExecutionRuntime), Is.EqualTo (executionRuntimeValue));
    }

    [Test]
    public void SupportedExecutionRuntimes_ConsidersAllEntries ()
    {
      var taskItem = new TaskItem ("Tests.dll");
      const string executionRuntimeKey1 = "ExecutionRuntimeKey1";
      const string executionRuntimeValue1 = "ExecutionRuntimeValue1";
      const string executionRuntimeKey2 = "ExecutionRuntimeKey2";
      const string executionRuntimeValue2 = "ExecutionRuntimeValue2";
      taskItem.SetMetadata ("TestingConfiguration", $"Chrome+SqlServer2014+x64+{executionRuntimeKey2}+release+NET45");
      var items = new ITaskItem[] { taskItem };
      var supportedExecutionRuntimes = new[]
                                       {
                                           new TaskItem ($"{executionRuntimeKey1}:{executionRuntimeValue1}"),
                                           new TaskItem ($"{executionRuntimeKey2}:{executionRuntimeValue2}")
                                       };
      var task = CreateBuildTestOutputFiles (items, supportedExecutionruntimes: supportedExecutionRuntimes);

      task.Execute();

      Assert.That (task.Output.Single().GetMetadata (TestingConfigurationMetadata.ExecutionRuntime), Is.EqualTo (executionRuntimeValue2));
    }

    [Test]
    public void MissingBrowser_LogsError_False ()
    {
      var taskItem = new TaskItem ("MyTest.dll");
      const string config = "SqlServer2014+x64+Win_NET46+release+net45";
      taskItem.SetMetadata ("TestingConfiguration", config);
      var loggerMock = MockRepository.Mock<ITaskLogger>();
      loggerMock.Expect (_ => _.LogError ("{0} ('{1}')", "Could not find a supported browser.", config));
      var items = new ITaskItem[] { taskItem };
      var task = CreateBuildTestOutputFiles (items, logger: loggerMock);

      var result = task.Execute();

      Assert.That (result, Is.False);
      loggerMock.VerifyAllExpectations();
    }

    [Test]
    public void MissingTargetRuntime_LogsError_False ()
    {
      var taskItem = new TaskItem ("MyTest.dll");
      const string config = "SqlServer2014+x64+Win_NET46+release+Chrome";
      taskItem.SetMetadata ("TestingConfiguration", config);
      var loggerMock = MockRepository.Mock<ITaskLogger>();
      loggerMock.Expect (_ => _.LogError ("{0} ('{1}')", "Could not find a supported target runtime.", config));
      var items = new ITaskItem[] { taskItem };
      var task = CreateBuildTestOutputFiles (items, logger: loggerMock);

      var result = task.Execute();

      Assert.That (result, Is.False);
      loggerMock.VerifyAllExpectations();
    }

    [Test]
    public void MissingDatabaseSystem_LogsError_False ()
    {
      var taskItem = new TaskItem ("MyTest.dll");
      const string config = "x64+Win_NET46+release+Chrome+net45";
      taskItem.SetMetadata ("TestingConfiguration", config);
      var loggerMock = MockRepository.Mock<ITaskLogger>();
      loggerMock.Expect (_ => _.LogError ("{0} ('{1}')", "Could not find a supported database system.", config));
      var items = new ITaskItem[] { taskItem };
      var task = CreateBuildTestOutputFiles (items, logger: loggerMock);

      var result = task.Execute();

      Assert.That (result, Is.False);
      loggerMock.VerifyAllExpectations();
    }

    private BuildTestOutputFiles CreateBuildTestOutputFiles (
        ITaskItem[] input,
        ITaskItem[] supportedPlatforms = null,
        ITaskItem[] supportedDatabaseSystems = null,
        ITaskItem[] supportedBrowsers = null,
        ITaskItem[] supportedExecutionruntimes = null,
        ITaskItem[] supportedConfigurationIDs = null,
        ITaskItem[] supportedTargetRuntimes = null,
        IPath pathHelper = null,
        ITaskLogger logger = null)
    {
      var pathStub = MockRepository.Mock<IPath>();
      pathStub.Stub (_ => _.GetFileName (null)).IgnoreArguments().Return ("");
      pathStub.Stub (_ => _.GetDirectoryName (null)).IgnoreArguments().Return ("");
      var loggerStub = MockRepository.Mock<ITaskLogger>();

      return new BuildTestOutputFiles (pathHelper ?? pathStub, logger ?? loggerStub)
             {
                 Input = input,
                 SupportedDatabaseSystems = supportedDatabaseSystems ?? new ITaskItem[] { new TaskItem ("SqlServer2014") },
                 SupportedPlatforms = supportedPlatforms ?? new ITaskItem[] { new TaskItem ("x64"), new TaskItem ("x86") },
                 SupportedBrowsers = supportedBrowsers ?? new ITaskItem[] { new TaskItem ("Chrome") },
                 SupportedExecutionRuntimes = supportedExecutionruntimes ?? new ITaskItem[] { new TaskItem ("Win_NET46:DockerImageName") },
                 SupportedTargetRuntimes = supportedTargetRuntimes ?? new ITaskItem[] { new TaskItem ("NET45") },
                 SupportedConfigurationIDs = supportedConfigurationIDs ?? new ITaskItem[] { new TaskItem ("release") },
             };
    }
  }
}