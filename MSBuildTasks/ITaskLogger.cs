using System;
using Microsoft.Build.Framework;

namespace Remotion.BuildTools.MSBuildTasks
{
  public interface ITaskLogger
  {
    void LogMessage (string message, params object[] args);
  }
}