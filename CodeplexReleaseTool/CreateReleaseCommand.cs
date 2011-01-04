using System;
using Remotion.Text.CommandLine;
using Remotion.Utilities;

namespace CodeplexReleaseTool
{
  public class CreateReleaseCommand : ICommand
  {
    private readonly ICodeplexWebService _service;

    public CreateReleaseCommand (ICodeplexWebService service)
    {
      ArgumentUtility.CheckNotNull ("service", service);

      _service = service;
    }


    public void Execute (string[] args)
    {
      ArgumentUtility.CheckNotNull ("args", args);

      var parser = new CommandLineClassParser<CreateReleaseParameter> ();
      var commandParameters = parser.Parse (args);

      _service.CreateARelease (
          commandParameters.ProjectName,
          commandParameters.ReleaseName,
          commandParameters.Description,
          commandParameters.ReleaseDate,
          commandParameters.Status,
          commandParameters.ShowToPublic,
          commandParameters.IsDefaultRelease,
          commandParameters.Username,
          commandParameters.Password);
    }
  }
}