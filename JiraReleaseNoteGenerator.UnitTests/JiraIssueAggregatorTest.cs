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
using System.Linq;
using System.Xml.Linq;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace Remotion.BuildTools.JiraReleaseNoteGenerator.UnitTests
{
  [TestFixture]
  public class JiraIssueAggregatorTest
  {
    private JiraIssueAggregator _jiraIssueAggregator;

    [SetUp]
    public void SetUp ()
    {
      var webClient = new WebClientStub();
      var jiraClient = new JiraClient (webClient, () => new JiraRequestUrlBuilderStub());

      _jiraIssueAggregator = new JiraIssueAggregator (Configuration.Current, jiraClient);
    }

    [Ignore("Test not finished")]
    [Test]
    public void GetXml ()
    {
      //_jiraClientStub.Stub (stub => stub.GetIssuesAsXml ("v1", "s3", null)).Returns ("firstXmlBlob");
      //_jiraClientStub.Stub (stub=>stub.GetIssuesAsXml (null, null, 1, 2, 3)).Returns ("secondXmlBlob");

      var output = _jiraIssueAggregator.GetXml ("2.0.2");
      var xmlWithoutParents = XDocument.Load (@"..\..\TestDomain\Issues_v2.0.2.xml");
      var xmlOnlyParents = XDocument.Load (@"..\..\TestDomain\Issues_COMMONS-4.xml");
      var expectedOutput = MergeXml (xmlWithoutParents, xmlOnlyParents);

      Assert.That (output, Is.EqualTo (expectedOutput));
    }

    private XDocument MergeXml (XDocument basicDocument, XDocument parentDocument)
    {
      var result = new XDocument (basicDocument);
      result.Root.Elements().First().Add (parentDocument.Root.Elements().First().Elements());
      return result;
    }
  }
}