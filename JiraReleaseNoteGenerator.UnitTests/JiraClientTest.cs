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
using System.Xml.Linq;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace Remotion.BuildTools.JiraReleaseNoteGenerator.UnitTests
{
  [TestFixture]
  public class JiraClientTest
  {
    private JiraClient _jiraClient;
    private readonly Configuration _configuration = Configuration.Current;
    private StringBuilder _basicUrl;

    [SetUp]
    public void SetUp ()
    {
      _jiraClient = new JiraClient (_configuration, new NtlmAuthenticatedWebClient());

      _basicUrl = new StringBuilder();
      _basicUrl.Append (_configuration.Url);
      _basicUrl.Append ("/sr/jira.issueviews:searchrequest-xml/temp/SearchRequest.xml?jqlQuery=project+%3D+%22");
      _basicUrl.Append (_configuration.Project);
      _basicUrl.Append ("%22");
    }


    [Test]
    public void JiraClient_ValidKey_SuccessfulRequest ()
    {
      var key = new[] { "UUU-116" };

      var output = _jiraClient.GetIssuesAsXml (null, null, key);
      var expectedOutput = XDocument.Load (@"..\..\TestDomain\IssuesForKey_UUU-116.xml");

      Assert.That (XmlComparisonHelper (output), Is.EqualTo (XmlComparisonHelper (expectedOutput)));
    }

    [Test]
    public void JiraClient_InvalidKey_BadRequest ()
    {
      var key = new[] { "UUU-000" };

      try
      {
        var output = _jiraClient.GetIssuesAsXml (null, null, key);
        Assert.Fail ("Expected exeption 'The remote server returned an error: (400) Bad Request.' was not thrown");
      }
      catch (System.Net.WebException ex)
      {
        Assert.That (ex.Message, Is.EqualTo ("The remote server returned an error: (400) Bad Request."));
      }
      
    }

    [Test]
    public void JiraClient_WebClientStub_ValidRequest ()
    {
      var sourceFile = XDocument.Load (@"..\..\TestDomain\IssuesForKey_UUU-116.xml");
      var webClientStub = new WebClientStub (sourceFile.ToString());
      var jiraClient = new JiraClient (_configuration, webClientStub);
      var key = new[] { "UUU-116" };

      var output = jiraClient.GetIssuesAsXml (null, null, key);
      var expectedOutput = XDocument.Load (@"..\..\TestDomain\IssuesForKey_UUU-116.xml");

      Assert.That (XmlComparisonHelper (output), Is.EqualTo (XmlComparisonHelper (expectedOutput)));
    }

    [Test]
    public void CreateRequestUrl_EmptyParameter_ValidUrl ()
    {
      var output = _jiraClient.CreateRequestUrl (null, null, null);
      _basicUrl.Append ("&tempMax=1000");
      var expectedOutput = _basicUrl.ToString();

      Assert.That (output, Is.EqualTo (expectedOutput));
    }

    [Test]
    public void CreateRequestUrl_VersionSet_ValidUrl ()
    {
      const string version = "1.2";
      var output = _jiraClient.CreateRequestUrl (version, null, null);
      _basicUrl.Append ("+and+fixVersion+%3D+%22");
      _basicUrl.Append (version);
      _basicUrl.Append ("%22");
      _basicUrl.Append ("&tempMax=1000");
      var expectedOutput = _basicUrl.ToString();

      Assert.That (output, Is.EqualTo (expectedOutput));
    }

    [Test]
    public void CreateRequestUrl_StatusSet_ValidUrl ()
    {
      const string status = "closed";
      var output = _jiraClient.CreateRequestUrl (null, status, null);
      _basicUrl.Append ("+and+status%3D+%22");
      _basicUrl.Append (status);
      _basicUrl.Append ("%22");
      _basicUrl.Append ("&tempMax=1000");
      var expectedOutput = _basicUrl.ToString();

      Assert.That (output, Is.EqualTo (expectedOutput));
    }

    [Test]
    public void CreateRequestUrl_OneKeySet_ValidUrl ()
    {
      var keys = new[] { "keyName-111" };

      var output = _jiraClient.CreateRequestUrl (null, null, keys);
      _basicUrl.Append ("+and+(+key+%3D+%22keyName-111%22+)&tempMax=1000");
      var expectedOutput = _basicUrl.ToString();

      Assert.That (output, Is.EqualTo (expectedOutput));
    }

    [Test]
    public void CreateRequestUrl_TwoKeysSet_ValidUrl ()
    {
      var keys = new[] { "keyName-111", "keyName-112" };

      var output = _jiraClient.CreateRequestUrl (null, null, keys);
      _basicUrl.Append ("+and+(+key+%3D+%22keyName-111%22+or+key+%3D+%22keyName-112%22+)&tempMax=1000");
      var expectedOutput = _basicUrl.ToString();

      Assert.That (output, Is.EqualTo (expectedOutput));
    }

    private string XmlComparisonHelper (XDocument document)
    {
      var documentAsString = document.ToString();
      return documentAsString.Substring (documentAsString.IndexOf ("-->"));
    }
  }
}