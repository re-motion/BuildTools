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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace Remotion.BuildTools.JiraReleaseNoteGenerator.UnitTests
{
  [TestFixture]
  public class XmlTransformerTest
  {
    [Test]
    public void GeneateHtmlFromXml_NonExistingXmlInputFile ()
    {
      // save and redirect standard error
      var standardError = Console.Error;
      var textWriter = new StringWriter();
      Console.SetError (textWriter);

      var transfomer = new XmlTransformer ("invalidFile.xml", "C:/");

      // error code 2 means - source file does not exist
      Assert.That (transfomer.GenerateHtmlFromXml(), Is.EqualTo (2));
      Assert.That (textWriter.ToString(), Is.EqualTo ("Source file invalidFile.xml does not exist\r\n"));

      // restore standard error
      Console.SetError (standardError);
    }

    [Test]
    public void GeneateHtmlFromXml_ValidXmlInputFile ()
    {
      const string documentationDirectory = "output";
      var fileName = Path.Combine (documentationDirectory, "index.html");

      Directory.CreateDirectory (documentationDirectory);
      var transfomer = new XmlTransformer (@"..\..\TestDomain\Issues_v2.0.2_complete.xml", documentationDirectory);

      Assert.That (File.Exists (fileName), Is.False);
      Assert.That (transfomer.GenerateHtmlFromXml(), Is.EqualTo (0));
      Assert.That (File.Exists (fileName), Is.True);

      Directory.Delete (documentationDirectory, true);
    }
  }
}