using System;
using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;

namespace Remotion.BuildTools.MSBuildTasks
{
  public class NuGetPush : ToolTask
  {
    [Required]
    public ITaskItem PackageFile { get; set; }

    [Required]
    public string ApiKey { get; set; }

    protected override string GenerateFullPathToTool ()
    {
      return ToolPath;
    }

    protected override string GenerateCommandLineCommands ()
    {
      return string.Format("push \"{0}\" {1} -NonInteractive -Timeout 60", PackageFile, ApiKey);
    }

    protected override string ToolName
    {
      get { return "NuGet.exe"; }
    }
  }
}
