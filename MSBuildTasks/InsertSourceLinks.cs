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
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Microsoft.Win32;
using SourceLink;
using Process = System.Diagnostics.Process;

namespace Remotion.BuildTools.MSBuildTasks
{
  public class InsertSourceLinks : ToolTask
  {
    [Required]
    public string VcsUrlTemplate { get; set; }

    [Required]
    public string ProjectBaseDirectory { get; set; }

    [Required]
    public ITaskItem BuildOutputFile { get; set; }

    protected override string GenerateFullPathToTool ()
    {
      string registryKeyPath = @"SOFTWARE\Microsoft\Windows Kits\Installed Roots";
      using (var key = Registry.LocalMachine.OpenSubKey (registryKeyPath))
      {
        if (key == null)
          throw new InvalidOperationException (string.Format ("Could not open Registry key '{0}'.", registryKeyPath));

        var value = (string) key.GetValue ("KitsRoot");
        if (string.IsNullOrEmpty (value))
          throw new InvalidOperationException (string.Format ("Could not open entry 'KitsRoot' in Registry key '{0}'.", registryKeyPath));

        var sdkPath = Path.Combine (value, @"Debuggers\x86\srcsrv");
        string toolPath = Path.Combine (sdkPath, ToolName);
        if (!File.Exists (toolPath))
          throw new InvalidOperationException (string.Format ("Could not find Windows Debug SDK at location '{0}'.", sdkPath));

        return toolPath;
      }
    }

    protected override string GenerateCommandLineCommands ()
    {
      return string.Format ("-w -s:srcsrv -i:\"{0}\" -p:\"{1}\"", GetSrcsrvFile(), GetPdbFile());
    }

    public override bool Execute ()
    {
      if (string.IsNullOrEmpty (ToolPath))
      {
        try
        {
          ToolPath = Path.GetDirectoryName (GenerateFullPathToTool());
        }
        catch (Exception ex)
        {
          Log.LogErrorFromException (ex);
          return false;
        }
      }

      string pdbFile = GetPdbFile();
      if (GetIndexedFilesCount (pdbFile, ToolPath) > 0)
      {
        Log.LogWarning ("Could not insert source links for '{0}' because the PDB-file has already been indexed.", BuildOutputFile);
        return true;
      }

      string rawUrl = string.Format (VcsUrlTemplate, "%var2%");
      string revision = "";
      var sourceFiles = GetSourceFiles (pdbFile, ToolPath);
      string srcsrvFile = GetSrcsrvFile();
      File.WriteAllBytes (srcsrvFile, SrcSrv.create (rawUrl, revision, sourceFiles));

      try
      {
        return base.Execute();
      }
      finally
      {
       File.Delete (srcsrvFile);
      }
    }

    protected override string ToolName
    {
      get { return "pdbstr.exe"; }
    }

    private string GetSrcsrvFile ()
    {
      return Path.ChangeExtension (BuildOutputFile.ToString(), ".srcsrv");
    }

    private string GetPdbFile ()
    {
      return Path.ChangeExtension (BuildOutputFile.ToString(), ".pdb");
    }

    private IEnumerable<Tuple<string,string>> GetSourceFiles (string pdbFile, string toolsPath)
    {
      int exitCode;
      string extractedCommandOutput = Execute (Path.Combine (toolsPath, "srctool.exe"), "-r " + pdbFile, true, out exitCode);
      var sourceFiles = extractedCommandOutput.Split (new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
      return sourceFiles.Take (sourceFiles.Length - 1) // last line is srctool.exe summary
          .Select (f => Tuple.Create (f, MakeRelativeFile (f)));
    }

    private int GetIndexedFilesCount (string pdbFile, string toolsPath)
    {
      int exitCode;
      Execute (Path.Combine (toolsPath, "srctool.exe"), "-c " + pdbFile, true, out exitCode);
      return exitCode;
    }


    private string MakeRelativeFile (string sourceFile)
    {
      var projectBaseUrl = new Uri ("file:///" + Path.GetFullPath (ProjectBaseDirectory + "\\"));
      var fileUrl = new Uri ("file:///" + sourceFile);
      return projectBaseUrl.MakeRelativeUri (fileUrl).ToString();
    }

    private string Execute (string cmd, string args, bool ignoreExitCode, out int exitCode)
    {
      Process p = new Process
                  {
                      StartInfo = new ProcessStartInfo (cmd, args)
                                  {
                                      UseShellExecute = false,
                                      RedirectStandardOutput = true,
                                      CreateNoWindow = true
                                  }
                  };

      p.Start();
      string output = p.StandardOutput.ReadToEnd();
      p.WaitForExit();
      exitCode = p.ExitCode;

      if (!ignoreExitCode && p.ExitCode != 0)
      {
        var errorMessage = string.Format ("Executable '{0}' failed with error code {1}, output: {2}", cmd, p.ExitCode, output);
        throw new InvalidOperationException (errorMessage);
      }

      return output;
    }
  }
}