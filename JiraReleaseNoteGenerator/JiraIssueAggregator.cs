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
    private const string CLOSED = "closed";
    private readonly Configuration _configuration;
    // private readonly IWebClient _webClient;

    public JiraIssueAggregator (Configuration configuration)
    {
      ArgumentUtility.CheckNotNull ("configuration", configuration);
      _configuration = configuration;
    }

    //public JiraIssueAggregator (Configuration configuration, IWebClient webClient)
    //{
    //  ArgumentUtility.CheckNotNull ("configuration", configuration);
    //  ArgumentUtility.CheckNotNull ("webClient", webClient);

    //  _configuration = configuration;
    //  _webClient = webClient;
    //}

    public string CreateUrl (string version, string status, string[] key)
    {
      var url = new StringBuilder();
      url.Append (_configuration.Url);
      url.Append ("/sr/jira.issueviews:searchrequest-xml/temp/SearchRequest.xml?jqlQuery=project+%3D+%22");
      url.Append (_configuration.Project);
      url.Append ("%22");

      if (version != null)
      {
        url.Append ("+and+fixVersion+%3D+%22");
        url.Append (version);
        url.Append ("%22");
      }

      if (status != null)
      {
        url.Append ("+and+status%3D+%22");
        url.Append (status);
        url.Append ("%22");
      }

      if (key != null)
      {
        url.Append ("+and+(");

        for (int i = 0; i < key.Length; i++)
        {
          if (i != 0) 
            url.Append ("+or");
          
          url.Append ("+key%3D+%22");
          url.Append (key[i]);
          url.Append ("%22");
        }
        url.Append ("+)");
      }

      url.Append ("&tempMax=1000");

      return url.ToString();
    }

    public XDocument GetIssuesForVersion (string version)
    {
      var url = CreateUrl (version, CLOSED, null);
      
      return GetXmlWebClientRequest (url);
    }

    public XDocument GetIssuesForKeys (string[] keys)
    {
      var url = CreateUrl (null, null, keys);

      return GetXmlWebClientRequest(url);
    }

    public string[] FindUnknownParentKeys (XDocument xmlDocument)
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

    private XDocument GetXmlWebClientRequest (string url)
    {
      var client = new NtlmAuthenticatedWebClient ();
      client.Credentials = CredentialCache.DefaultNetworkCredentials;

      using (var data = client.OpenRead (url))
      {
        using (var reader = new StreamReader (data))
        {
          return XDocument.Parse (reader.ReadToEnd ());
        }
      }
    }
  }
}