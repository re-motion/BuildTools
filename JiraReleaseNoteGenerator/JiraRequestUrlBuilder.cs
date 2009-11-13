// Copyright (C) 2005 - 2009 rubicon informationstechnologie gmbh
// All rights reserved.
//
using System.Text;
using Remotion.BuildTools.JiraReleaseNoteGenerator.Utility;

namespace Remotion.BuildTools.JiraReleaseNoteGenerator
{
  public class JiraRequestUrlBuilder :IJiraRequestUrlBuilder
  {
    private readonly Configuration _configuration;

    public string FixVersion { get; set; }
    public string Status { get; set; }
    public string[] Keys { get; set; }

    public JiraRequestUrlBuilder (Configuration configuration)
    {
      ArgumentUtility.CheckNotNull ("configuration", configuration);
      _configuration = configuration;
    }

    public string Build()
    {
      var url = new StringBuilder ();
      url.Append (_configuration.Url);
      url.Append ("/sr/jira.issueviews:searchrequest-xml/temp/SearchRequest.xml?jqlQuery=project+%3D+%22");
      url.Append (_configuration.Project);
      url.Append ("%22");

      if (FixVersion != null)
      {
        url.Append ("+and+fixVersion+%3D+%22");
        url.Append (FixVersion);
        url.Append ("%22");
      }

      if (Status != null)
      {
        url.Append ("+and+status%3D+%22");
        url.Append (Status);
        url.Append ("%22");
      }

      if (Keys != null)
      {
        url.Append ("+and+(");

        for (int i = 0; i < Keys.Length; i++)
        {
          if (i != 0)
            url.Append ("+or");

          url.Append ("+key+%3D+%22");
          url.Append (Keys[i]);
          url.Append ("%22");
        }
        url.Append ("+)");
      }

      url.Append ("&tempMax=1000");

      return url.ToString ();
    }
  }
}