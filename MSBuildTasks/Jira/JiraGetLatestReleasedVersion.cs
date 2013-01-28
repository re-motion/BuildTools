using System;
using System.Linq;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Remotion.BuildTools.MSBuildTasks.Jira.ServiceFacade;

namespace Remotion.BuildTools.MSBuildTasks.Jira
{
  public class JiraGetLatestReleasedVersion : Task
  {
    [Required]
    public string JiraUrl { get; set; }

    [Required]
    public string JiraUsername { get; set; }

    [Required]
    public string JiraPassword { get; set; }

    [Required]
    public string JiraProject { get; set; }

    public string VersionPattern { get; set; }

    [Output]
    public string VersionID { get; set; }

    [Output]
    public string VersionName { get; set; }

    [Output]
    public string NextUnreleasedVersionID { get; set; }

    [Output]
    public string NextUnreleasedVersionName { get; set; }

    public override bool Execute ()
    {
      try
      {
        IJiraProjectVersionService service = new JiraProjectVersionService (JiraUrl, JiraUsername, JiraPassword);
        var versions = service.FindVersions (JiraProject, VersionPattern).ToArray();

        var unreleasedVersions = versions.Where (v => v.released != true);
        var releasedVersions = versions.Where (v => v.released == true);

        var latestReleasedVersion = releasedVersions.Last();
        var earliestUnreleasedVersion = unreleasedVersions.First();

        VersionID = latestReleasedVersion.id;
        VersionName = latestReleasedVersion.name;
        NextUnreleasedVersionID = earliestUnreleasedVersion.id;
        NextUnreleasedVersionName = earliestUnreleasedVersion.name;

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
