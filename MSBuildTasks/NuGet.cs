using System;
using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;

namespace Remotion.BuildTools.MSBuildTasks
{
  public class NuGet : ToolTask
  {
    [Required]
    public ITaskItem NuSpecFile { get; set; }
    
    [Required]
    public ITaskItem NuGetPath { get; set; }

    protected override string GenerateFullPathToTool ()
    {
      return NuGetPath.ItemSpec;
    }

    protected override string GenerateCommandLineCommands ()
    {
      return String.Format("pack {0} -sym", NuSpecFile.ItemSpec);
    }

    protected override string ToolName
    {
      get { return "NuGet"; }
    }
  }
}
