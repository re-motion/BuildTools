// Copyright (C) 2005 - 2009 rubicon informationstechnologie gmbh
// All rights reserved.
//
namespace Remotion.BuildTools.JiraReleaseNoteGenerator
{
  public class Configuration
  {
    public static Configuration Current = new Configuration();

    public string Url { get { return "http://jira.atlassian.com/sr/jira.issueviews:searchrequest-xml/temp/SearchRequest.xml?jqlQuery=project+%3D+CWD+AND+fixVersion+%3D+%222.0.2%22+AND+status+%3D+Resolved+ORDER+BY+priority+DESC&tempMax=1000"; } }
    }
}