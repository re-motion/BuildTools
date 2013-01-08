using System;
using System.Collections.Generic;

namespace Remotion.BuildTools.MSBuildTasks.Jira.ServiceFacade
{
  public class JiraIssue
  {
    public string id { get; set; }
    public string summary { get; set; }
    public List<string> fixVersion { get; set; }

    public string issuetype { get; set; }
    public string project { get; set; }
  }

  public class JiraIssues
  {
    public List<JiraIssue> issues { get; set; }
  }
}
