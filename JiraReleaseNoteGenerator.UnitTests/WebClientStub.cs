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
using Remotion.BuildTools.JiraReleaseNoteGenerator.Utility;

namespace Remotion.BuildTools.JiraReleaseNoteGenerator.UnitTests
{
  public class WebClientStub : IWebClient
  {
    private readonly string _resultStream;

    public WebClientStub (string resultStream)
    {
      ArgumentUtility.CheckNotNull ("resultStream", resultStream);
      _resultStream = resultStream;
    }

    public ICredentials Credentials { get; set; }

    public Stream OpenRead (string address)
    {
      return new MemoryStream (ASCIIEncoding.UTF8.GetBytes (_resultStream));
    }
  }
}