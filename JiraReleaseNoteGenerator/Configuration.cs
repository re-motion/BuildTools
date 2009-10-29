// Copyright (C) 2005 - 2009 rubicon informationstechnologie gmbh
// All rights reserved.
//
namespace Remotion.BuildTools.JiraReleaseNoteGenerator
{
  public class Configuration
  {
    public static Configuration Current = new Configuration();

    public string Url { get { return "http://jira.atlassian.com"; } }
    }
}