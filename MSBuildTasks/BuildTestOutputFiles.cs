﻿// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
    private readonly ITaskLogger _logger;

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

    [Required]
    public ITaskItem[] SupportedTargetRuntimes { get; set; }


    [Output]
    public ITaskItem[] Output { get; set; }

    public BuildTestOutputFiles ()
    {
      _pathHelper = new PathWrapper (new FileSystem());
      _logger = new TaskLogger (Log);
    }

    public BuildTestOutputFiles (IPath pathHelper, ITaskLogger logger)
    {
      _pathHelper = pathHelper;
      _logger = logger;
    }

    public override bool Execute ()
    {
      var output = new List<ITaskItem>();
      foreach (var item in Input)
      {
        var testingConfiguration = item.GetMetadata ("TestingConfiguration");
        var testingSetupBuildFile = item.GetMetadata ("TestingSetupBuildFile");
        _logger.LogWarning ("superduperoriginal:{0}", item.ItemSpec);
        _logger.LogWarning ("originaltsetingconfigurations:{0}", testingConfiguration);


        var configurations = GetSplitConfigurations (testingConfiguration);
        foreach (var plusSeparatedConfigurationItems in configurations)
        {
          var configurationItems = GetConfigurationItems (plusSeparatedConfigurationItems);

          var duplicateItems = GetDuplicateItems (configurationItems);

          if (duplicateItems.Any())
          {
            foreach (var duplicateItem in duplicateItems)
            {
              _logger.LogError ("The following configuration values were found multiple times: '{0}'", duplicateItem);
            }

            return false;
          }

          var newItem = CreateConfigurationItem (item.ItemSpec, configurationItems, plusSeparatedConfigurationItems, testingSetupBuildFile);
          output.Add (newItem);
        }
      }

      Output = output.ToArray();
      return true;
    }

    private IEnumerable<string> GetConfigurationItems (string configuration)
    {
      return Regex.Replace (configuration, @"\s+", "").Split ('+');
    }

    private IEnumerable<string> GetSplitConfigurations (string testingConfiguration)
    {
      return Regex.Replace (testingConfiguration, @"\s+", "").Split (new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
    }

    private IEnumerable<string> GetDuplicateItems (IEnumerable<string> configurationItems)
    {
      return configurationItems
          .GroupBy (x => x)
          .Where (group => group.Count() > 1)
          .Select (group => group.Key)
          .ToArray();
    }

    private ITaskItem CreateConfigurationItem (string originalItemSpec, IEnumerable<string> splitConfiguration, string unsplitConfiguration, string testingSetupBuildFile)
    {
      var testAssemblyFileName = _pathHelper.GetFileName (originalItemSpec);
      var testingConfigurationItem = new TaskItem (testAssemblyFileName + "_" + unsplitConfiguration);
      testingConfigurationItem.SetMetadata (TestingConfigurationMetadata.TestAssemblyFileName, testAssemblyFileName);
      testingConfigurationItem.SetMetadata (TestingConfigurationMetadata.TestingSetupBuildFile, testingSetupBuildFile);

      testingConfigurationItem.SetMetadata (TestingConfigurationMetadata.TestAssemblyFullPath, originalItemSpec);

      testingConfigurationItem.SetMetadata (TestingConfigurationMetadata.TestAssemblyDirectoryName, _pathHelper.GetDirectoryName (originalItemSpec));

      var browser = GetBrowser (splitConfiguration);
      testingConfigurationItem.SetMetadata (TestingConfigurationMetadata.Browser, browser);

      var isWebTest = string.Equals (browser, EmptyMetadataID.Browser, StringComparison.OrdinalIgnoreCase) ? "False" : "True";
      testingConfigurationItem.SetMetadata (TestingConfigurationMetadata.IsWebTest, isWebTest);

      var databaseSystem = GetDatabaseSystem (splitConfiguration);
      testingConfigurationItem.SetMetadata (TestingConfigurationMetadata.DatabaseSystem, databaseSystem);

      var isDatabaseTest = string.Equals (databaseSystem, EmptyMetadataID.DatabaseSystem, StringComparison.OrdinalIgnoreCase) ? "False" : "True";
      testingConfigurationItem.SetMetadata (TestingConfigurationMetadata.IsDatabaseTest, isDatabaseTest);

      var platform = splitConfiguration.Single (x => SupportedPlatforms.Select (i => i.ItemSpec).Contains (x, StringComparer.OrdinalIgnoreCase));
      testingConfigurationItem.SetMetadata (TestingConfigurationMetadata.Platform, platform);

      var use32Bit = string.Equals (platform, "x86", StringComparison.OrdinalIgnoreCase) ? "True" : "False";
      testingConfigurationItem.SetMetadata (TestingConfigurationMetadata.Use32Bit, use32Bit);

      var executionRuntime = GetExecutionRuntime (splitConfiguration);
      testingConfigurationItem.SetMetadata (TestingConfigurationMetadata.ExecutionRuntime, executionRuntime);

      var useDocker = string.Equals (executionRuntime, EmptyMetadataID.ExecutionRuntime, StringComparison.OrdinalIgnoreCase) ? "False" : "True";
      testingConfigurationItem.SetMetadata (TestingConfigurationMetadata.UseDocker, useDocker);

      var configurationID = splitConfiguration.Single (x => SupportedConfigurationIDs.Select (i => i.ItemSpec).Contains (x));
      testingConfigurationItem.SetMetadata (TestingConfigurationMetadata.ConfigurationID, configurationID);

      var targetRuntime = GetTargetRuntimes (splitConfiguration);
      testingConfigurationItem.SetMetadata (TestingConfigurationMetadata.TargetRuntime, targetRuntime);

      return testingConfigurationItem;
    }

    private string GetExecutionRuntime (IEnumerable<string> configurationItems)
    {
      if (configurationItems.Contains (EmptyMetadataID.ExecutionRuntime, StringComparer.OrdinalIgnoreCase))
        return EmptyMetadataID.ExecutionRuntime;

      var supportedExecutionRuntimesDictionary = SupportedExecutionRuntimes
          .Select (item => item.ItemSpec)
          .Select (x => x.Split (new[] { ':' }, StringSplitOptions.RemoveEmptyEntries))
          .ToDictionary (split => split[0], split => split[1], StringComparer.OrdinalIgnoreCase);

      foreach (var item in configurationItems)
      {
        if (supportedExecutionRuntimesDictionary.TryGetValue (item, out var supportedExecutionRuntime))
          return supportedExecutionRuntime;
      }

      throw new KeyNotFoundException();
    }

    private string GetDatabaseSystem (IEnumerable<string> configurationItems)
    {
      if (configurationItems.Contains (EmptyMetadataID.DatabaseSystem, StringComparer.OrdinalIgnoreCase))
        return EmptyMetadataID.DatabaseSystem;

      return configurationItems.Single (x => SupportedDatabaseSystems.Select (i => i.ItemSpec).Contains (x));
    }

    private string GetBrowser (IEnumerable<string> configurationItems)
    {
      if (configurationItems.Contains (EmptyMetadataID.Browser, StringComparer.OrdinalIgnoreCase))
        return EmptyMetadataID.Browser;

      var caseInsensitiveBrowser = configurationItems.Single (x => SupportedBrowsers.Select (i => i.ItemSpec).Contains (x, StringComparer.OrdinalIgnoreCase));
      return char.ToUpper (caseInsensitiveBrowser[0]) + caseInsensitiveBrowser.ToLower().Substring (1);
    }

    private string GetTargetRuntimes (IEnumerable<string> configurationItems)
    {
      var raw = configurationItems.Single (x => SupportedTargetRuntimes.Select (i => i.ItemSpec).Contains (x, StringComparer.OrdinalIgnoreCase)).ToUpper();
      return Regex.Replace (raw, @"NET(\d)(\d)", "NET-$1.$2");
    }
  }
}