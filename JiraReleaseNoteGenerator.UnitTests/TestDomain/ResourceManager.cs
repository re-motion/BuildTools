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

namespace Remotion.BuildTools.JiraReleaseNoteGenerator.UnitTests.TestDomain
{
  public static class ResourceManager
  {
    public static byte[] GetResource (string resourceID)
    {
      using (Stream resourceStream = GetResourceStream (resourceID))
      {
        byte[] buffer = new byte[resourceStream.Length];
        resourceStream.Read (buffer, 0, buffer.Length);
        return buffer;
      }
    }

    public static Stream GetResourceStream (string resourceID)
    {
      Type resourceManagerType = typeof (ResourceManager);
      Stream stream = resourceManagerType.Assembly.GetManifestResourceStream (resourceManagerType, resourceID);
      Debug.Assert (stream != null, string.Format ("Resource '{0}.{1}' was not found", resourceManagerType.Namespace, resourceID));
      return stream;
    }
  }
}