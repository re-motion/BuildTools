// Copyright (c) rubicon IT GmbH, www.rubicon.eu
//
// See the NOTICE file distributed with this work for additional information
// regarding copyright ownership.  rubicon licenses this file to you under 
// the Apache License, Version 2.0 (the "License"); you may not use this 
// file except in compliance with the License.  You may obtain a copy of the 
// License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT 
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.  See the 
// License for the specific language governing permissions and limitations
// under the License.
// 
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