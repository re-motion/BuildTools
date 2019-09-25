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
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using NUnit.Framework;
using Remotion.BuildTools.MSBuildTasks;
using Rhino.Mocks;

namespace BuildTools.MSBuildTasks.UnitTests
{
  [TestFixture]
  public class FilterTestingConfigurationsTest
  {
    [Test]
    public void AllValid_SameList ()
    {
      var validItem = CreateTestConfiguration ("ValidItem", "x64", "SqlServer2012", "Firefox");
      var items = new[] { validItem };
      var filter = CreateFilterTestingConfigurations (
          items,
          platforms: new ITaskItem[] { new TaskItem ("x64") },
          databaseSystems: new ITaskItem[] { new TaskItem ("SqlServer2012") },
          browsers: new ITaskItem[] { new TaskItem ("Firefox") });

      filter.Execute();

      Assert.That (filter.Output, Is.EqualTo (items));
    }

    [Test]
    public void SupportedPlatforms_PlatformNotSupported_ReturnsFalse ()
    {
      var itemWithInvalidPlatform = CreateTestConfiguration ("ItemWithInvalidPlatform", "x86");
      var items = new[] { itemWithInvalidPlatform };
      var loggerMock = MockRepository.Mock<ITaskLogger>();
      loggerMock.Expect (_ => _.LogError ("Metadata 'Platform' with value 'x86' of TestingConfiguration is not supported."));
      var filter = CreateFilterTestingConfigurations (items, platforms: new ITaskItem[] { new TaskItem ("x64") }, logger: loggerMock);

      var success = filter.Execute();

      Assert.That (success, Is.False);
      loggerMock.VerifyAllExpectations();
    }

    [Test]
    public void SupportedBrowsers_BrowserNotSupported_ReturnsFalse ()
    {
      var itemWithValidPlatform = CreateTestConfiguration ("ItemWithInvalidBrowser", browser: "Safari");
      var items = new[] { itemWithValidPlatform };
      var loggerMock = MockRepository.Mock<ITaskLogger>();
      loggerMock.Expect (_ => _.LogError ("Metadata 'Browser' with value 'Safari' of TestingConfiguration is not supported."));
      var filter = CreateFilterTestingConfigurations (items, browsers: new ITaskItem[] { new TaskItem ("Chrome") }, logger: loggerMock);

      var success = filter.Execute();

      Assert.That (success, Is.False);
      loggerMock.VerifyAllExpectations();
    }

    [Test]
    public void SupportedDatabaseSystems_DatabaseSystemNotSupported_ReturnsFalse ()
    {
      var itemWithDb = CreateTestConfiguration ("ItemWithDB", databaseSystem: "SqlServer2012");
      var items = new[] { itemWithDb };
      var loggerMock = MockRepository.Mock<ITaskLogger>();
      loggerMock.Expect (_ => _.LogError ("Metadata 'DatabaseSystem' with value 'SqlServer2012' of TestingConfiguration is not supported."));
      var filter = CreateFilterTestingConfigurations (items, databaseSystems: new ITaskItem[] { new TaskItem ("SqlServer2016"), }, logger: loggerMock);

      var success = filter.Execute();

      Assert.That (success, Is.False);
      loggerMock.VerifyExpectations();
    }

    [Test]
    public void SupportedPlatforms_IgnoresCase ()
    {
      var validItem = CreateTestConfiguration ("ValidItem", platform: "X64");
      var items = new[] { validItem };
      var filter = CreateFilterTestingConfigurations (items, platforms: new ITaskItem[] { new TaskItem ("x64") });

      filter.Execute();

      Assert.That (filter.Output, Is.EqualTo (items));
    }

    [Test]
    public void SupportedDatabaseSystems_IgnoresCase ()
    {
      var validItem = CreateTestConfiguration ("ValidItem", databaseSystem: "sqlserver2012");
      var items = new[] { validItem };
      var filter = CreateFilterTestingConfigurations (items, databaseSystems: new ITaskItem[] { new TaskItem ("SqlServer2012") });

      filter.Execute();

      Assert.That (filter.Output, Is.EqualTo (items));
    }

    [Test]
    public void SupportedBrowsers_IgnoresCase ()
    {
      var validItem = CreateTestConfiguration ("ValidItem", browser: "chrome");
      var items = new[] { validItem };
      var filter = CreateFilterTestingConfigurations (items, browsers: new ITaskItem[] { new TaskItem ("Chrome") });

      filter.Execute();

      Assert.That (filter.Output, Is.EqualTo (items));
    }

    private ITaskItem CreateTestConfiguration (string name, string platform = null, string databaseSystem = null, string browser = null)
    {
      var item = new TaskItem (name);
      item.SetMetadata (TestingConfigurationMetadata.Platform, platform ?? "x64");
      item.SetMetadata (TestingConfigurationMetadata.DatabaseSystem, databaseSystem ?? "SqlServer2012");
      item.SetMetadata (TestingConfigurationMetadata.Browser, browser ?? "Firefox");
      item.SetMetadata (TestingConfigurationMetadata.ExecutionRuntime, "dockerNet45");
      item.SetMetadata (TestingConfigurationMetadata.ConfigurationID, "release");
      return item;
    }

    private FilterTestingConfigurations CreateFilterTestingConfigurations (
        ITaskItem[] input,
        ITaskItem[] platforms = null,
        ITaskItem[] databaseSystems = null,
        ITaskItem[] browsers = null,
        ITaskLogger logger = null)
    {
      return new FilterTestingConfigurations (logger ?? MockRepository.Mock<ITaskLogger>())
             {
                 Input = input,
                 SupportedDatabaseSystems = databaseSystems ?? new ITaskItem[] { new TaskItem ("SqlServer2012") },
                 SupportedPlatforms = platforms ?? new ITaskItem[] { new TaskItem ("x64") },
                 SupportedBrowsers = browsers ?? new ITaskItem[] { new TaskItem ("Firefox") },
             };
    }
  }
}