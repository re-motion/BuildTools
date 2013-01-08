using System;

namespace Remotion.BuildTools.MSBuildTasks.Jira.ServiceFacade
{
  public class JiraException : Exception
  {
    public JiraException(string message)
      : base(message)
    {
    }
  }
}
