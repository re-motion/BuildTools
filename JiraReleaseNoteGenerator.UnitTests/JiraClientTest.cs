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
    private JiraRequestUrlBuilder _builder;

    [SetUp]
    public void SetUp ()
    {
      var webClient = new NtlmAuthenticatedWebClient ();
      webClient.Credentials = CredentialCache.DefaultNetworkCredentials;
      _builder = new JiraRequestUrlBuilder(_configuration);
      _jiraClient = new JiraClient (webClient, ()=>_builder);
    }


    [Test]
    public void JiraClient_ValidKey_SuccessfulRequest ()
    {
      var key = new[] { "UUU-116" };

      var output = _jiraClient.GetIssuesByKeys (key);
      var expectedOutput = XDocument.Load (@"..\..\TestDomain\IssuesForKey_UUU-116.xml");

      Assert.That (XmlComparisonHelper (output), Is.EqualTo (XmlComparisonHelper (expectedOutput)));
    }

    [Test]
    public void JiraClient_InvalidKey_BadRequest ()
    {
      var key = new[] { "UUU-000" };

      try
      {
        var output = _jiraClient.GetIssuesByKeys (key);
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
      var urlBuilderStub = new JiraRequestUrlBuilder(_configuration);
      //var urlBuilderStub = new JiraRequestUrlBuilderStub();
      var jiraClient = new JiraClient (webClientStub, ()=>urlBuilderStub);
      var key = new[] { "UUU-116" };

      var output = jiraClient.GetIssuesByKeys(key);
      var expectedOutput = XDocument.Load (@"..\..\TestDomain\IssuesForKey_UUU-116.xml");

      Assert.That (XmlComparisonHelper (output), Is.EqualTo (XmlComparisonHelper (expectedOutput)));
    }
  

    private string XmlComparisonHelper (XDocument document)
    {
      var documentAsString = document.ToString();
      return documentAsString.Substring (documentAsString.IndexOf ("-->"));
    }
      
  }
}