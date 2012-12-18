using System;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Remotion.BuildTools.MSBuildTasks
{
  public class CodeplexUploadFiles : ToolTask
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
    public string FileDisplayName { get; set; }
    [Required]
    public ITaskItem File { get; set; }
    // FileType: RuntimeBinary, SourceCode, Documentation or Example
    [Required]
    public string FileType { get; set; }

    protected override string GenerateFullPathToTool ()
    {
      return ToolPath;
    }

    protected override string GenerateCommandLineCommands ()
    {
      var fileDataPath = File.ToString();
      return string.Format("uploadReleaseFiles /projectName:{0} /releaseName:{1} /fileDisplayName:{2} /fileDataPath:{3} /fileType:{4} /username:{5} /password:{6}", ProjectName, ReleaseName, FileDisplayName, fileDataPath, FileType, Username, Password);
    }

    protected override string ToolName
    {
      get { return "CodeplexReleaseTool.exe"; }
    }
  }
}
