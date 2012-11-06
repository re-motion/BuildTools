using System;

namespace Remotion.BuildTools.MSBuildTasks.Jira
{
  public class JiraProjectVersion
  {
    public string self { get; set; }
    public string id { get; set; }
    public string description { get; set; }
    public string name { get; set; }
    public bool? archived { get; set; }
    public bool? released { get; set; }
    public DateTime? releaseDate { get; set; }
    public bool? overdue { get; set; }
    public string project { get; set; }
  }
}
