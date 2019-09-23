using Microsoft.Build.Utilities;

namespace Remotion.BuildTools.MSBuildTasks
{
  public class TaskLogger : ITaskLogger
  {
    private readonly TaskLoggingHelper _loggingHelper;

    public TaskLogger (TaskLoggingHelper loggingHelper)
    {
      _loggingHelper = loggingHelper;
    }

    public void LogMessage (string message, params object[] args)
    {
      _loggingHelper.LogMessage (message, args);
    }

    public void LogWarning (string message, params object[] args)
    {
      _loggingHelper.LogWarning (message, args);
    }
  }
}