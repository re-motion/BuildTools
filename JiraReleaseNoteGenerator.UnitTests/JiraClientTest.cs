// Copyright (C) 2005 - 2009 rubicon informationstechnologie gmbh
// All rights reserved.
//
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

      _basicUrl = new StringBuilder ();
      _basicUrl.Append (_configuration.Url);
      _basicUrl.Append ("/sr/jira.issueviews:searchrequest-xml/temp/SearchRequest.xml?jqlQuery=project+%3D+%22");
      _basicUrl.Append (_configuration.Project);
      _basicUrl.Append ("%22");
    }

    [Test]
    public void JiraClient_VersionSet_ValidRequst ()
    {
      const string version = "1.2";

      var output = _jiraClient.GetIssuesAsXml (version, null, null);
      var expectedOutput = XDocument.Load (@"..\..\TestDomain\IssuesForVersion_1.2.xml");

      Assert.That(output, Is.EqualTo(expectedOutput));
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
      var expectedOutput = _basicUrl.ToString ();

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
      var expectedOutput = _basicUrl.ToString ();

      Assert.That (output, Is.EqualTo (expectedOutput));
    }

    [Test]
    public void CreateRequestUrl_OneKeySet_ValidUrl ()
    {
      var keys = new [] {"keyName-111"};

      var output = _jiraClient.CreateRequestUrl (null, null, keys);
      _basicUrl.Append ("+and+(+key+%3D+%22keyName-111%22+)&tempMax=1000");
      var expectedOutput = _basicUrl.ToString ();

      Assert.That (output, Is.EqualTo (expectedOutput));
    }

    [Test]
    public void CreateRequestUrl_TwoKeysSet_ValidUrl ()
    {
      var keys = new[] { "keyName-111", "keyName-112" };

      var output = _jiraClient.CreateRequestUrl (null, null, keys);
      _basicUrl.Append ("+and+(+key+%3D+%22keyName-111%22+or+key+%3D+%22keyName-112%22+)&tempMax=1000");
      var expectedOutput = _basicUrl.ToString ();

      Assert.That (output, Is.EqualTo (expectedOutput));
    }
  }
}