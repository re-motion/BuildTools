// Copyright (C) 2005 - 2009 rubicon informationstechnologie gmbh
// All rights reserved.
//
using System;
using System.IO;
using System.Xml.Linq;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace Remotion.BuildTools.JiraReleaseNoteGenerator.UnitTests
{
  [TestFixture]
  public class XmlGeneratorTest
  {
    [Test]
    public void CreateUrl ()
    {
      const string version = "2.0.2";
      const string status = "closed";
      var xmlCreator = new XmlGenerator (Configuration.Current);

      var output = xmlCreator.CreateUrl(version, status);
      var expectedOutput = Configuration.Current.Url + "/sr/jira.issueviews:searchrequest-xml/temp/SearchRequest.xml?jqlQuery=project+%3D+%22" + Configuration.Current.Project + "%22+and+fixVersion+%3D+%22" + version + "%22+and+status%3D+%22" + status + "%22&tempMax=1000";

      Assert.That(output, Is.EqualTo(expectedOutput));

    }

    [Test]
    public void GetBasicXmlDocument ()
    {
      const string version = "2.0.2";
      var xmlCreator = new XmlGenerator (Configuration.Current);

      var output = xmlWithoutHeader(xmlCreator.GetBasicXmlDocument(version).ToString());
      var expectedOutput = xmlWithoutHeader (XDocument.Load (@"..\..\TestDomain\SearchRequest.xml").ToString ());

      Assert.That (output, Is.EqualTo (expectedOutput));
    }

    [Test]
    public void ResolveUnknownParentIds ()
    {
      const string version = "2.0.2";
      var xmlCreator = new XmlGenerator (Configuration.Current);

      var output = xmlCreator.ResolveUnknownParentIds (XDocument.Load (@"..\..\TestDomain\SearchRequest.xml"));
      var expectedOutput = new[] { "10003" };

      Assert.That (output, Is.EquivalentTo (expectedOutput));
    }
    
    private string xmlWithoutHeader (string xml)
    {
      return xml.Substring (xml.IndexOf ("-->"));
    }
  }
}