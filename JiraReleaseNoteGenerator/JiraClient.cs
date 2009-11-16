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
using System.Xml.Linq;
using Remotion.BuildTools.JiraReleaseNoteGenerator.Utility;

namespace Remotion.BuildTools.JiraReleaseNoteGenerator
{
  public class JiraClient
  {
    private readonly IWebClient _webClient;
    private readonly Func<IJiraRequestUrlBuilder> _builderFactory;

    public JiraClient (IWebClient webClient, Func<IJiraRequestUrlBuilder> builderFactory)
    {
      ArgumentUtility.CheckNotNull ("webClient", webClient);
      ArgumentUtility.CheckNotNull ("builderFactory", builderFactory);

      _webClient = webClient;
      _builderFactory = builderFactory;
    }

    public XDocument GetIssuesByVersion (string version, string status)
    {
      ArgumentUtility.CheckNotNull ("version", version);

      return GetIssues (version, status, null);
    }

    public XDocument GetIssuesByKeys (string[] keys)
    {
      ArgumentUtility.CheckNotNull ("keys", keys);

      return GetIssues (null, null, keys);
    }

    private XDocument GetIssues (string version, string status, string[] keys)
    {
      var builder = _builderFactory();
      builder.FixVersion = version;
      builder.Status = status;
      builder.Keys = keys;

      if (builder.IsValidQuery ())
      {
        var url = builder.Build();

        using (var data = _webClient.OpenRead (url))
        {
          using (var reader = new StreamReader (data))
          {
            return XDocument.Parse (reader.ReadToEnd());
          }
        }
      }

      var result = new XDocument();
      result.Add (new XElement ("rss", new XElement ("channel")));
      return result;
    }
  }
}