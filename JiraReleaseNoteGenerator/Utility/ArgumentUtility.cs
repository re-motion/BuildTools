// Copyright (C) 2005 - 2009 rubicon informationstechnologie gmbh
// All rights reserved.
//
using System;

namespace Remotion.BuildTools.JiraReleaseNoteGenerator.Utility
{
  public class ArgumentUtility
  {
    public static void CheckNotNull (string argumentName, object argumentValue)
    {
      if (argumentValue == null)
        throw new ArgumentNullException (argumentName);
    }
  }
}