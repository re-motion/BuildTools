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
using System.Text;
using System.Xml.Linq;
using Remotion.BuildTools.JiraReleaseNoteGenerator.Utility;

namespace Remotion.BuildTools.JiraReleaseNoteGenerator
{
  public class JiraClient
  {
    private readonly Configuration _configuration;
    private readonly IWebClient _webClient;

    public JiraClient (Configuration configuration, IWebClient webClient)
    {
      ArgumentUtility.CheckNotNull ("configuration", configuration);
      ArgumentUtility.CheckNotNull ("webClient", webClient);

      _configuration = configuration;
      _webClient = webClient;
    }

    public XDocument GetIssuesAsXml (string version, string status, string[] keys)
    {
      throw new NotImplementedException();
    }

    public string CreateRequestUrl (string version, string status, string[] keys)
    {
      var url = new StringBuilder ();
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

      if (keys != null)
      {
        url.Append ("+and+(");

        for (int i = 0; i < keys.Length; i++)
        {
          if (i != 0)
            url.Append ("+or");

          url.Append ("+key+%3D+%22");
          url.Append (keys[i]);
          url.Append ("%22");
        }
        url.Append ("+)");
      }

      url.Append ("&tempMax=1000");

      return url.ToString ();
    }
  }
}