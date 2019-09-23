using System;

namespace Remotion.BuildTools.MSBuildTasks
{
  public interface ITaskLogger
  {
    void LogMessage (string message, params object[] args);
    void LogWarning (string message, params object[] args);
  }
}