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
using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;

namespace Remotion.BuildTools.MSBuildTasks
{
  public class FilterTestingConfigurations : Task
  {
    private class MetadataValidationError
    {
      public string Assembly { get; }
      public string Metadata { get; }
      public string Value { get; }

      public MetadataValidationError (string assembly, string metadata, string value)
      {
        Assembly = assembly;
        Metadata = metadata;
        Value = value;
      }
    }

    private readonly ITaskLogger _logger;

    [Required]
    public ITaskItem[] Input { get; set; }

    [Required]
    public ITaskItem[] SupportedPlatforms { get; set; }

    [Required]
    public ITaskItem[] AllPlatforms { get; set; }

    [Required]
    public ITaskItem[] AllBrowsers { get; set; }

    [Required]
    public ITaskItem[] SupportedDatabaseSystems { get; set; }

    [Required]
    public ITaskItem[] AllDatabaseSystems { get; set; }

    [Required]
    public ITaskItem[] SupportedBrowsers { get; set; }

    [Output]
    public ITaskItem[] Output { get; set; }

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
      var errors = GetErrors().ToArray();

      foreach (var error in errors)
      {
        _logger.LogError ($"{error.Assembly}: Metadata '{error.Metadata}' with value '{error.Value}' of TestingConfiguration is not supported.");
      }

      if (errors.Any())
      {
        return false;
      }

      Output = Input
          .Where (IsInAllBrowsers)
          .Where (IsInAllPlatforms)
          .Where (IsInAllDatabaseSystems)
          .ToArray();

      return true;
    }

    private IEnumerable<MetadataValidationError> GetErrors ()
    {
      foreach (var item in Input)
      {
        var platform = item.GetMetadata (TestingConfigurationMetadata.Platform);
        if (!IsValidPlatform (platform))
        {
          yield return new MetadataValidationError (item.ItemSpec, TestingConfigurationMetadata.Platform, platform);
        }

        var browser = item.GetMetadata (TestingConfigurationMetadata.Browser);
        if (!IsValidBrowser (browser))
        {
          yield return new MetadataValidationError (item.ItemSpec, TestingConfigurationMetadata.Browser, browser);
        }

        var databaseSystem = item.GetMetadata (TestingConfigurationMetadata.DatabaseSystem);
        if (!IsValidDatabaseSystem (databaseSystem))
        {
          yield return new MetadataValidationError (item.ItemSpec, TestingConfigurationMetadata.DatabaseSystem, databaseSystem);
        }
      }
    }

    private bool IsValidPlatform (string platform)
    {
      return SupportedPlatforms.Select (i => i.ItemSpec).Contains (platform, StringComparer.OrdinalIgnoreCase);
    }

    private bool IsValidDatabaseSystem (string database)
    {
      if (string.Equals (database, EmptyMetadataID.DatabaseSystem, StringComparison.OrdinalIgnoreCase) && !SupportedDatabaseSystems.Any())
        return true;

      return SupportedDatabaseSystems.Select (i => i.ItemSpec).Contains (database, StringComparer.OrdinalIgnoreCase);
    }

    private bool IsValidBrowser (string browser)
    {
      if (string.Equals (browser, EmptyMetadataID.Browser, StringComparison.OrdinalIgnoreCase) && !SupportedBrowsers.Any())
        return true;

      return SupportedBrowsers.Select (i => i.ItemSpec).Contains (browser, StringComparer.OrdinalIgnoreCase);
    }

    private IEnumerable<ITaskItem> GetFilteredItems ()
    {
      var itemsToBeFiltered = new List<ITaskItem>();
      foreach (var item in Input)
      {
        var platform = item.GetMetadata (TestingConfigurationMetadata.Platform);
        if (!PlatformShouldBeFiltered (platform))
        {
          itemsToBeFiltered.Add (item);
          continue;
        }

        var browser = item.GetMetadata (TestingConfigurationMetadata.Browser);
        if (BrowserShouldBeFiltered (browser))
        {
          itemsToBeFiltered.Remove (item);
        }
      }

      return itemsToBeFiltered;
    }

    private bool PlatformShouldBeFiltered (string platform)
    {
      return !AllPlatforms.Select (i => i.ItemSpec).Contains (platform, StringComparer.OrdinalIgnoreCase);
    }

    private bool BrowserShouldBeFiltered (string platform)
    {
      return !AllBrowsers.Select (i => i.ItemSpec).Contains (platform, StringComparer.OrdinalIgnoreCase);
    }

    private bool IsInAllPlatforms (ITaskItem item)
    {
      return AllPlatforms.Select (i => i.ItemSpec).Contains (item.GetMetadata (TestingConfigurationMetadata.Platform), StringComparer.OrdinalIgnoreCase);
    }

    private bool IsInAllBrowsers (ITaskItem item)
    {
      var browser = item.GetMetadata (TestingConfigurationMetadata.Browser);

      if (string.Equals (browser, EmptyMetadataID.Browser, StringComparison.OrdinalIgnoreCase))
        return true;

      return AllBrowsers.Select (i => i.ItemSpec).Contains (browser, StringComparer.OrdinalIgnoreCase);
    }

    private bool IsInAllDatabaseSystems (ITaskItem item)
    {
      var databaseSystem = item.GetMetadata (TestingConfigurationMetadata.DatabaseSystem);

      if (string.Equals (databaseSystem, EmptyMetadataID.DatabaseSystem, StringComparison.OrdinalIgnoreCase))
        return true;

      return AllDatabaseSystems.Select (i => i.ItemSpec).Contains (databaseSystem, StringComparer.OrdinalIgnoreCase);
    }
  }
}