// Copyright (c) 2009 rubicon informationstechnologie gmbh
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
// 
using System;
using System.IO;
using System.Net;
using Remotion.BuildTools.JiraReleaseNoteGenerator.Utilities;

namespace Remotion.BuildTools.JiraReleaseNoteGenerator
{
  public class Program
  {
    private const int WebServiceError = 1;
    private const int XmlTransformerError = 2;

    private static readonly Configuration s_Configuration = Configuration.Current;


    public static int Main (string[] args)
    {
      var argumentCheckResult = CheckArguments (args);
      if (argumentCheckResult != 0)
        return (argumentCheckResult);

      
      var version = args[0];
      var outputDirectory = Path.GetFullPath (args[1]);
      var outputFile = Path.Combine (outputDirectory, "ReleaseNotes_v" + version + ".html");;
      string customJQuery = null;

      if (args.Length == 3)
      {
        customJQuery = Uri.EscapeDataString(args[2]);
      }

      var customConstraints = new CustomConstraints (version, customJQuery);

      Console.Out.WriteLine ("Starting Remotion.BuildTools for version " + version);
      
      var webClient = new NtlmAuthenticatedWebClient ();
      webClient.Credentials = CredentialCache.DefaultNetworkCredentials;
      var requestUrlBuilder = new JiraRequestUrlBuilder (s_Configuration);
      var jiraClient = new JiraClient (webClient, () => requestUrlBuilder);
      var jiraIssueAggregator = new JiraIssueAggregator (jiraClient);
      var xmlTransformer = new XmlTransformer (s_Configuration.XsltStyleSheetPath, s_Configuration.XsltProcessorPath);
      var releaseNoteGenerator = new ReleaseNoteGenerator (s_Configuration, jiraIssueAggregator, xmlTransformer);

      var exitCode = releaseNoteGenerator.GenerateReleaseNotes (customConstraints, outputFile);

      if (exitCode == WebServiceError)
        return 3;

      if (exitCode == XmlTransformerError)
        return 4;

      Console.Out.WriteLine ("Creation of ReleaseNotes for version {0} was successful.", version);

      return 0;
    }


    public static int CheckArguments (string[] arguments)
    {
      ArgumentUtility.CheckNotNull ("arguments", arguments);

      const string usage = "usage: JiraReleaseNoteGenerator versionNumber outputDirectory <additionalConstraint>";

      if (arguments.Length != 2 && arguments.Length != 3)
      {
        Console.Error.WriteLine ("Wrong number of arguments.");
        Console.Error.WriteLine (usage);
        return 1;
      }
      
      if (string.IsNullOrEmpty (arguments[0]))
      {
        Console.Error.WriteLine ("versionNumber must not be empty.");
        Console.Error.WriteLine (usage);
        return 2;
      }

      if (string.IsNullOrEmpty (arguments[1]))
      {
        Console.Error.WriteLine ("outputDirectory must be specified.");
        Console.Error.WriteLine (usage);
        return 2;
      }

      return 0;
    }
  }
}