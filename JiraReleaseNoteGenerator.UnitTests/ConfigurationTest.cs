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
using System.Configuration;
using NUnit.Framework;
using Remotion.Development.UnitTesting.Configuration;

namespace Remotion.BuildTools.JiraReleaseNoteGenerator.UnitTests
{
  [TestFixture]
  public class ConfigurationTest
  {
    [Test]
    public void Deserialize ()
    {
      var configuration = new Configuration();
      const string xmlFragment =
          @"<root project = ""myProject"" 
            xsltConfigFile = ""myfile.cfg"" 
            url = ""http://myJira"" 
            xsltStyleSheetPath = ""myStyleSheet.xslt"" 
            xsltProcessorPath = ""myXsltProcessor.exe""
          />";

      ConfigurationHelper.DeserializeSection (configuration, xmlFragment);

      Assert.That (configuration.Project, Is.EqualTo ("myProject"));
      Assert.That (configuration.ConfigFile, Is.EqualTo ("myfile.cfg"));
      Assert.That (configuration.Url, Is.EqualTo ("http://myJira"));
      Assert.That (configuration.XsltStyleSheetPath, Is.EqualTo ("myStyleSheet.xslt"));
      Assert.That (configuration.XsltProcessorPath, Is.EqualTo ("myXsltProcessor.exe"));
    }

    [Test]
    public void Deserialize_MissingUrl ()
    {
      var configuration = new Configuration();
      const string xmlFragment = @"<root project = ""myProject"" />";

      try
      {
        ConfigurationHelper.DeserializeSection (configuration, xmlFragment);
        Assert.Fail ("Expected exception not thrown.");
      }
      catch (ConfigurationErrorsException ex)
      {
        Assert.That (ex.Message, Is.EqualTo ("Required attribute 'url' not found."));
      }
    }

    [Test]
    public void Deserialize_MissingProject ()
    {
      var configuration = new Configuration ();
      const string xmlFragment = @"<root url = ""http://myJira"" />";

      try
      {
        ConfigurationHelper.DeserializeSection (configuration, xmlFragment);
        Assert.Fail ("Expected exception not thrown.");
      }
      catch (ConfigurationErrorsException ex)
      {
        Assert.That (ex.Message, Is.EqualTo ("Required attribute 'project' not found."));
      }
    }
  }
}