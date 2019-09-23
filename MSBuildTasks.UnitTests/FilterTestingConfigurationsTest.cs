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
using System.Linq;
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
    public void ValidPlatforms_PlatformInvalid_EmptyList ()
    {
      var itemWithInvalidPlatform = CreateTestConfiguration ("ItemWithValidPlatform", "x86");
      var items = new[] { itemWithInvalidPlatform };
      var filter = CreateFilterTestingConfigurations (items, platforms: new ITaskItem[] { new TaskItem ("x64") });

      filter.Execute();

      Assert.That (filter.Output, Is.Empty);
    }

    [Test]
    public void ValidPlatforms_SomeValidPlatforms_OnlyValidOutputs ()
    {
      var itemWithValidPlatform = CreateTestConfiguration ("ItemWithValidPlatform", "x64");
      var itemWithInvalidPlatform = CreateTestConfiguration ("ItemWithInvalidPlatform", "x86");
      var items = new[] { itemWithValidPlatform, itemWithInvalidPlatform };
      var filter = CreateFilterTestingConfigurations (items, platforms: new ITaskItem[] { new TaskItem ("x64") });

      filter.Execute();

      Assert.That (filter.Output, Is.EquivalentTo (new[] { itemWithValidPlatform }));
    }

    [Test]
    public void ValidDatabaseSystems_DatabaseSystemInvalid_Empty ()
    {
      var itemWithValidDatabaseSystem = CreateTestConfiguration ("ItemWithValidDatabaseSystem", databaseSystem: "SqlServer2012");
      var items = new[] { itemWithValidDatabaseSystem };
      var filter = CreateFilterTestingConfigurations (items, databaseSystems: new ITaskItem[] { new TaskItem ("SqlServer2014") });

      filter.Execute();

      Assert.That (filter.Output, Is.Empty);
    }

    [Test]
    public void ValidBrowser_BrowserInvalid_Empty ()
    {
      var itemWithInvalidBrowser = CreateTestConfiguration ("ItemWithInvalidBrowser", browser: "Chrome");
      var items = new[] { itemWithInvalidBrowser };
      var filter = CreateFilterTestingConfigurations (items, browsers: new ITaskItem[] { new TaskItem ("Firefox") });

      filter.Execute();

      Assert.That (filter.Output, Is.Empty);
    }

    [Test]
    public void ValidBrowser_ValidBrowsersEmpty_NoBrowser ()
    {
      var itemWithBrowser = CreateTestConfiguration ("ItemWithBrowser", browser: "Chrome");
      var itemWithNoBrowser = CreateTestConfiguration ("ItemWithNoBrowser", browser: "NoBrowser");
      var items = new[] { itemWithBrowser, itemWithNoBrowser };
      var filter = CreateFilterTestingConfigurations (items, browsers: new ITaskItem[0]);

      filter.Execute();

      Assert.That (filter.Output.Single(), Is.EqualTo (itemWithNoBrowser));
    }


    [Test]
    public void ValidDatabaseSystem_ValidDatabaseSystemEmpty_NoDb ()
    {
      var itemWithDb = CreateTestConfiguration ("ItemWithDB", databaseSystem: "SqlServer2012");
      var itemWithNoDb = CreateTestConfiguration ("ItemWithNoDB", databaseSystem: "NoDb");
      var items = new[] { itemWithDb, itemWithNoDb };
      var filter = CreateFilterTestingConfigurations (items, databaseSystems: new ITaskItem[0]);

      filter.Execute();

      Assert.That (filter.Output.Single(), Is.EqualTo (itemWithNoDb));
    }

    [Test]
    public void ValidDatabaseSystem_ValidDatabaseSystemEmpty_NoDbCaseInsensitive ()
    {
      var itemWithDb = CreateTestConfiguration ("ItemWithDB", databaseSystem: "SqlServer2012");
      var itemWithNoDb = CreateTestConfiguration ("ItemWithNoDB", databaseSystem: "NoDB");
      var items = new[] { itemWithDb, itemWithNoDb };
      var filter = CreateFilterTestingConfigurations (items, databaseSystems: new ITaskItem[0]);

      filter.Execute();

      Assert.That (filter.Output.Single(), Is.EqualTo (itemWithNoDb));
    }

    [Test]
    public void FilterOutputs_LogsFilteredItems ()
    {
      var itemWithDb = CreateTestConfiguration ("ItemWithDB", databaseSystem: "SqlServer2012");
      var itemWithNoDb = CreateTestConfiguration ("ItemWithNoDB", databaseSystem: "NoDB");
      var items = new[] { itemWithDb, itemWithNoDb };
      var loggerMock = MockRepository.Mock<ITaskLogger>();
      loggerMock.Expect (
          _ => _.LogMessage (
              @"The following test configurations were ignored:
{0}",
              "ItemWithDB: Firefox, SqlServer2012, x64, dockerNet45, release"));
      var filter = CreateFilterTestingConfigurations (items, databaseSystems: new ITaskItem[0], logger: loggerMock);

      filter.Execute();

      loggerMock.VerifyAllExpectations();
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
                 ValidDatabaseSystems = databaseSystems ?? new ITaskItem[] { new TaskItem ("SqlServer2012") },
                 ValidPlatforms = platforms ?? new ITaskItem[] { new TaskItem ("x64") },
                 ValidBrowsers = browsers ?? new ITaskItem[] { new TaskItem ("Firefox") },
             };
    }
  }
}