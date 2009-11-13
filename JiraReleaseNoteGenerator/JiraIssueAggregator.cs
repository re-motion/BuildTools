// This file is part of the re-motion Build Tools (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Build Tools are free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
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
    
    public XDocument GetXml(string version)
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