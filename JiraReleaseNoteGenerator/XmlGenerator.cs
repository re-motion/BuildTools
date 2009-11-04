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
  public class XmlGenerator
  {
    private readonly Configuration _configuration;

    public XmlGenerator (Configuration configuration)
    {
      ArgumentUtility.CheckNotNull ("configuration", configuration);

      _configuration = configuration;
    }

    public string CreateUrl (string version, string status)
    {
      var url = new StringBuilder();
      url.Append (_configuration.Url);
      url.Append ("/sr/jira.issueviews:searchrequest-xml/temp/SearchRequest.xml?jqlQuery=project+%3D+%22");
      url.Append (_configuration.Project);
      url.Append ("%22+and+fixVersion+%3D+%22");
      url.Append (version);
      url.Append ("%22+and+status%3D+%22");
      url.Append (status);
      url.Append ("%22&tempMax=1000");
      return url.ToString();
    }

    public XDocument GetBasicXmlDocument (string version)
    {
      var url = CreateUrl (version, "closed");
      var xmlResult = "";

      var request = WebRequest.Create (url);
      request.Headers.Add ("X-AUTH-USER", "RUBICON\\patrick.groess");

      using (var response = request.GetResponse())
      {
        using (var dataStream = response.GetResponseStream())
        {
          using (var reader = new StreamReader (dataStream))
          {
            return XDocument.Parse (reader.ReadToEnd());
          }
        }
      }
    }

    public string[] ResolveUnknownParentIds (XDocument xmlDocument)
    {
      var issueKeyList = new HashSet<string>();
      var parentKeyList = new HashSet<string>();

      var pathNavigator = GetXPathDocument (xmlDocument).CreateNavigator();
      var issueNodeIterator = pathNavigator.Select ("//key/@id");
      var parentNodeIterator = pathNavigator.Select ("//parent/@id");

      while (issueNodeIterator.MoveNext())
        issueKeyList.Add (issueNodeIterator.Current.Value);
      
      while (parentNodeIterator.MoveNext())
      {
        if (!issueKeyList.Contains (parentNodeIterator.Current.Value))
          parentKeyList.Add (parentNodeIterator.Current.Value);
      }
      
      return parentKeyList.ToArray();
    }

    private XPathDocument GetXPathDocument (XDocument xmlDocument)
    {
      xmlDocument.Save ("tmpRequest.xml");
      var docNav = new XPathDocument (@"tmpRequest.xml");
      File.Delete ("tmpRequest");
      return docNav;
    }
  }
}