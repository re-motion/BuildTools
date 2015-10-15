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
using Remotion.BuildTools.MSBuildTasks.Jira.ServiceFacadeImplementations;
using Remotion.BuildTools.MSBuildTasks.Jira.ServiceFacadeInterfaces;

namespace Remotion.BuildTools.MSBuildTasks.Jira
{
  public class JiraGetEarliestUnreleasedVersion : JiraTask
  {
    [Required]
    public string JiraProject { get; set; }

    public string VersionPattern { get; set; }

    [Output]
    public string VersionID { get; set; }

    [Output]
    public string VersionName { get; set; }

    [Output]
    public string NextVersionID { get; set; }

    [Output]
    public string NextVersionName { get; set; }

    [Output]
    public int NumberOfUnreleasedVersions { get; set; }

    public override bool Execute ()
    {
      try
      {
        IJiraProjectVersionFinder finder = new JiraProjectVersionFinder (JiraUrl, Authenticator);
        var versions = finder.FindUnreleasedVersions (JiraProject, VersionPattern).ToArray();

        VersionID = "";
        VersionName = "";
        NextVersionID = "";
        NextVersionName = "";
        NumberOfUnreleasedVersions = versions.Count();

        if(NumberOfUnreleasedVersions >= 1)
        {
          var version = versions.First();
          VersionID = version.id;
          VersionName = version.name;
        }

        if(NumberOfUnreleasedVersions >= 2)
        {
          var nextVersion = versions.Skip (1).First();
          NextVersionID = nextVersion.id;
          NextVersionName = nextVersion.name;
        }

        return true;
      }
      catch(Exception ex)
      {
        Log.LogErrorFromException (ex);
        return false;
      }
    }
  }
}
