using System;
using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;

namespace Remotion.BuildTools.MSBuildTasks
{
  public class NuGetPack : ToolTask
  {
    [Required]
    public ITaskItem NuSpecFile { get; set; }

    [Required]
    public string Version { get; set; }

    [Required]
    public ITaskItem OutputDirectory { get; set; }

    protected override string GenerateFullPathToTool ()
    {
      return ToolPath;
    }

    protected override string GenerateCommandLineCommands ()
    {
      return string.Format("pack \"{0}\" -NonInteractive -Symbols -Version {1} -OutputDirectory \"{2}\"", NuSpecFile.ItemSpec, Version, OutputDirectory);
    }

    protected override string ToolName
    {
      get { return "NuGet.exe"; }
    }
  }
}
