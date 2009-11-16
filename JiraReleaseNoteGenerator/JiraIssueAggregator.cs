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
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using Remotion.BuildTools.JiraReleaseNoteGenerator.Utility;

namespace Remotion.BuildTools.JiraReleaseNoteGenerator
{
  public class JiraIssueAggregator
  {
    private readonly Configuration _configuration;
    private readonly JiraClient _jiraClient;


    public JiraIssueAggregator (Configuration configuration, JiraClient jiraClient)
    {
      ArgumentUtility.CheckNotNull ("configuration", configuration);
      ArgumentUtility.CheckNotNull ("jiraClient", jiraClient);

      _configuration = configuration;
      _jiraClient = jiraClient;
    }

    public XDocument GetXml (string version)
    {
      throw new NotImplementedException();
      //var xmlForVersion = _jiraClient.GetIssuesByVersion(version, "closed");
      //var keys = FindUnknownParentKeys (basicXml);
      //var xmlWithMissingParents = _jiraClient.GetIssuesByKeys (keys);
      //return merge(xmlForVersion, xmlWithMissingParents);
    }


    private string[] FindUnknownParentKeys (XDocument xmlDocument)
    {
      var issueKeyList = new HashSet<string>();
      var parentKeyList = new HashSet<string>();

      var pathNavigator = xmlDocument.CreateNavigator();
      var issueNodeIterator = pathNavigator.Select ("//key");
      var parentNodeIterator = pathNavigator.Select ("//parent");

      while (issueNodeIterator.MoveNext())
        issueKeyList.Add (issueNodeIterator.Current.Value);

      while (parentNodeIterator.MoveNext())
      {
        if (!issueKeyList.Contains (parentNodeIterator.Current.Value))
          parentKeyList.Add (parentNodeIterator.Current.Value);
      }
      return parentKeyList.ToArray();
    }
  }
}