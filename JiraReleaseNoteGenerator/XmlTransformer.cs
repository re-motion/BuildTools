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
using System.Diagnostics;
using System.IO;
using Remotion.BuildTools.JiraReleaseNoteGenerator.Utilities;

namespace Remotion.BuildTools.JiraReleaseNoteGenerator
{
  public class XmlTransformer : IXmlTransformer
  {
    // stylesheet path
    private const string _xsltStyleSheetPath = @"XmlUtilities\Main.xslt";
    // xslt processor path
    private const string xsltProcessorPath = @"XmlUtilities\Saxon\Transform.exe";


    public int GenerateHtmlFromXml (string xmlInputFile, string outputFile)
    {
      ArgumentUtility.CheckNotNull ("xmlInputFile", xmlInputFile);
      ArgumentUtility.CheckNotNull ("outputFile", outputFile);

      var _xmlInputFile = xmlInputFile;
      var _outputFile = outputFile;
   
      var arguments = String.Format ("-s:{0} -xsl:{1} -o:{2}", _xmlInputFile, _xsltStyleSheetPath, _outputFile);

      var xsltProcessor = new Process();
      xsltProcessor.StartInfo.FileName = xsltProcessorPath;
      xsltProcessor.StartInfo.Arguments = arguments;
      xsltProcessor.StartInfo.RedirectStandardError = true;
      xsltProcessor.StartInfo.RedirectStandardOutput = true;
      xsltProcessor.StartInfo.UseShellExecute = false;

      xsltProcessor.Start();
      Console.Error.Write (xsltProcessor.StandardError.ReadToEnd());
      Console.Out.Write (xsltProcessor.StandardOutput.ReadToEnd());
      xsltProcessor.WaitForExit();

      return xsltProcessor.ExitCode;
    }
  }
}