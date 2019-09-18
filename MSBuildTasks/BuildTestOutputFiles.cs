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
using System;
using System.Collections.Generic;
using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;

namespace Remotion.BuildTools.MSBuildTasks
{
  public class BuildTestOutputFiles : Task
  {
    private string _toolName;

    [Required]
    public ITaskItem[] Input { get; set; }

    [Output]
    public ITaskItem[] Output { get; set; }

    public override bool Execute ()
    {
      var output = new List<ITaskItem>();
      foreach (var item in Input)
      {
        var index = 0;
        
        var testingConfiguration = item.GetMetadata("TestingConfiguration");

        var configurations = testingConfiguration.Split (';');
        foreach (var configuration in configurations)
        {
          var newItem = new TaskItem(item.ItemSpec + index);
          var splitItems = configuration.Split ('+');
          newItem.SetMetadata ("Browser", splitItems[0]);
          index++;
          output.Add(newItem);
        }
      }

      Output = output.ToArray();
      return true;
    }
}
}