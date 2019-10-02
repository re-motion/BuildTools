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
using System.IO.Abstractions;
using System.Text.RegularExpressions;
using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;

namespace Remotion.BuildTools.MSBuildTasks
{
  public class BuildTestOutputFiles : Task
  {
    private readonly IPath _pathHelper;

    [Required]
    public ITaskItem[] Input { get; set; }

    [Output]
    public ITaskItem[] Output { get; set; }

    public BuildTestOutputFiles ()
    {
      _pathHelper = new PathWrapper (new FileSystem());
    }

    public BuildTestOutputFiles (IPath pathHelper)
    {
      _pathHelper = pathHelper;
    }

    public override bool Execute ()
    {
      var output = new List<ITaskItem>();
      foreach (var item in Input)
      {
        var testingConfiguration = item.GetMetadata ("TestingConfiguration");

        var configurations = Regex.Replace (testingConfiguration, @"\s+", "").Split (new[]{";"}, StringSplitOptions.RemoveEmptyEntries);
        foreach (var configuration in configurations)
        {
          var newItem = CreateTaskItem (item.ItemSpec, configuration);
          output.Add (newItem);
        }
      }

      Output = output.ToArray();
      return true;
    }

    private ITaskItem CreateTaskItem (string originalItemSpec, string unsplitConfiguration)
    {
      var testAssemblyFileName = _pathHelper.GetFileName (originalItemSpec);
      var testingConfigurationItem = new TaskItem (testAssemblyFileName + "_" + unsplitConfiguration);
      testingConfigurationItem.SetMetadata (TestingConfigurationMetadata.TestAssemblyFileName, testAssemblyFileName);

      var testAssemblyFullPath = _pathHelper.GetFullPath (originalItemSpec);
      testingConfigurationItem.SetMetadata (TestingConfigurationMetadata.TestAssemblyFullPath, testAssemblyFullPath);

      testingConfigurationItem.SetMetadata (TestingConfigurationMetadata.TestAssemblyDirectoryName, _pathHelper.GetDirectoryName (testAssemblyFullPath));

      var configurationItems = Regex.Replace (unsplitConfiguration, @"\s+", "").Split ('+');

      var browser = configurationItems[0];
      testingConfigurationItem.SetMetadata (TestingConfigurationMetadata.Browser, browser);

      var isWebTest = string.Equals (browser, EmptyMetadataID.Browser, StringComparison.OrdinalIgnoreCase) ? "False" : "True";
      testingConfigurationItem.SetMetadata (TestingConfigurationMetadata.IsWebTest, isWebTest);

      var databaseSystem = configurationItems[1];
      testingConfigurationItem.SetMetadata (TestingConfigurationMetadata.DatabaseSystem, databaseSystem);

      var isDatabaseTest = string.Equals (databaseSystem, EmptyMetadataID.DatabaseSystem, StringComparison.OrdinalIgnoreCase) ? "False" : "True";
      testingConfigurationItem.SetMetadata (TestingConfigurationMetadata.IsDatabaseTest, isDatabaseTest);

      var platform = configurationItems[2];
      testingConfigurationItem.SetMetadata (TestingConfigurationMetadata.Platform, platform);

      var use32Bit = string.Equals (platform, "x86", StringComparison.OrdinalIgnoreCase) ? "True" : "False";
      testingConfigurationItem.SetMetadata (TestingConfigurationMetadata.Use32Bit, use32Bit);

      testingConfigurationItem.SetMetadata (TestingConfigurationMetadata.ExecutionRuntime, configurationItems[3]);
      testingConfigurationItem.SetMetadata (TestingConfigurationMetadata.ConfigurationID, configurationItems[4]);

      return testingConfigurationItem;
    }
  }
}