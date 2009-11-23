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
using System.Text;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace Remotion.BuildTools.JiraReleaseNoteGenerator.UnitTests
{
  [TestFixture]
  public class JiraRequestUrlBuilderTest
  {
    private readonly Configuration _configuration = Configuration.Current;
    private StringBuilder _basicUrl;
    private JiraRequestUrlBuilder _jiraRequestUrlBuilder;

    [SetUp]
    public void SetUp ()
    {
      _jiraRequestUrlBuilder = new JiraRequestUrlBuilder (_configuration);

      _basicUrl = new StringBuilder();
      _basicUrl.Append (_configuration.Url);
      _basicUrl.Append ("/sr/jira.issueviews:searchrequest-xml/temp/SearchRequest.xml?jqlQuery=project+%3D+%22");
      _basicUrl.Append (_configuration.Project);
      _basicUrl.Append ("%22");
    }

    [Test]
    public void CreateRequestUrl_EmptyParameter_ValidUrl ()
    {
      var output = _jiraRequestUrlBuilder.Build();
      _basicUrl.Append ("&tempMax=1000");
      var expectedOutput = _basicUrl.ToString();

      Assert.That (output, Is.EqualTo (expectedOutput));
    }

    [Test]
    public void CreateRequestUrl_VersionSet_ValidUrl ()
    {
      const string version = "1.2";

      _jiraRequestUrlBuilder.FixVersion = version;
      _basicUrl.Append ("+and+fixVersion+%3D+%22");
      _basicUrl.Append (version);
      _basicUrl.Append ("%22");
      _basicUrl.Append ("&tempMax=1000");

      var output = _jiraRequestUrlBuilder.Build();
      var expectedOutput = _basicUrl.ToString();

      Assert.That (output, Is.EqualTo (expectedOutput));
    }

    [Test]
    public void CreateRequestUrl_OneKeySet_ValidUrl ()
    {
      var keys = new[] { "keyName-111" };
      _basicUrl.Append ("+and+(+key+%3D+%22keyName-111%22+)&tempMax=1000");
      _jiraRequestUrlBuilder.Keys = keys;

      var output = _jiraRequestUrlBuilder.Build();
      var expectedOutput = _basicUrl.ToString();

      Assert.That (output, Is.EqualTo (expectedOutput));
    }

    [Test]
    public void CreateRequestUrl_TwoKeysSet_ValidUrl ()
    {
      var keys = new[] { "keyName-111", "keyName-112" };
      _jiraRequestUrlBuilder.Keys = keys;
      _basicUrl.Append ("+and+(+key+%3D+%22keyName-111%22+or+key+%3D+%22keyName-112%22+)&tempMax=1000");

      var output = _jiraRequestUrlBuilder.Build();
      var expectedOutput = _basicUrl.ToString();

      Assert.That (output, Is.EqualTo (expectedOutput));
    }

    [Test]
    public void CreateRequestUrl_NothingSet_ValidUrl ()
    {
      _basicUrl.Append ("&tempMax=1000");

      var output = _jiraRequestUrlBuilder.Build();
      var expectedOutput = _basicUrl.ToString();

      Assert.That (output, Is.EqualTo (expectedOutput));
    }
  }
}