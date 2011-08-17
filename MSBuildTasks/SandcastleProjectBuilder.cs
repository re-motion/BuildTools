using System;
using System.IO;
using System.Linq;
using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;
using System.Xml.Linq;

namespace Remotion.BuildTools.MSBuildTasks
{
  public class SandcastleProjectBuilder : Task
  {
    [Required]
    public ITaskItem File { get; set; }

    [Required]
    public ITaskItem[] Assemblies { get; set; }

    public ITaskItem[] NamespaceSummaryFiles { get; set; }

    public override bool Execute ()
    {
      XDocument projectFile;
      try
      {
        projectFile = XDocument.Load (File.ItemSpec, LoadOptions.None);
        
        var propertyGroup = projectFile.Descendants ("PropertyGroup").Single ();
        propertyGroup.Add (GetDocumentationSources ());

        projectFile.Save (File.ItemSpec, SaveOptions.None);
        
        Log.LogMessage (MessageImportance.Normal, "Generated sandcastle project file '{0}'.", File);
        return true;
      }
      catch (Exception ex)
      {
        Log.LogErrorFromException (ex);
        return false;
      }
    }

    private XElement GetDocumentationSources ()
    {
      try
      {
        var sources = new XElement ("DocumentationSources");

        foreach (var assembly in Assemblies)
        {
          var documentation = GetAssemblyDocumentation (assembly);
          if (documentation != null)
          {
            sources.Add (GetDocumentationSource (assembly.ItemSpec));
            sources.Add (GetDocumentationSource (documentation));
          }
        }

        if (NamespaceSummaryFiles != null)
        {
          foreach (var namespaceSummary in NamespaceSummaryFiles)
            sources.Add (GetDocumentationSource (namespaceSummary.ItemSpec));
        }

        return sources;
      }
      catch (Exception ex)
      {
        Log.LogErrorFromException (ex);
        return null;
      }
    }

    private XElement GetDocumentationSource (string path)
    {
      return new XElement("DocumentationSource", new XAttribute("sourceFile", path));
    }

    private string GetAssemblyDocumentation (ITaskItem assembly)
    {
      var assemblyLocation = assembly.ItemSpec;
      var assemblyDocumention = Path.ChangeExtension (assemblyLocation, "xml");

      if (!System.IO.File.Exists (assemblyDocumention))
      {
        Log.LogMessage (MessageImportance.Normal, "Assembly '{0}' does not contain documentation. Assembly will be ignored.", assemblyLocation);
        return null;
      }

      Log.LogMessage (MessageImportance.Normal, "Added assembly '{0}'.", assemblyLocation);
      return assemblyDocumention;
    }
  }
}