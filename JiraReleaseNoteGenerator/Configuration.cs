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

namespace Remotion.BuildTools.JiraReleaseNoteGenerator
{
  public class Configuration : ConfigurationSection
  {
    private static Configuration _current;

    public static Configuration Current
    {
      get {
        if (_current == null)
          _current = (Configuration) ConfigurationManager.GetSection ("requestUrlBuilder");
        return _current; 
      }
    }

    [ConfigurationProperty ("url", IsRequired = true)]
    public string Url
    {
      get { return (string) this["url"]; }
    }

    [ConfigurationProperty ("project", IsRequired = true)]
    public string Project
    {
      get { return (string) this["project"]; }
    }

    [ConfigurationProperty ("outputFile", DefaultValue = @".\Output\ReleaseNotes.html")]
    [StringValidator (InvalidCharacters = " ~!@#$%^&*()[]{}/;'\"|", MinLength = 1, MaxLength = 60)]
    public string OutputFileName
    {
      get { return (string) this["outputFile"]; }
    }

    [ConfigurationProperty ("xsltConfigFile", DefaultValue = @".\XmlUtilities\Config.xml")]
    [StringValidator (InvalidCharacters = " ~!@#$%^&*()[]{}/;'\"|", MinLength = 1, MaxLength = 60)]
    public string ConfigFile
    {
      get { return (string) this["xsltConfigFile"]; }
    }

    [ConfigurationProperty ("xsltStyleSheetPath", DefaultValue = @"XmlUtilities\Main.xslt")]
    public string XsltStyleSheetPath
    {
      get {  return (string) this["xsltStyleSheetPath"]; } 
    }

    [ConfigurationProperty ("xsltProcessorPath", DefaultValue = @"XmlUtilities\Saxon\Transform.exe")]
    public string XsltProcessorPath
    {
      get { return (string) this["xsltProcessorPath"]; }
    }
  }
}