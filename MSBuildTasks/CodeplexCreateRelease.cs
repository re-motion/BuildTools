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
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Remotion.BuildTools.MSBuildTasks
{
  public class CodeplexCreateRelease : ToolTask
  {
    [Required]
    public string ProjectName { get; set; }
    [Required]
    public string ReleaseName { get; set; }
    [Required]
    public string Username { get; set; }
    [Required]
    public string Password { get; set; }
    [Required]
    public string Description { get; set; }
    [Required]
    public bool IsDefaultRelease { get; set; }
    // ReleaseDate: yyyy-MM-dd
    [Required]
    public string ReleaseDate { get; set; }
    // ReleaseStatus: Planning, Alpha, Beta or Stable
    [Required]
    public string ReleaseStatus { get; set; }
    [Required]
    public bool ShowToPublic { get; set; }

    protected override string GenerateFullPathToTool ()
    {
      return ToolPath;
    }

    protected override string GenerateCommandLineCommands ()
    {
      var showToPublic = ShowToPublic ? '+' : '-';
      var isDefaultRelease = IsDefaultRelease ? '+' : '-';
      return string.Format("createRelease /projectName:{0} /releaseName:{1} /description:\"{2}\" /releaseDate:{3} /status:{4} /showToPublic:{5} /isDefaultRelease:{6} /username:{7} /password:{8}", ProjectName, ReleaseName, Description, ReleaseDate, ReleaseStatus, showToPublic, isDefaultRelease, Username, Password);
    }

    protected override string ToolName
    {
      get { return "CodeplexReleaseTool.exe"; }
    }
  }
}
