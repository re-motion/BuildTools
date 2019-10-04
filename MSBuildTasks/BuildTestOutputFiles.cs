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
using System.Linq;
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

    [Required]
    public ITaskItem[] SupportedDatabaseSystems { get; set; }

    [Required]
    public ITaskItem[] SupportedPlatforms { get; set; }

    [Required]
    public ITaskItem[] SupportedBrowsers { get; set; }

    [Required]
    public ITaskItem[] SupportedConfigurationIDs { get; set; }

    [Required]
    public ITaskItem[] SupportedExecutionRuntimes { get; set; }

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
        var testingSetupBuildFile = item.GetMetadata ("TestingSetupBuildFile");

        var configurations = Regex.Replace (testingConfiguration, @"\s+", "").Split (new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
        foreach (var configuration in configurations)
        {
          var newItem = CreateTaskItem (item.ItemSpec, configuration, testingSetupBuildFile);
          output.Add (newItem);
        }
      }

      Output = output.ToArray();
      return true;
    }

    private ITaskItem CreateTaskItem (string originalItemSpec, string unsplitConfiguration, string testingSetupBuildFile)
    {
      var testAssemblyFileName = _pathHelper.GetFileName (originalItemSpec);
      var testingConfigurationItem = new TaskItem (testAssemblyFileName + "_" + unsplitConfiguration);
      testingConfigurationItem.SetMetadata (TestingConfigurationMetadata.TestAssemblyFileName, testAssemblyFileName);
      testingConfigurationItem.SetMetadata (TestingConfigurationMetadata.TestingSetupBuildFile, testingSetupBuildFile);

      var testAssemblyFullPath = _pathHelper.GetFullPath (testAssemblyFileName);
      testingConfigurationItem.SetMetadata (TestingConfigurationMetadata.TestAssemblyFullPath, testAssemblyFullPath);

      testingConfigurationItem.SetMetadata (TestingConfigurationMetadata.TestAssemblyDirectoryName, _pathHelper.GetDirectoryName (testAssemblyFullPath));

      var configurationItems = Regex.Replace (unsplitConfiguration, @"\s+", "").Split ('+');

      var browser = GetBrowser (configurationItems);
      testingConfigurationItem.SetMetadata (TestingConfigurationMetadata.Browser, browser);

      var isWebTest = string.Equals (browser, EmptyMetadataID.Browser, StringComparison.OrdinalIgnoreCase) ? "False" : "True";
      testingConfigurationItem.SetMetadata (TestingConfigurationMetadata.IsWebTest, isWebTest);

      var databaseSystem = GetDatabaseSystem (configurationItems);
      testingConfigurationItem.SetMetadata (TestingConfigurationMetadata.DatabaseSystem, databaseSystem);

      var isDatabaseTest = string.Equals (databaseSystem, EmptyMetadataID.DatabaseSystem, StringComparison.OrdinalIgnoreCase) ? "False" : "True";
      testingConfigurationItem.SetMetadata (TestingConfigurationMetadata.IsDatabaseTest, isDatabaseTest);

      var platform = configurationItems.Single (x => SupportedPlatforms.Select (i => i.ItemSpec).Contains (x, StringComparer.OrdinalIgnoreCase));
      testingConfigurationItem.SetMetadata (TestingConfigurationMetadata.Platform, platform);

      var use32Bit = string.Equals (platform, "x86", StringComparison.OrdinalIgnoreCase) ? "True" : "False";
      testingConfigurationItem.SetMetadata (TestingConfigurationMetadata.Use32Bit, use32Bit);

      var executionRuntime = configurationItems.Single (x => SupportedExecutionRuntimes.Select (i => i.ItemSpec).Contains (x));
      testingConfigurationItem.SetMetadata (TestingConfigurationMetadata.ExecutionRuntime, executionRuntime == "LocalMachine" ? "net-4.5" : executionRuntime);

      var configurationID = configurationItems.Single (x => SupportedConfigurationIDs.Select (i => i.ItemSpec).Contains (x));
      testingConfigurationItem.SetMetadata (TestingConfigurationMetadata.ConfigurationID, configurationID);

      return testingConfigurationItem;
    }

    private string GetDatabaseSystem (string[] configurationItems)
    {
      if (configurationItems.Contains ("NoDB", StringComparer.OrdinalIgnoreCase))
      {
        return "NoDB";
      }

      return configurationItems.Single (x => SupportedDatabaseSystems.Select (i => i.ItemSpec).Contains (x));
    }

    private string GetBrowser (string[] configurationItems)
    {
      if (configurationItems.Contains ("NoBrowser", StringComparer.OrdinalIgnoreCase))
      {
        return "NoBrowser";
      }

      return configurationItems.Single (x => SupportedBrowsers.Select (i => i.ItemSpec).Contains (x));
    }
  }
}