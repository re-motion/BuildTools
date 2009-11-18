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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace Remotion.BuildTools.JiraReleaseNoteGenerator.UnitTests
{
  [TestFixture]
  public class ProgramTest
  {
    [Test]
    public void CheckArguments_MissingArgument_False ()
    {
      var result = Program.CheckArguments (new string[] { });

      Assert.That (result, Is.EqualTo (1));
    }

    [Test]
    public void CheckArguments_WrongArgument_False ()
    {
      var result = Program.CheckArguments (new string[] { "" });

      Assert.That (result, Is.EqualTo (2));
    }

    [Test]
    public void CheckArguments_True ()
    {
      var result = Program.CheckArguments (new [] { "2.0.2" });

      Assert.That (result, Is.EqualTo (0));
    }

    [Test]
    public void Main_Stub_ValidVersion_SuccessfulGeneration ()
    {
      Program.RequestUrlBuilder = new JiraRequestUrlBuilderStub();
      Program.WebClient = new WebClientStub();
      var result = Program.Main (new [] {"2.0.2"});

      Assert.That (result, Is.EqualTo (0));
    }

    [Test]
    public void Main_Stub_InvalidVersion_WebException ()
    {
      Program.RequestUrlBuilder = new JiraRequestUrlBuilderStub ();
      Program.WebClient = new WebClientStub ();
      var result = Program.Main (new[] { "2.1.0" });

      Assert.That (result, Is.EqualTo (3));
    }
  }
}