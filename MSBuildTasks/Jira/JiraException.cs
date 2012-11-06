using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Remotion.BuildTools.MSBuildTasks.Jira
{
  public class JiraException : Exception
  {
    public JiraException(string message)
      : base(message)
    {
    }
  }
}
