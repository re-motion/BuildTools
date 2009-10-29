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
using Remotion.BuildTools.JiraReleaseNoteGenerator.Utility;

namespace Remotion.BuildTools.JiraReleaseNoteGenerator
{
  public class XmlGenerator
  {
    private readonly Configuration _configuration;
    private readonly string _version;

    public XmlGenerator (Configuration configuration, string version)
    {
      ArgumentUtility.CheckNotNull ("configuration", configuration);
      ArgumentUtility.CheckNotNull ("version", version);

      _configuration = configuration;
      _version = version;
    }

    public string CreateUrl (string version, string status)
    {
      return "";
    }

  }
}