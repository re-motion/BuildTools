using System;
using System.Linq;
using System.Net;
using System.Threading;
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
      service.Timeout = Timeout.Infinite; 
      
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
        return 1;
      }
      catch (Exception ex)
      {
        Console.Error.WriteLine (ex);
        return 2;
      }

      return 0;
    }

    private static void ShowUsage ()
    {
      Console.WriteLine ("Usage:");

      var exeFileName = Environment.GetCommandLineArgs ()[0];

      var createReleaseParser = new CommandLineClassParser<CreateReleaseParameter> ();
      Console.WriteLine (createReleaseParser.GetAsciiSynopsis (exeFileName + " createRelease", System.Console.BufferWidth));

      var uploadReleaseParser = new CommandLineClassParser<UploadReleaseFilesParameter> ();
      Console.WriteLine (uploadReleaseParser.GetAsciiSynopsis (exeFileName + " uploadReleaseFiles", System.Console.BufferWidth));
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
