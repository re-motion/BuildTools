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
using System.Net;
using System.Xml.Linq;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace Remotion.BuildTools.JiraReleaseNoteGenerator.UnitTests
{
  [TestFixture]
  public class JiraClientTest
  {
    private JiraClient _jiraClient;
    private readonly JiraRequestUrlBuilder _builder = new JiraRequestUrlBuilder (Configuration.Current);
    private readonly JiraRequestUrlBuilderStub _builderStub = new JiraRequestUrlBuilderStub ();
    private readonly NtlmAuthenticatedWebClient _webClient = new NtlmAuthenticatedWebClient ();
    
    [SetUp]
    public void SetUp ()
    {
      _webClient.Credentials = CredentialCache.DefaultNetworkCredentials;
      var webClientStub = new WebClientStub ();
      _jiraClient = new JiraClient (webClientStub, () => _builderStub);

      // _jiraClient = new JiraClient (_webClient, () => _builder);
    }


    [Test]
    public void JiraClient_GetIssuesByKeys_ValidKey_SuccessfulRequest ()
    {
      var key = new[] { "UUU-116" };
      _jiraClient = new JiraClient (_webClient, () => _builder);

      var output = _jiraClient.GetIssuesByKeys (key);
      var expectedOutput = XDocument.Load (@"..\..\TestDomain\Issues_UUU-116.xml");

      Assert.That (XmlComparisonHelper (output), Is.EqualTo (XmlComparisonHelper (expectedOutput)));
    }

    [Test]
    public void JiraClient_GetIssuesByKeys_InvalidKey_BadRequest ()
    {
      var key = new[] { "UUU-000" };
      _jiraClient = new JiraClient (_webClient, () => _builder);

      try
      {
        _jiraClient.GetIssuesByKeys (key);
        Assert.Fail ("Expected exeption was not thrown");
      }
      catch (System.Net.WebException ex)
      {
        Assert.That (ex.Message, Is.EqualTo ("The remote server returned an error: (400) Bad Request."));
      } 
    }

    [Test]
    public void JiraClient_GetIssuesByVersion_KeyIsNull_ArgumentNotNullException ()
    {
      
      _jiraClient = new JiraClient (_webClient, () => _builder);

      try
      {
        _jiraClient.GetIssuesByVersion (null, null);
        Assert.Fail ("Expected exeption was not thrown");
      }
      catch (ArgumentNullException ex)
      {
        Assert.That (ex.Message, Is.EqualTo ("Value cannot be null.\r\nParameter name: version"));
      }
    }

    [Test]
    public void JiraClient_GetIssuesByKeys_KeyIsNull_ArgumentNotNullException ()
    {
      _jiraClient = new JiraClient (_webClient, () => _builder);

      try
      {
        _jiraClient.GetIssuesByKeys (null);
        Assert.Fail ("Expected exeption was not thrown");
      }
      catch (ArgumentNullException ex)
      {
        Assert.That (ex.Message, Is.EqualTo ("Value cannot be null.\r\nParameter name: keys"));
      }
    }

    [Test]
    public void JiraClient_WebClientStub_GetIssuesByVersion_ValidRequest ()
    {
      var version = "1.2";

      var output = _jiraClient.GetIssuesByVersion (version, null);
      var expectedOutput = XDocument.Load (@"..\..\TestDomain\Issues_v1.2.xml");

      Assert.That (XmlComparisonHelper (output), Is.EqualTo (XmlComparisonHelper (expectedOutput)));
    }

    [Test]
    public void JiraClient_WebClientStub_OneKey_ValidRequest ()
    {
      var key = new[] { "UUU-116" };

      var output = _jiraClient.GetIssuesByKeys(key);
      var expectedOutput = XDocument.Load (@"..\..\TestDomain\Issues_UUU-116.xml");

      Assert.That (XmlComparisonHelper (output), Is.EqualTo (XmlComparisonHelper (expectedOutput)));
    }

    [Test]
    public void JiraClient_WebClientStub_TwoKeys_ValidRequest ()
    {
      var key = new[] { "UUU-111", "UUU-112" };

      var output = _jiraClient.GetIssuesByKeys (key);
      var expectedOutput = XDocument.Load (@"..\..\TestDomain\Issues_UUU-111_UUU-112.xml");

      Assert.That (XmlComparisonHelper (output), Is.EqualTo (XmlComparisonHelper (expectedOutput)));
    }
  

    private string XmlComparisonHelper (XDocument document)
    {
      var documentAsString = document.ToString();
      return documentAsString.Substring (documentAsString.IndexOf ("-->"));
    }
      
  }
}