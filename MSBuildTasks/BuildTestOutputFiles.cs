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
using System.Collections.Generic;
using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;

namespace Remotion.BuildTools.MSBuildTasks
{
  public class BuildTestOutputFiles : Task
  {
    [Required]
    public ITaskItem[] Input { get; set; }

    [Output]
    public ITaskItem[] Output { get; set; }

    public override bool Execute ()
    {
      var output = new List<ITaskItem>();
      foreach (var item in Input)
      {
        var testingConfiguration = item.GetMetadata ("TestingConfiguration");

        var configurations = testingConfiguration.Split (';');
        var configurationIDIndex = 0;
        foreach (var configuration in configurations)
        {
          var splitConfigurationItems = configuration.Split ('+');
          var newItem = CreateTaskItem (item, configurationIDIndex, splitConfigurationItems);
          output.Add (newItem);
          configurationIDIndex++;
        }
      }

      Output = output.ToArray();
      return true;
    }

    private ITaskItem CreateTaskItem (ITaskItem originalItem, int configurationIDIndex, IReadOnlyList<string> configurationItems)
    {
      var item = new TaskItem (originalItem.ItemSpec);
      item.SetMetadata (TestingConfigurationMetadata.ID, $"{originalItem.ItemSpec}_{configurationIDIndex}");

      var browser = configurationItems[0];
      item.SetMetadata (TestingConfigurationMetadata.Browser, browser);

      var isWebTest = browser == "NoBrowser" ? "False" : "True";
      item.SetMetadata (TestingConfigurationMetadata.IsWebTest, isWebTest);

      var databaseSystem = configurationItems[1];
      item.SetMetadata (TestingConfigurationMetadata.DatabaseSystem, databaseSystem);

      var isDatabaseTest = databaseSystem == "NoDb" ? "False" : "True";
      item.SetMetadata (TestingConfigurationMetadata.IsDatabaseTest, isDatabaseTest);

      var platform = configurationItems[2];
      item.SetMetadata (TestingConfigurationMetadata.Platform, platform);

      var use32Bit = platform == "x86" ? "True" : "False";
      item.SetMetadata (TestingConfigurationMetadata.Use32Bit, use32Bit);

      item.SetMetadata (TestingConfigurationMetadata.ExecutionRuntime, configurationItems[3]);
      item.SetMetadata (TestingConfigurationMetadata.ConfigurationID, configurationItems[4]);

      return item;
    }
  }
}