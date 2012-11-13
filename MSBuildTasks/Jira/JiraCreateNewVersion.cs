using System;
using System.Linq;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Remotion.BuildTools.MSBuildTasks.Jira.ServiceFacade;

namespace Remotion.BuildTools.MSBuildTasks.Jira
{
  public class JiraCreateNewVersion : Task
  {
    [Required]
    public string JiraUrl { get; set; }

    [Required]
    public string JiraUsername { get; set; }

    [Required]
    public string JiraPassword { get; set; }

    [Required]
    public string JiraProject { get; set; }

    [Required]
    public string VersionPattern { get; set; }

    [Required]
    public int VersionComponentToIncrement { get; set; }

    [Required]
    public DayOfWeek VersionReleaseWeekday { get; set; }

    public override bool Execute ()
    {
      try
      {
        IJiraProjectVersionService service = new JiraProjectVersionService (JiraUrl, JiraUsername, JiraPassword);
        service.CreateSubsequentVersion (JiraProject, VersionPattern, VersionComponentToIncrement, VersionReleaseWeekday);

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
