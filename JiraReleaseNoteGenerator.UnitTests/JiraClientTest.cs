// Copyright (c) 2009 rubicon informationstechnologie gmbh
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
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
    private readonly JiraRequestUrlBuilderStub _builderStub = new JiraRequestUrlBuilderStub();
    private readonly NtlmAuthenticatedWebClient _webClient = new NtlmAuthenticatedWebClient();

    [SetUp]
    public void SetUp ()
    {
      _webClient.Credentials = CredentialCache.DefaultNetworkCredentials;
      var webClientStub = new WebClientStub();
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
      catch (WebException ex)
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

      var output = _jiraClient.GetIssuesByKeys (key);
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