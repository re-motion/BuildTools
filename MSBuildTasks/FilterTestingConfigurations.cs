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
using System.Linq;
using System.Security.Policy;
using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;

namespace Remotion.BuildTools.MSBuildTasks
{
  public class FilterTestingConfigurations : Task
  {
    private readonly ITaskLogger _logger;
    [Required] public ITaskItem[] Input { get; set; }
    [Required] public ITaskItem[] ValidPlatforms { get; set; }
    [Required] public ITaskItem[] ValidDatabaseSystems { get; set; }
    [Required] public ITaskItem[] ValidBrowsers { get; set; }

    [Output] public ITaskItem[] Output { get; set; }

    public FilterTestingConfigurations ()
    {
      _logger = new TaskLogger (Log);
    }

    public FilterTestingConfigurations (ITaskLogger logger)
    {
      _logger = logger;
    }

    public override bool Execute ()
    {
      var validInputs = Input
          .Where (HasValidPlatform)
          .Where (HasValidDatabaseSystem)
          .Where (HasValidBrowser);
      Output = validInputs.ToArray();

      _logger.LogMessage (
          @"The following test configurations were filtered out and will not be run:
{0}",
          string.Join (Environment.NewLine, Input.Except (Output).Select (GetConfigurationString)));

      var uniqueInputAssemblyNames = GetUniqueAssemblyNames (Input);
      var uniqueOutputAssemblyNames = GetUniqueAssemblyNames (Output);
      var untestedAssemblyNames = uniqueInputAssemblyNames.Where (assemblyName => !uniqueOutputAssemblyNames.Contains (assemblyName));
      foreach (var assemblyName in untestedAssemblyNames)
      {
        _logger.LogWarning ($"All testing configurations in {assemblyName} were ignored.");
      }

      return true;
    }

    private string GetConfigurationString (ITaskItem taskItem)
    {
      var browser = taskItem.GetMetadata (TestingConfigurationMetadata.Browser);
      var platform = taskItem.GetMetadata (TestingConfigurationMetadata.Platform);
      var databaseSystem = taskItem.GetMetadata (TestingConfigurationMetadata.DatabaseSystem);
      var executionRuntime = taskItem.GetMetadata (TestingConfigurationMetadata.ExecutionRuntime);
      var configurationID = taskItem.GetMetadata (TestingConfigurationMetadata.ConfigurationID);
      return $"{taskItem.ItemSpec}: {browser}, {databaseSystem}, {platform}, {executionRuntime}, {configurationID}";
    }

    private bool HasValidPlatform (ITaskItem item)
    {
      return ValidPlatforms.Select (i => i.ItemSpec).Contains (item.GetMetadata (TestingConfigurationMetadata.Platform));
    }

    private bool HasValidDatabaseSystem (ITaskItem item)
    {
      var database = item.GetMetadata (TestingConfigurationMetadata.DatabaseSystem);
      if (string.Equals (database, "NoDb", StringComparison.OrdinalIgnoreCase) && !ValidDatabaseSystems.Any())
        return true;

      return ValidDatabaseSystems.Select (i => i.ItemSpec).Contains (item.GetMetadata (TestingConfigurationMetadata.DatabaseSystem));
    }

    private bool HasValidBrowser (ITaskItem item)
    {
      var browser = item.GetMetadata (TestingConfigurationMetadata.Browser);
      if (browser == "NoBrowser" && !ValidBrowsers.Any())
        return true;

      return ValidBrowsers.Select (i => i.ItemSpec).Contains (browser);
    }

    private HashSet<string> GetUniqueAssemblyNames (IEnumerable<ITaskItem> items)
    {
      return new HashSet<string> (items.Select (x => x.ItemSpec));
    }
  }
}