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
using Remotion.BuildTools.JiraReleaseNoteGenerator.UnitTests.TestDomain;
using Rhino.Mocks;

namespace Remotion.BuildTools.JiraReleaseNoteGenerator.UnitTests
{
  [TestFixture]
  public class JiraClientTest
  {
    private JiraClient _jiraClient;
    private IJiraRequestUrlBuilder _builderMock;
    private IWebClient _webClientStub;

    [SetUp]
    public void SetUp ()
    {
      _builderMock = MockRepository.GenerateMock<IJiraRequestUrlBuilder>();
      _webClientStub = MockRepository.GenerateStub<IWebClient>();
      _jiraClient = new JiraClient (_webClientStub, () => _builderMock);
    }

    [Test]
    public void GetIssuesByVersion_BuildsUrlFromInputParameters()
    {
      using (_builderMock.GetMockRepository().Ordered())
      {
        _builderMock.Expect (mock => mock.FixVersion = "1.2");
        _builderMock.Expect (mock => mock.Keys = null);
        _builderMock.Expect (mock => mock.AdditionalConstraint = null);
        _builderMock.Expect (mock => mock.IsValidQuery()).Return (true);
        _builderMock.Expect (mock => mock.Build()).Return ("JiraUrl");
      }
      
      using (var stream = ResourceManager.GetResourceStream ("Issues_v1.2.xml"))
      {
        _webClientStub.Stub (stub => stub.OpenRead ("JiraUrl")).Return (stream);

        _jiraClient.GetIssuesByCustomConstraints (new CustomConstraints ("1.2", null));
      }

      _builderMock.VerifyAllExpectations();
    }

    [Test]
    public void GetIssuesByVersionWithAdditionalConstraint_BuildsUrlFromInputParameters ()
    {
      using (_builderMock.GetMockRepository ().Ordered ())
      {
        _builderMock.Expect (mock => mock.FixVersion = "1.2");
        _builderMock.Expect (mock => mock.Keys = null);
        _builderMock.Expect (mock => mock.AdditionalConstraint = "abc");
        _builderMock.Expect (mock => mock.IsValidQuery ()).Return (true);
        _builderMock.Expect (mock => mock.Build ()).Return ("JiraUrl");
      }

      using (var stream = ResourceManager.GetResourceStream ("Issues_v1.2.xml"))
      {
        _webClientStub.Stub (stub => stub.OpenRead ("JiraUrl")).Return (stream);

        _jiraClient.GetIssuesByCustomConstraints (new CustomConstraints ("1.2", "abc"));
      }

      _builderMock.VerifyAllExpectations ();
    }

    [Test]
    public void GetIssuesByKeys_BuildsUrlFromInputParameters ()
    {
      using (_builderMock.GetMockRepository ().Ordered ())
      {
        _builderMock.Expect (mock => mock.FixVersion = null);
        _builderMock.Expect (mock => mock.Keys = new[] { "UUU-116" });
        _builderMock.Expect (mock => mock.AdditionalConstraint = null);
        _builderMock.Expect (mock => mock.IsValidQuery ()).Return (true);
        _builderMock.Expect (mock => mock.Build ()).Return ("JiraUrl");
      }

      using (var stream = ResourceManager.GetResourceStream ("Issues_v1.2.xml"))
      {
        _webClientStub.Stub (stub => stub.OpenRead ("JiraUrl")).Return (stream);

        _jiraClient.GetIssuesByKeys (new[] { "UUU-116" });
      }

      _builderMock.VerifyAllExpectations ();
    }

    [Test]
    public void GetIssuesByVersion_ReturnsStreamForUrl ()
    {
      _builderMock.Expect (mock => mock.IsValidQuery ()).Return (true);
      _builderMock.Stub (mock => mock.Build ()).Return ("JiraUrl");

      using (var stream = ResourceManager.GetResourceStream ("Issues_v1.2.xml"))
      {
        _webClientStub.Stub (stub => stub.OpenRead ("JiraUrl")).Return (stream);
        var output = _jiraClient.GetIssuesByCustomConstraints (new CustomConstraints ("1.2", null));

        using (var reader = new StreamReader (ResourceManager.GetResourceStream ("Issues_v1.2.xml")))
        {
          var expectedOutput = XDocument.Load (reader);
          Assert.That (output.ToString(), Is.EqualTo (expectedOutput.ToString()));
        }
      }
    }

    [Test]
    public void GetIssuesByKeys_ReturnsStreamForUrl ()
    {
      _builderMock.Expect (mock => mock.IsValidQuery ()).Return (true);
      _builderMock.Stub (mock => mock.Build ()).Return ("JiraUrl");

      using (var stream = ResourceManager.GetResourceStream ("Issues_UUU-116.xml"))
      {
        _webClientStub.Stub (stub => stub.OpenRead ("JiraUrl")).Return (stream);
        var output = _jiraClient.GetIssuesByKeys (new[] { "UUU-116"});

        using (var reader = new StreamReader (ResourceManager.GetResourceStream ("Issues_UUU-116.xml")))
        {
          var expectedOutput = XDocument.Load (reader);
          Assert.That (output.ToString (), Is.EqualTo (expectedOutput.ToString ()));
        }
      }
    }

  }
}