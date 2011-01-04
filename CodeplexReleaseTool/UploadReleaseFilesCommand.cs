using System;
using Remotion.Text.CommandLine;
using Remotion.Utilities;

namespace CodeplexReleaseTool
{
  public class UploadReleaseFilesCommand : ICommand
  {
    private ICodeplexWebService _service;

    public UploadReleaseFilesCommand (ICodeplexWebService service)
    {
      ArgumentUtility.CheckNotNull ("service", service);

      _service = service;
    }

    public void Execute (string[] args)
    {
      ArgumentUtility.CheckNotNull ("args", args);

      var parser = new CommandLineClassParser<UploadReleaseFilesParameter> ();
      var commandParameters = parser.Parse (args);


    }
  }
}