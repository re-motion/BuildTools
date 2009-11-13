// Copyright (C) 2005 - 2009 rubicon informationstechnologie gmbh
// All rights reserved.
//
namespace Remotion.BuildTools.JiraReleaseNoteGenerator
{
  public interface IJiraRequestUrlBuilder
  {
    string Version { get; set; }
    string Status { get; set; }
    string[] Keys { get; set; }

    string Build ();
  }
}