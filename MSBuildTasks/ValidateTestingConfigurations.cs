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
using System.Linq;
using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;

namespace Remotion.BuildTools.MSBuildTasks
{
  public class ValidateTestingConfigurations : Task
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
    public ITaskItem[] SupportedDatabaseSystems { get; set; }

    [Required]
    public ITaskItem[] SupportedBrowsers { get; set; }

    [Output]
    public ITaskItem[] Output { get; set; }

    public ValidateTestingConfigurations ()
    {
      _logger = new TaskLogger (Log);
    }

    public ValidateTestingConfigurations (ITaskLogger logger)
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

      Output = Input;

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
  }
}