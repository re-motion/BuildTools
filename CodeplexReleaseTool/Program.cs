using System;
using System.Linq;
using System.Net;
using Remotion.Text.CommandLine;

namespace CodeplexReleaseTool
{
  public class Program
  {
    private static int Main (string[] args)
    {
      if (args.Length == 0)
      {
        ShowUsage();
        return 1;
      }

      var service = new CodeplexWebService (Configuration.Current.Url);
      
      var commandString = args[0];
      var command = GetCommand (commandString, service);

      try
      {
        command.Execute (args.Skip (1).ToArray ());
      }
      catch (CommandLineArgumentException ex)
      {
        Console.Error.WriteLine (ex.Message);
        ShowUsage ();
      }
      catch (Exception ex)
      {
        Console.Error.WriteLine (ex);
        return 1;
      }

      return 0;
    }

    private static void ShowUsage ()
    {
      Console.WriteLine ("Usage:");
      
      var createReleaseParser = new CommandLineClassParser<CreateReleaseParameter> ();
      Console.WriteLine (createReleaseParser.GetAsciiSynopsis (Environment.GetCommandLineArgs ()[0]+" createRelease", System.Console.BufferWidth));

      var uploadReleaseParser = new CommandLineClassParser<UploadReleaseFilesParameter> ();
      Console.WriteLine (uploadReleaseParser.GetAsciiSynopsis (Environment.GetCommandLineArgs ()[0]+" uploadReleaseFiles", System.Console.BufferWidth));
    }

    private static ICommand GetCommand (string commandString, CodeplexWebService service)
    {
      switch (commandString.ToLowerInvariant())
      {
        case "createrelease":
          return new CreateReleaseCommand (service);
        case "uploadreleasefiles":
          return new UploadReleaseFilesCommand (service);
        default:
          throw new NotSupportedException (string.Format ("The command '{0}' is not supported.", commandString));
      }
    }
  }
}
