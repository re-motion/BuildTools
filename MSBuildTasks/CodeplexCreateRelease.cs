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
