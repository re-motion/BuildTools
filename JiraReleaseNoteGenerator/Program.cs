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
using System.Net;
using Remotion.BuildTools.JiraReleaseNoteGenerator.Utilities;

namespace Remotion.BuildTools.JiraReleaseNoteGenerator
{
  public class Program
  {
    private static IWebClient _webClient;
    private static IJiraRequestUrlBuilder _requestUrlBuilder;

    public static IWebClient WebClient
    {
      get
      {
        if (_webClient == null)
        {
          _webClient = new NtlmAuthenticatedWebClient();
          ((NtlmAuthenticatedWebClient)_webClient).Credentials = CredentialCache.DefaultNetworkCredentials;
        }
        return _webClient;
      }
      set { _webClient = value; }
    }

    public static IJiraRequestUrlBuilder RequestUrlBuilder
    {
      get {
        if (_requestUrlBuilder == null)
          _requestUrlBuilder = new JiraRequestUrlBuilder(Configuration.Current);
        return _requestUrlBuilder; 
      }
      set { _requestUrlBuilder = value; }
    }


    public static int Main (string[] args)
    {
      var argumentCheckResult = CheckArguments (args);
      if (argumentCheckResult != 0)
        return (argumentCheckResult);

      var version = args[0];
      Console.Out.WriteLine ("Starting Remotion.BuildTools for version " + version);

      var client = new JiraClient (WebClient, () => RequestUrlBuilder);
      var jiraClient = client;

      var releaseNoteGenerator = new ReleaseNoteGenerator (Configuration.Current, jiraClient);

      try
      {
        releaseNoteGenerator.GenerateReleaseNotes (args[0]);
        return 0;
      }
      catch (WebException webException)
      {
        Console.Error.Write (webException);
        return 3;
      }
      catch (Exception ex)
      {
        Console.Error.Write (ex);
        return 4;
      }
    }


    public static int CheckArguments (string[] arguments)
    {
      ArgumentUtility.CheckNotNull ("arguments", arguments);

      if (arguments.Length != 1)
      {
        Console.Error.WriteLine ("Wrong number of arguments.");
        Console.Error.WriteLine ("usage: Remotion.BuildTools versionNumber");
        return 1;
      }
      if (string.IsNullOrEmpty (arguments[0]))
      {
        Console.Error.WriteLine ("usage: Remotion.BuildTools versionNumber");
        Console.Error.WriteLine ("versionNumber must not be empty.");
        return 2;
      }

      return 0;
    }
  }
}