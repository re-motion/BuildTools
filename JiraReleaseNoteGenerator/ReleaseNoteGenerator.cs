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
using System.Xml.Linq;
using Remotion.BuildTools.JiraReleaseNoteGenerator.Utilities;

namespace Remotion.BuildTools.JiraReleaseNoteGenerator
{
  public class ReleaseNoteGenerator
  {
    private readonly Configuration _configuration;
    private readonly IJiraIssueAggregator _jiraIssueAggregator;
    private readonly IXmlTransformer _xmlTransformer;

    public ReleaseNoteGenerator (Configuration configuration, IJiraIssueAggregator jiraIssueAggregator, IXmlTransformer xmlTransformer)
    {
      ArgumentUtility.CheckNotNull ("configuration", configuration);
      ArgumentUtility.CheckNotNull ("jiraIssueAggregator", jiraIssueAggregator);
      ArgumentUtility.CheckNotNull ("xmlTransformer", xmlTransformer);

      _configuration = configuration;
      _jiraIssueAggregator = jiraIssueAggregator;
      _xmlTransformer = xmlTransformer;
    }

    public int GenerateReleaseNotes (string version, string outputFile)
    {
      ArgumentUtility.CheckNotNull ("version", version);
      ArgumentUtility.CheckNotNull ("outputFile", outputFile);

      XDocument issues;

      try
      {
        issues = _jiraIssueAggregator.GetXml (version);
        var config = XDocument.Load (_configuration.ConfigFile);
        issues.Root.AddFirst (config.Elements());
      }
      catch (WebException webException)
      {
        Console.Error.Write (webException);
        return 1;
      }

      var transformerExitCode = _xmlTransformer.GenerateHtmlFromXml (issues, outputFile);

      return transformerExitCode;
    }
  }
}