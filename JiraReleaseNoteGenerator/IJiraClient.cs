// Copyright (C) 2005 - 2009 rubicon informationstechnologie gmbh
// All rights reserved.
//
using System.Xml.Linq;

namespace Remotion.BuildTools.JiraReleaseNoteGenerator
{
  public interface IJiraClient
  {
    XDocument GetIssuesByVersion (string version);
    XDocument GetIssuesByKeys (string[] keys);
  }
}