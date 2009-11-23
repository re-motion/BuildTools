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
using System.Xml.Linq;
using Remotion.BuildTools.JiraReleaseNoteGenerator.Utilities;

namespace Remotion.BuildTools.JiraReleaseNoteGenerator
{
  public class Program
  {
    private static IWebClient s_WebClient;
    private static IJiraRequestUrlBuilder s_RequestUrlBuilder;
    private static readonly Configuration s_Configuration = Configuration.Current;
    private static string _outputFile;

    public static IWebClient WebClient
    {
      get
      {
        if (s_WebClient == null)
        {
          s_WebClient = new NtlmAuthenticatedWebClient();
          ((NtlmAuthenticatedWebClient) s_WebClient).Credentials = CredentialCache.DefaultNetworkCredentials;
        }
        return s_WebClient;
      }
      set { s_WebClient = value; }
    }

    public static IJiraRequestUrlBuilder RequestUrlBuilder
    {
      get
      {
        if (s_RequestUrlBuilder == null)
          s_RequestUrlBuilder = new JiraRequestUrlBuilder (s_Configuration);
        return s_RequestUrlBuilder;
      }
      set { s_RequestUrlBuilder = value; }
    }

    public static string OutputFile
    {
      get
      {
        if (_outputFile == null)
          _outputFile = s_Configuration.OutputFileName;

        return _outputFile;
      }
      set { _outputFile = value; }
    }


    public static int Main (string[] args)
    {
      var argumentCheckResult = CheckArguments (args);
      if (argumentCheckResult != 0)
        return (argumentCheckResult);

      var version = args[0];
      Console.Out.WriteLine ("Starting Remotion.BuildTools for version " + version);

      var jiraClient = new JiraClient (WebClient, () => RequestUrlBuilder);
      var jiraIssueAggregator = new JiraIssueAggregator (s_Configuration, jiraClient);

      var releaseNoteGenerator = new ReleaseNoteGenerator (s_Configuration, jiraIssueAggregator);
      XDocument releaseNotesXml;

      try
      {
        releaseNotesXml = releaseNoteGenerator.GenerateReleaseNotes (args[0]);
      }
      catch (WebException webException)
      {
        Console.Error.Write (webException);
        return 3;
      }


      var outputDirectory = Path.GetDirectoryName (OutputFile);
      var filename = Path.GetFileName (OutputFile);

      if (!Directory.Exists (outputDirectory))
        Directory.CreateDirectory (outputDirectory);

      var xmlInputFile = Path.Combine (outputDirectory, "JiraIssues_v" + version + ".xml");
      releaseNotesXml.Save (xmlInputFile);

      var outputFile = Path.Combine (outputDirectory, filename);
      var xmlTransformer = new XmlTransformer (xmlInputFile, outputFile);
      var transformerExitCode = xmlTransformer.GenerateHtmlFromXml();

      if (transformerExitCode != 0)
      {
        Console.Error.WriteLine ("Error applying XSLT (code {0})", transformerExitCode);
        return transformerExitCode;
      }

      Console.Out.WriteLine ("Creation of ReleaseNotes for version {0} was successful.", version);

      return 0;
    }


    public static int CheckArguments (string[] arguments)
    {
      ArgumentUtility.CheckNotNull ("arguments", arguments);

      if (arguments.Length != 1)
      {
        Console.Error.WriteLine ("Wrong number of arguments.");
        Console.Error.WriteLine ("usage: JiraReleaseNoteGenerator versionNumber");
        return 1;
      }
      if (string.IsNullOrEmpty (arguments[0]))
      {
        Console.Error.WriteLine ("versionNumber must not be empty.");
        Console.Error.WriteLine ("usage: JiraReleaseNoteGenerator versionNumber");
        return 2;
      }

      return 0;
    }
  }
}