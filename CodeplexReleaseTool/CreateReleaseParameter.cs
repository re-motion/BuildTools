using System;
using Remotion.Text.CommandLine;

namespace CodeplexReleaseTool
{
  public class CreateReleaseParameter
  {
    [CommandLineStringArgument ("projectName", false,
        Description = "The Codeplex project name.",
        Placeholder = "project")]
    public string ProjectName = string.Empty;

    [CommandLineStringArgument ("releaseName", false,
        Description = "The name of the release.",
        Placeholder = "release")]
    public string ReleaseName = string.Empty;

    [CommandLineStringArgument ("description", true,
        Description = "The description for the release.",
        Placeholder = "description")]
    public string Description = string.Empty;

    [CommandLineStringArgument ("status", false,
        Description = "The status of the release. Valid values are Planning, Alpha, Beta, Stable.",
        Placeholder = "status")]
    public string Status = string.Empty;

    [CommandLineStringArgument("releaseDate", true,
        Description = "The date that the release was released. Must be a valid date for releases with a status other than Planning. "
                     +"For Planning releases, this value is ignored.",
        Placeholder = "date")]
    public string ReleaseDate = DateTime.Now.ToString("dd.MM.yyyy");

    [CommandLineFlagArgument ("showToPublic", true,
        Description = "true if the release is visible to the public, false if the release should only be visible to Coordinators and Developers.",
        Placeholder = "showToPublic")]
    public bool ShowToPublic = false;

    [CommandLineFlagArgument ("isDefaultRelease", true,
        Description = "true to mark the release as the default release for the project, false otherwise. If there is an existing default release, "
                     +"that release will no longer be the default. ",
        Placeholder = "isDefaultRelease")]
    public bool IsDefaultRelease = false;

    [CommandLineStringArgument ("username", false,
        Description = "The codeplex user name.",
        Placeholder = "user")]
    public string Username = string.Empty;

    [CommandLineStringArgument ("password", false,
        Description = "The codeplex password.",
        Placeholder = "password")]
    public string Password = string.Empty;
  }
}