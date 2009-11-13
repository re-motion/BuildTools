// Copyright (C) 2005 - 2009 rubicon informationstechnologie gmbh
// All rights reserved.
//
using System;
using System.Text;

namespace Remotion.BuildTools.JiraReleaseNoteGenerator.UnitTests
{
  public class JiraRequestUrlBuilderStub : IJiraRequestUrlBuilder
  {
    public string FixVersion { get; set; }
    public string Status { get; set; }
    public string[] Keys { get; set; }
    
    public string Build ()
    {
      var url = new StringBuilder();

      if (FixVersion != null)
      {
        url.Append ("v");
        url.Append (FixVersion);
      }
      
      if (Status != null)
      {
        url.Append ("_");
        url.Append (Status);
      }

      if (Keys != null)
      {
        for (int i = 0; i < Keys.Length; i++)
        {
          if (i != 0)
            url.Append ("_");
          url.Append (Keys[i]);
        }
      }

      return url.ToString();
    }
  }
}