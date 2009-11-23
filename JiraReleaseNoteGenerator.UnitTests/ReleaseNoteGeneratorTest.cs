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
  public class ReleaseNoteGeneratorTest
  {
    private ReleaseNoteGenerator _releaseNoteGenerator;
    private IJiraIssueAggregator _jiraIssueAggregatorStub;

    [SetUp]
    public void SetUp ()
    {
      _jiraIssueAggregatorStub = MockRepository.GenerateStub<IJiraIssueAggregator>();
      _releaseNoteGenerator = new ReleaseNoteGenerator (Configuration.Current, _jiraIssueAggregatorStub);
    }

    [Test]
    public void GenerateReleaseNotes_JiraXmlWithConfigSection ()
    {
      using (var reader = new StreamReader (ResourceManager.GetResourceStream ("Issues_v2.0.2_complete.xml")))
      {
        _jiraIssueAggregatorStub.Stub (stub => stub.GetXml ("2.0.2")).Return (XDocument.Load (reader));
      }

      using (var reader = new StreamReader (ResourceManager.GetResourceStream ("Issues_v2.0.2_withConfig.xml")))
      {
        _releaseNoteGenerator.GenerateReleaseNotes ("2.0.2");
        var output = XDocument.Load ("JiraIssues.xml");
        var expectedOutput = XDocument.Load (reader);

        Assert.That (output.ToString (), Is.EqualTo (expectedOutput.ToString ())); 
      }
    }
  }
}