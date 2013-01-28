using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

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

  public class JiraNonClosedIssues
  {
    public List<JiraNonClosedIssue> issues { get; set; }
  }

  public class JiraNonClosedIssue
  {
    public string id { get; set; }
    public JiraNonClosedIssueFields fields { get; set; }
  }

  public class JiraNonClosedIssueFields
  {
    public List<JiraVersion> fixVersions { get; set; }
  }

  public class JiraVersion
  {
    public string id { get; set; }
  }
}
