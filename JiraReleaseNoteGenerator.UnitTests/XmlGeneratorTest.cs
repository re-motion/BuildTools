// Copyright (C) 2005 - 2009 rubicon informationstechnologie gmbh
// All rights reserved.
//
using System;
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
      var xmlCreator = new XmlGenerator (Configuration.Current, version);

      var output = xmlCreator.CreateUrl(version, status);
      var expectedOutput = Configuration.Current.Url + "/sr/jira.issueviews:searchrequest-xml/temp/SearchRequest.xml?jqlQuery=project+%3D+%22" + Configuration.Current.Project + "%22+and+fixVersion+%3D+%22" + version + "%22+and+status%3D+%22" + status + "%22";

      Assert.That(output, Is.EqualTo(expectedOutput));

    }
  }
}