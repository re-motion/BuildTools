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

using Microsoft.Build.Utilities;
using NUnit.Framework;
using Remotion.BuildTools.MSBuildTasks;

namespace BuildTools.MSBuildTasks.UnitTests
{
  [TestFixture]
  public class FilterTestingConfigurationsTest
  {
    [Test]
    public void ValidPlatforms_PlatformValid_SameList ()
    {
      var itemWithValidPlatform = new TaskItem ("ItemWithValidPlatform");
      itemWithValidPlatform.SetMetadata ("Platform", "x64");
      var items = new[] { itemWithValidPlatform };

      var filter = new FilterTestingConfigurations { Input = items, ValidPlatforms = new[] { new TaskItem ("x64") } };

      filter.Execute();

      Assert.That (filter.Output, Is.EqualTo (items));
    }

    [Test]
    public void ValidPlatforms_PlatformInvalid_EmptyList ()
    {
      var itemWithInvalidPlatform = new TaskItem ("ItemWithValidPlatform");
      itemWithInvalidPlatform.SetMetadata ("Platform", "x86");
      var items = new[] { itemWithInvalidPlatform };

      var filter = new FilterTestingConfigurations { Input = items, ValidPlatforms = new[] { new TaskItem ("x64") } };

      filter.Execute();

      Assert.That (filter.Output, Is.Empty);
    }

    [Test]
    public void ValidPlatforms_SomeValidPlatforms_OnlyValidOutputs ()
    {
      var itemWithValidPlatform = new TaskItem ("ItemWithValidPlatform");
      itemWithValidPlatform.SetMetadata ("Platform", "x64");
      var itemWithInvalidPlatform = new TaskItem ("ItemWithValidPlatform");
      itemWithInvalidPlatform.SetMetadata ("Platform", "x86");
      var items = new[] { itemWithValidPlatform, itemWithInvalidPlatform };

      var filter = new FilterTestingConfigurations { Input = items, ValidPlatforms = new[] { new TaskItem ("x64") } };

      filter.Execute();

      Assert.That (filter.Output, Is.EquivalentTo (new[] { itemWithValidPlatform }));
    }
  }
}