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
using System.IO;
using System.Xml.Linq;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.BuildTools.JiraReleaseNoteGenerator.UnitTests.TestDomain;
using Rhino.Mocks;

namespace Remotion.BuildTools.JiraReleaseNoteGenerator.UnitTests
{
  [TestFixture]
  public class JiraIssueAggregatorTest
  {
    private JiraIssueAggregator _jiraIssueAggregator;
    private IJiraClient _jiraClientStub;

    [SetUp]
    public void SetUp ()
    {
      _jiraClientStub = MockRepository.GenerateStub<IJiraClient>();
      _jiraIssueAggregator = new JiraIssueAggregator (_jiraClientStub);
    }

    [Test]
    public void GetXml_VersionWithMissingParents ()
    {
      var constraints = new CustomConstraints ("2.0.2", null);

      using (var reader = new StreamReader (ResourceManager.GetResourceStream ("Issues_v2.0.2.xml")))
      {
        _jiraClientStub.Stub (stub => stub.GetIssuesByCustomConstraints (constraints)).Return (XDocument.Load (reader));
      }
      using (var reader = new StreamReader (ResourceManager.GetResourceStream ("Issues_COMMONS-4.xml")))
      {
        _jiraClientStub.Stub (stub => stub.GetIssuesByKeys (new[] { "COMMONS-4" })).Return (XDocument.Load (reader));
      }

      using (var resultReader = new StreamReader (ResourceManager.GetResourceStream ("Issues_v2.0.2_complete.xml")))
      {
        var output = _jiraIssueAggregator.GetXml (constraints);
        var expectedOutput = XDocument.Load (resultReader);

        Assert.That (output.ToString(), Is.EqualTo (expectedOutput.ToString()));
      }
    }

    [Test]
    public void GetXml_VersionWithoutMissingParents ()
    {
      using (var reader = new StreamReader (ResourceManager.GetResourceStream ("Issues_v1.2_closed.xml")))
      {
        var xmlFile = XDocument.Load (reader);
        var constraints = new CustomConstraints ("1.2", null);
        _jiraClientStub.Stub (stub => stub.GetIssuesByCustomConstraints (constraints)).Return (xmlFile);
        var emptyXml = new XDocument (new XElement ("rss", new XElement ("channel")));
        _jiraClientStub.Stub (stub => stub.GetIssuesByKeys (new string[0])).Return (emptyXml);

        var output = _jiraIssueAggregator.GetXml (constraints);
        var expectedOutput = xmlFile;

        Assert.That (output.ToString(), Is.EqualTo (expectedOutput.ToString()));
      }
    }
  }
}