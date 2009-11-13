// Copyright (C) 2005 - 2009 rubicon informationstechnologie gmbh
// All rights reserved.
//
using System;

namespace Remotion.BuildTools.JiraReleaseNoteGenerator.UnitTests
{
  public class JiraRequestUrlBuilderStub : IJiraRequestUrlBuilder
  {
    public string Version { get; set; }
    public string Status { get; set; }
    public string[] Keys { get; set; }
    
    public string Build ()
    {
      throw new NotImplementedException();
    }
  }
}