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
      const string status = "Resolved";
      var xmlCreator = new XmlGenerator (Configuration.Current, version);

      var output = xmlCreator.CreateUrl(version, status);
      const string expectedOutput = "";

      Assert.That(output, Is.EqualTo(expectedOutput));

    }
  }
}