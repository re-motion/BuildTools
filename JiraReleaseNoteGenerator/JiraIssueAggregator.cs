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
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using Remotion.BuildTools.JiraReleaseNoteGenerator.Utilities;

namespace Remotion.BuildTools.JiraReleaseNoteGenerator
{
  /// <summary>
  /// Default implementation of <see cref="IJiraIssueAggregator"/>.
  /// Used by the <see cref="JiraClient"/> to build the request urls for jira.
  /// </summary>
  public class JiraIssueAggregator : IJiraIssueAggregator
  {
    private readonly IJiraClient _jiraClient;

    public JiraIssueAggregator (IJiraClient jiraClient)
    {
      ArgumentUtility.CheckNotNull ("jiraClient", jiraClient);

      _jiraClient = jiraClient;
    }

    public XDocument GetXml (CustomConstraints customConstraints)
    {
      ArgumentUtility.CheckNotNull ("customConstraints", customConstraints);

      var xmlForVersion = _jiraClient.GetIssuesByCustomConstraints (customConstraints);
      var keys = FindUnknownParentKeys (xmlForVersion);
      var xmlWithMissingParents = SetIssuesInvisible(_jiraClient.GetIssuesByKeys (keys));
      
      return MergeXml (xmlForVersion, xmlWithMissingParents);
    }

    private string[] FindUnknownParentKeys (XDocument xmlDocument)
    {
      var pathNavigator = xmlDocument.CreateNavigator();
      var issueKeyList = pathNavigator.Select ("//key").Cast<XPathNavigator>().Select (n => n.Value).Distinct();
      var parentKeyList = pathNavigator.Select ("//parent").Cast<XPathNavigator>().Select (n => n.Value).Distinct();
      return parentKeyList.Except (issueKeyList).ToArray();
    }

    private XDocument MergeXml (XDocument xDocument1, XDocument xDocument2)
    {
      var result = new XDocument (xDocument1);
      result.Element ("rss").Element ("channel").Add (xDocument2.Element ("rss").Element ("channel").Elements("item"));
      return result;
    }

    private XDocument SetIssuesInvisible (XDocument document)
    {
      var tmp = document.ToString ().Replace ("</status>", "</status><invisible/>");
      return XDocument.Parse (tmp);
    }
  }
}