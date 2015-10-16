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

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using RestSharp;

namespace Remotion.BuildTools.MSBuildTasks.Jira
{
  public abstract class JiraTask : Task
  {
    [Required]
    public string JiraUrl { get; set; }

    private string jiraUsername = "";
    private string jiraPassword = "";

    [Required]
    public string JiraUsername
    {
      get { return jiraUsername; } 
      set { jiraUsername = value; }
    }

    [Required]
    public string JiraPassword
    {
      get { return jiraPassword; } 
      set { jiraPassword = value; }
    }

    protected IAuthenticator Authenticator
    {
      get
      {
        IAuthenticator authenticator;

        if (string.IsNullOrEmpty(JiraUsername) && string.IsNullOrEmpty(JiraPassword))
        {
          authenticator = new NtlmAuthenticator();
        }
        else
        {
          authenticator = new HttpBasicAuthenticator(JiraUsername, JiraPassword);
        }
        return authenticator;
      }
    }
  }
}
