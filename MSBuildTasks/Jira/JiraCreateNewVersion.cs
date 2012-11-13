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

    private DayOfWeek _versionReleaseWeekday;

    [Required]
    public string VersionReleaseWeekday
    {
      get { return _versionReleaseWeekday.ToString(); }
      set { _versionReleaseWeekday = (DayOfWeek)Enum.Parse (typeof (DayOfWeek), value, true); }
    }

    public override bool Execute ()
    {
      try
      {
        IJiraProjectVersionService service = new JiraProjectVersionService (JiraUrl, JiraUsername, JiraPassword);
        service.CreateSubsequentVersion (JiraProject, VersionPattern, VersionComponentToIncrement, _versionReleaseWeekday);

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
