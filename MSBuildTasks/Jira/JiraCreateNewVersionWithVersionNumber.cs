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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Remotion.BuildTools.MSBuildTasks.Jira.ServiceFacade;

namespace Remotion.BuildTools.MSBuildTasks.Jira
{
  public class JiraCreateNewVersionWithVersionNumber : Task
  {
    [Required]
    public string JiraUrl { get; set; }

    [Required]
    public string JiraUsername { get; set; }

    [Required]
    public string JiraPassword { get; set; }

    [Required]
    public string JiraProjectKey { get; set; }

    [Required]
    public string VersionNumber { get; set; }

    [Output]
    public string CreatedVersionID { get; set; }

    public override bool Execute ()
    {
      try
      {
        IJiraProjectVersionService service = new JiraProjectVersionService (JiraUrl, JiraUsername, JiraPassword);
        
        var versions = service.FindVersions (JiraProjectKey, "(?s).*");
        var jiraProject = versions.Where(x => x.name == VersionNumber).DefaultIfEmpty().First();
        
        if (jiraProject != null)
        {
          if (jiraProject.released != null)
          {
            if (jiraProject.released.Value)
              throw new JiraException ("The Version '" + VersionNumber + "' got already released in Jira.");  
          }
          
          CreatedVersionID = jiraProject.id;
          return true;
        }

        CreatedVersionID = service.CreateVersion (JiraProjectKey, VersionNumber, null);
        return true;
      }
      catch (Exception ex)
      {
        Log.LogErrorFromException (ex);
        return false;
      }
    }
  }
}