using System;
using System.Linq;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Remotion.BuildTools.MSBuildTasks.Jira.ServiceFacade;

namespace Remotion.BuildTools.MSBuildTasks.Jira
{
  public class JiraReleaseVersion : Task
  {
    [Required]
    public string JiraUrl { get; set; }

    [Required]
    public string JiraUsername { get; set; }

    [Required]
    public string JiraPassword { get; set; }

    [Required]
    public string VersionID { get; set; }

    [Required]
    public string NextVersionID { get; set; }

    public override bool Execute ()
    {
      try
      {
        IJiraProjectVersionService service = new JiraProjectVersionService (JiraUrl, JiraUsername, JiraPassword);
        service.ReleaseVersion (VersionID, NextVersionID);

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
