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
using System.IO;
using System.Net;
using System.Text;
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
      builder.Version = version;
      builder.Status = status;
      builder.Keys = keys;

      var url = builder.Build();

      using (var data = _webClient.OpenRead (url))
      {
        using (var reader = new StreamReader (data))
        {
          return XDocument.Parse (reader.ReadToEnd ());
        }
      }
    }
  }
}