using System;
using System.IO;
using Remotion.Text.CommandLine;
using Remotion.Utilities;

namespace CodeplexReleaseTool
{
  public class UploadReleaseFilesCommand : ICommand
  {
    private readonly ICodeplexWebService _service;

    public UploadReleaseFilesCommand (ICodeplexWebService service)
    {
      ArgumentUtility.CheckNotNull ("service", service);

      _service = service;
    }

    public void Execute (string[] args)
    {
      ArgumentUtility.CheckNotNull ("args", args);

      var parser = new CommandLineClassParser<UploadReleaseFilesParameter>();
      var commandParameters = parser.Parse (args);

      var releaseFile = new ReleaseFile();
      releaseFile.FileName = Path.GetFileName (commandParameters.FileDataPath);
      releaseFile.Name = string.IsNullOrEmpty (commandParameters.FileDisplayName) ? releaseFile.FileName : commandParameters.FileDisplayName;
      releaseFile.MimeType = commandParameters.MimeType;
      releaseFile.FileType = commandParameters.FileType;
      releaseFile.FileData = File.ReadAllBytes (commandParameters.FileDataPath);

      _service.UploadTheReleaseFiles (
          commandParameters.ProjectName,
          commandParameters.ReleaseName,
          new[] { releaseFile },
          null,
          commandParameters.Username,
          commandParameters.Password);
    }
  }
}