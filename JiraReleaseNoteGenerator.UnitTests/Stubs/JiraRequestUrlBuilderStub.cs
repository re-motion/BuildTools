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
using Remotion.BuildTools.JiraReleaseNoteGenerator.Utility;

namespace Remotion.BuildTools.JiraReleaseNoteGenerator.UnitTests
{
  public class JiraRequestUrlBuilderStub : IJiraRequestUrlBuilder
  {
    public string FixVersion { get; set; }
    public string Status { get; set; }
    public string[] Keys { get; set; }

    public string Build ()
    {
      var url = new StringBuilder();

      if (FixVersion != null)
      {
        url.Append ("v");
        url.Append (FixVersion);
      }

      if (Status != null)
      {
        url.Append ("_");
        url.Append (Status);
      }

      if (Keys != null)
      {
        for (int i = 0; i < Keys.Length; i++)
        {
          if (i != 0)
            url.Append ("_");
          url.Append (Keys[i]);
        }
      }

      return url.ToString();
    }

    public bool IsValidQuery ()
    {
      if (String.IsNullOrEmpty (FixVersion) && ArrayUtility.IsNullOrEmpty (Keys))
        return false;

      if (Status != null && Status.Length == 0)
        return false;
      
      return true;
    }
  }
}