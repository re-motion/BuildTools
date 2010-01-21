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
  /// <summary> Default implementation of the <see cref="IJiraClient"/>.
  /// </summary>
  /// <example>
  /// The issues returned from JIRA have the following structure:
  /// <![CDATA[
  /// <rss>
  ///   <channel>
  ///     <item>
  ///       <title>
  ///       <project>
  ///       ...
  ///     </item>
  ///     <item>
  ///     ...
  ///    </channel>
  ///  </rss>
  /// ]]>
  /// </example>
  public class JiraClient : IJiraClient
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

    public XDocument GetIssuesByCustomConstraints (CustomConstraints customConstraints)
    {
      ArgumentUtility.CheckNotNull ("customConstraints", customConstraints);

      return GetIssues (customConstraints, null);
    }

    public XDocument GetIssuesByKeys (string[] keys)
    {
      ArgumentUtility.CheckNotNull ("keys", keys);

      return GetIssues (null, keys);
    }

    private XDocument GetIssues (CustomConstraints customConstraints, string[] keys)
    {
      var builder = _builderFactory();
      builder.FixVersion = customConstraints == null ? null : customConstraints.Version;
      builder.Keys = keys;
      builder.AdditionalConstraint = customConstraints == null ? null : customConstraints.AdditionalConstraint;

      if (builder.IsValidQuery())
      {
        var url = builder.Build();
        return GetIssuesFromUrl(url);
      }
      else
      {
        return CreateEmptyDocumentStructure();
      }
    }
    
    private XDocument GetIssuesFromUrl (string url)
    {
      try
      {
        using (var data = _webClient.OpenRead (url))
        {
          using (var reader = new StreamReader (data))
          {
            var result = XDocument.Parse (reader.ReadToEnd());

            if (IsValidXml (result))
              return result;
            else
              return CreateEmptyDocumentStructure();
          }
        }
      }
      catch (WebException ex)
      {
        throw new WebException (ex.Message + "\nURL: " + url);
      }
    }

    private XDocument CreateEmptyDocumentStructure ()
    {
      var result = new XDocument();
      result.Add (new XElement ("rss", new XElement ("channel")));
      return result;
    }

    private bool IsValidXml (XDocument result)
    {
      return result.Root != null
        && result.Element ("rss") != null 
        && result.Element ("rss").Element("channel") != null;
    }
  }
}