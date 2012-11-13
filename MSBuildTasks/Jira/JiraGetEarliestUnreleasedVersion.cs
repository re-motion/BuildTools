using System;
using System.Linq;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Remotion.BuildTools.MSBuildTasks.Jira.ServiceFacade;

namespace Remotion.BuildTools.MSBuildTasks.Jira
{
  public class JiraGetEarliestUnreleasedVersions : Task
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
    public string NextVersionID { get; set; }

    [Output]
    public string NextVersionName { get; set; }

    [Output]
    public int NumberOfUnreleasedVersions { get; set; }

    public override bool Execute ()
    {
      try
      {
        IJiraProjectVersionService service = new JiraProjectVersionService (JiraUrl, JiraUsername, JiraPassword);
        var versions = service.FindUnreleasedVersions (JiraProject, VersionPattern).ToArray();

        VersionID = "";
        VersionName = "";
        NextVersionID = "";
        NextVersionName = "";
        NumberOfUnreleasedVersions = versions.Count();

        if(NumberOfUnreleasedVersions >= 1)
        {
          var version = versions.First();
          VersionID = version.id;
          VersionName = version.name;
        }

        if(NumberOfUnreleasedVersions >= 2)
        {
          var nextVersion = versions.Skip (1).First();
          NextVersionID = nextVersion.id;
          NextVersionName = nextVersion.name;
        }

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
