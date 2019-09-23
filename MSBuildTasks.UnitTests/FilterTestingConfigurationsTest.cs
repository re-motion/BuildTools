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

using System.Linq;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using NUnit.Framework;
using Remotion.BuildTools.MSBuildTasks;

namespace BuildTools.MSBuildTasks.UnitTests
{
  [TestFixture]
  public class FilterTestingConfigurationsTest
  {
    [Test]
    public void AllValid_SameList ()
    {
      var validItem = CreateTestConfiguration ("ValidItem", "x64", "SqlServer2012");
      var items = new[] { validItem };
      var filter = new FilterTestingConfigurations
                   {
                       Input = items,
                       ValidPlatforms = new[] { new TaskItem ("x64") },
                       ValidDatabaseSystems = new[] { new TaskItem ("SqlServer2012") },
                       ValidBrowsers = new[] { new TaskItem ("Firefox") }
                   };

      filter.Execute();

      Assert.That (filter.Output, Is.EqualTo (items));
    }

    [Test]
    public void ValidPlatforms_PlatformInvalid_EmptyList ()
    {
      var itemWithInvalidPlatform = CreateTestConfiguration ("ItemWithValidPlatform", "x86");
      var items = new[] { itemWithInvalidPlatform };
      var filter = new FilterTestingConfigurations { Input = items, ValidPlatforms = new[] { new TaskItem ("x64") } };

      filter.Execute();

      Assert.That (filter.Output, Is.Empty);
    }

    [Test]
    public void ValidPlatforms_SomeValidPlatforms_OnlyValidOutputs ()
    {
      var itemWithValidPlatform = CreateTestConfiguration ("ItemWithValidPlatform", "x64");
      var itemWithInvalidPlatform = CreateTestConfiguration ("ItemWithInvalidPlatform", "x86");
      var items = new[] { itemWithValidPlatform, itemWithInvalidPlatform };

      var filter = new FilterTestingConfigurations
                   {
                       Input = items,
                       ValidPlatforms = new[] { new TaskItem ("x64") },
                       ValidDatabaseSystems = new[] { new TaskItem ("SqlServer2012") },
                       ValidBrowsers = new[] { new TaskItem ("Firefox") }
                   };

      filter.Execute();

      Assert.That (filter.Output, Is.EquivalentTo (new[] { itemWithValidPlatform }));
    }

    [Test]
    public void ValidDatabaseSystems_DatabaseSystemInvalid_Empty ()
    {
      var itemWithValidDatabaseSystem = CreateTestConfiguration ("ItemWithValidDatabaseSystem", databaseSystem: "SqlServer2012");
      var items = new[] { itemWithValidDatabaseSystem };
      var filter = new FilterTestingConfigurations
                   {
                       Input = items,
                       ValidDatabaseSystems = new[] { new TaskItem ("SqlServer2014") },
                       ValidPlatforms = new[] { new TaskItem ("x64") },
                       ValidBrowsers = new[] { new TaskItem ("Firefox") }
                   };

      filter.Execute();

      Assert.That (filter.Output, Is.Empty);
    }

    [Test]
    public void ValidBrowser_BrowserInvalid_Empty ()
    {
      var itemWithInvalidBrowser = CreateTestConfiguration ("ItemWithInvalidBrowser", browser: "Chrome");
      var items = new[] { itemWithInvalidBrowser };
      var filter = new FilterTestingConfigurations
                   {
                       Input = items,
                       ValidDatabaseSystems = new[] { new TaskItem ("SqlServer2012") },
                       ValidPlatforms = new[] { new TaskItem ("x64") },
                       ValidBrowsers = new[] { new TaskItem ("Firefox") }
                   };

      filter.Execute();

      Assert.That (filter.Output, Is.Empty);
    }

    [Test]
    public void ValidBrowser_ValidBrowsersEmpty_NoBrowser ()
    {
      var itemWithBrowser = CreateTestConfiguration ("ItemWithBrowser", browser: "Chrome");
      var itemWithNoBrowser = CreateTestConfiguration ("ItemWithNoBrowser", browser: "NoBrowser");
      var items = new[] { itemWithBrowser, itemWithNoBrowser };
      var filter = new FilterTestingConfigurations
                   {
                       Input = items,
                       ValidDatabaseSystems = new[] { new TaskItem ("SqlServer2012") },
                       ValidPlatforms = new[] { new TaskItem ("x64") },
                       ValidBrowsers = new ITaskItem[0]
                   };

      filter.Execute();

      Assert.That (filter.Output.Single(), Is.EqualTo (itemWithNoBrowser));
    }

    private ITaskItem CreateTestConfiguration (string name, string platform = null, string databaseSystem = null, string browser = null)
    {
      var item = new TaskItem (name);
      item.SetMetadata (TestingConfigurationMetadata.Platform, platform ?? "x64");
      item.SetMetadata (TestingConfigurationMetadata.DatabaseSystem, databaseSystem ?? "SqlServer2012");
      item.SetMetadata (TestingConfigurationMetadata.Browser, browser ?? "Firefox");
      return item;
    }
  }
}