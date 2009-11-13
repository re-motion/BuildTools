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
using System.Text;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace Remotion.BuildTools.JiraReleaseNoteGenerator.UnitTests
{
  [TestFixture]
  public class JiraRequestUrlBuilderTest
  {
    private readonly Configuration _configuration = Configuration.Current;
    private StringBuilder _basicUrl;
    private JiraRequestUrlBuilder _jiraRequestUrlBuilder;

    [SetUp]
    public void SetUp ()
    {
      _jiraRequestUrlBuilder = new JiraRequestUrlBuilder (_configuration);

      _basicUrl = new StringBuilder ();
      _basicUrl.Append (_configuration.Url);
      _basicUrl.Append ("/sr/jira.issueviews:searchrequest-xml/temp/SearchRequest.xml?jqlQuery=project+%3D+%22");
      _basicUrl.Append (_configuration.Project);
      _basicUrl.Append ("%22");
    }

    [Test]
    public void CreateRequestUrl_EmptyParameter_ValidUrl ()
    {
      var output = _jiraRequestUrlBuilder.Build ();
      _basicUrl.Append ("&tempMax=1000");
      var expectedOutput = _basicUrl.ToString();

      Assert.That (output, Is.EqualTo (expectedOutput));
    }

    [Test]
    public void CreateRequestUrl_VersionSet_ValidUrl ()
    {
      const string version = "1.2";  

      _jiraRequestUrlBuilder.FixVersion = version;
      _basicUrl.Append ("+and+fixVersion+%3D+%22");
      _basicUrl.Append (version);
      _basicUrl.Append ("%22");
      _basicUrl.Append ("&tempMax=1000");

      var output = _jiraRequestUrlBuilder.Build();
      var expectedOutput = _basicUrl.ToString();

      Assert.That (output, Is.EqualTo (expectedOutput));
    }

    [Test]
    public void CreateRequestUrl_VersionAndStatusSet_ValidUrl ()
    {
      const string version = "1.2";
      const string status = "closed";
      _jiraRequestUrlBuilder.FixVersion = version;
      _jiraRequestUrlBuilder.Status = status;
      _basicUrl.Append ("+and+fixVersion+%3D+%22");
      _basicUrl.Append (version);
      _basicUrl.Append ("%22");
      _basicUrl.Append ("+and+status%3D+%22");
      _basicUrl.Append (status);
      _basicUrl.Append ("%22");
      _basicUrl.Append ("&tempMax=1000");

      var output = _jiraRequestUrlBuilder.Build();
      var expectedOutput = _basicUrl.ToString();

      Assert.That (output, Is.EqualTo (expectedOutput));
    }
    
    [Test]
    public void CreateRequestUrl_OneKeySet_ValidUrl ()
    {
      var keys = new[] { "keyName-111" };
      _basicUrl.Append ("+and+(+key+%3D+%22keyName-111%22+)&tempMax=1000");
      _jiraRequestUrlBuilder.Keys = keys;

      var output = _jiraRequestUrlBuilder.Build ();
      var expectedOutput = _basicUrl.ToString();

      Assert.That (output, Is.EqualTo (expectedOutput));
    }

    [Test]
    public void CreateRequestUrl_TwoKeysSet_ValidUrl ()
    {
      var keys = new[] { "keyName-111", "keyName-112" };
      _jiraRequestUrlBuilder.Keys = keys;
      _basicUrl.Append ("+and+(+key+%3D+%22keyName-111%22+or+key+%3D+%22keyName-112%22+)&tempMax=1000");
      
      var output = _jiraRequestUrlBuilder.Build ();
      var expectedOutput = _basicUrl.ToString();

      Assert.That (output, Is.EqualTo (expectedOutput));
    }

    [Test]
    public void CreateRequestUrl_NothingSet_ValidUrl ()
    {
      _basicUrl.Append ("&tempMax=1000");

      var output = _jiraRequestUrlBuilder.Build ();
      var expectedOutput = _basicUrl.ToString ();

      Assert.That (output, Is.EqualTo (expectedOutput));
    }
  }
}