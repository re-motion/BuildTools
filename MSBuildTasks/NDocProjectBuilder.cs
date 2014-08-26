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
using System.IO;
using System.Xml.Linq;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Remotion.BuildTools.MSBuildTasks
{
  public class NDocProjectBuilder : Task
  {
    [Required]
    public ITaskItem[] DocumenterFiles { get; set; }

    [Required]
    public ITaskItem[] Assemblies { get; set; }

    public ITaskItem[] NamespaceSummaryFiles { get; set; }

    [Required]
    public string ResultFile { get; set; }

    public override bool Execute ()
    {
      XDocument projectFile;
      try
      {
        projectFile = new XDocument (
            new XDeclaration ("1.0", "utf-8", "yes"),
            new XElement (
                "project",
                new XAttribute ("SchemaVersion", "2.0"),
                GetAssemblies(),
                GetNamespaceSummaries(),
                GetDocumenters()));
      }
      catch (InvalidOperationException ex)
      {
        Log.LogErrorFromException (ex);
        return false;
      }

      try
      {
        projectFile.Save (ResultFile, SaveOptions.None);
        Log.LogMessage (MessageImportance.Normal, "Generated ndoc project file '{0}'.", ResultFile);
      }
      catch (Exception ex)
      {
        Log.LogErrorFromException (ex);
        return false;
      }

      return true;
    }

    private XElement GetAssemblies ()
    {
      var assemblies = new XElement ("assemblies");

      foreach (var assembly in Assemblies)
        assemblies.Add (GetAssemblyDocumentation (assembly));

      return assemblies;
    }

    private XElement GetAssemblyDocumentation (ITaskItem assembly)
    {
      var assemblyLocation = assembly.ItemSpec;
      var assemblyDocumention = Path.ChangeExtension (assemblyLocation, "xml");

      if (!File.Exists (assemblyDocumention))
      {
        Log.LogMessage (MessageImportance.Normal, "Assembly '{0}' does not contain documentation. Assembly will be ignored.", assemblyLocation);
        return null;
      }

      try
      {
        return new XElement (
            "assembly",
            new XAttribute ("location", assemblyLocation),
            new XAttribute ("documentation", assemblyDocumention));
      }
      finally
      {
        Log.LogMessage (MessageImportance.Normal, "Added assembly '{0}'.", assemblyLocation);
      }
    }

    private XElement GetNamespaceSummaries ()
    {
      var namespaceSummaries = new XElement ("namespaces");

      foreach (var documenterFile in NamespaceSummaryFiles)
        namespaceSummaries.Add (GetNamespaceSummariesFromFile (documenterFile));

      return namespaceSummaries;
    }

    private XElement GetDocumenters ()
    {
      var documenters = new XElement ("documenters");

      foreach (var documenterFile in DocumenterFiles)
        documenters.Add (GetDocumentersFromFile (documenterFile));

      return documenters;
    }

    private IEnumerable<XElement> GetNamespaceSummariesFromFile (ITaskItem namespaceSummaryFile)
    {
      Log.LogMessage (MessageImportance.Low, "Importing namespace summary file: {0}", namespaceSummaryFile.ItemSpec);

      XDocument xDocument;
      try
      {
        xDocument = XDocument.Load (namespaceSummaryFile.ItemSpec, LoadOptions.None);
      }
      catch (Exception ex)
      {
        throw new InvalidOperationException (
            string.Format ("There was an error reading the namespace-summary-file '{0}': {1}", namespaceSummaryFile.ItemSpec, ex.Message), ex);
      }

      if (xDocument.Root == null || xDocument.Root.Name != "namespaces")
      {
        throw new InvalidOperationException (
            string.Format ("The namespace-summary-file '{0}' does not contain a 'namespaces' root node.", namespaceSummaryFile.ItemSpec));
      }

      try
      {
        return xDocument.Root.Elements ("namespace");
      }
      finally
      {
        Log.LogMessage (MessageImportance.Normal, "Imported namespace summary file '{0}'.", namespaceSummaryFile.ItemSpec);
      }
    }

    private IEnumerable<XElement> GetDocumentersFromFile (ITaskItem documenterFile)
    {
      Log.LogMessage (MessageImportance.Low, "Importing documenter file: {0}", documenterFile.ItemSpec);

      XDocument xDocument;
      try
      {
        xDocument = XDocument.Load (documenterFile.ItemSpec, LoadOptions.None);
      }
      catch (Exception ex)
      {
        throw new InvalidOperationException (
            string.Format ("There was an error reading the namespace-summary-file '{0}': {1}", documenterFile.ItemSpec, ex.Message), ex);
      }

      if (xDocument.Root == null || xDocument.Root.Name != "documenters")
      {
        throw new InvalidOperationException (
            string.Format ("The documenters-file '{0}' does not contain a 'documenters' root node.", documenterFile.ItemSpec));
      }

      try
      {
        return xDocument.Root.Elements ("documenter");
      }
      finally
      {
        Log.LogMessage (MessageImportance.Normal, "Imported documenter file '{0}'.", documenterFile.ItemSpec);
      }
    }
  }
}