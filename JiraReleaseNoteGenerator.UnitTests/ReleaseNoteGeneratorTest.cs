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
  public class ReleaseNoteGeneratorTest
  {
    private ReleaseNoteGenerator _releaseNoteGenerator;

    [SetUp]
    public void SetUp ()
    {
      var webClient = new WebClientStub ();
      var jiraClient = new JiraClient (webClient, () => new JiraRequestUrlBuilderStub ());

      _releaseNoteGenerator = new ReleaseNoteGenerator (Configuration.Current, jiraClient);
    }

    [Test]
    public void GenerateReleaseNotes_JiraXmlWithConfigSection ()
    {
      _releaseNoteGenerator.GenerateReleaseNotes("2.0.2");
      var output = XDocument.Load ("JiraIssues.xml");
      var expectedOutput = XDocument.Load ("Issues_v2.0.2_withConfig.xml");

      Assert.That (output.ToString(), Is.EqualTo (expectedOutput.ToString()));
    }

    [Test]
    public void GenerateReleaseNotes_JiraHtmlOutput ()
    {
    } 
  }
}