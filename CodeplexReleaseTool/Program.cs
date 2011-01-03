using System;
using System.Linq;

namespace CodeplexReleaseTool
{
  public class Program
  {
    private static int Main (string[] args)
    {
      //TODO
      //if (args.Length == 0)
      //  ShowUsage ();

      var service = new CodeplexWebService (Configuration.Current.Url);

      var commandString = args[0];
      var command = GetCommand (commandString, service);

      try
      {
        command.Execute (args.Skip (1).ToArray ());
      }
      //catch (CommandLineArgumentException ex)
      //{
      //  Console.Error.WriteLine (ex.Message);
      //  //TODO: ShowUsage ();
      //}
      catch (Exception ex)
      {
        Console.Error.WriteLine (ex);
        return 1;
      }

      return 0;
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
