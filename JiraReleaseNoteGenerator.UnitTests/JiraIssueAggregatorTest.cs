// This file is part of the re-motion Build Tools (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Build Tools are free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.Xml.Linq;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace Remotion.BuildTools.JiraReleaseNoteGenerator.UnitTests
{
  [TestFixture]
  public class JiraIssueAggregatorTest
  {
    [Test]
    public void CreateUrl ()
    {
      const string version = "2.0.2";
      const string status = "closed";
      var xmlCreator = new JiraIssueAggregator (Configuration.Current);

      var output = xmlCreator.CreateUrl (version, status, null);
      var expectedOutput = Configuration.Current.Url + "/sr/jira.issueviews:searchrequest-xml/temp/SearchRequest.xml?jqlQuery=project+%3D+%22"
                           + Configuration.Current.Project + "%22+and+fixVersion+%3D+%22" + version + "%22+and+status%3D+%22" + status
                           + "%22&tempMax=1000";

      Assert.That (output, Is.EqualTo (expectedOutput));
    }

    [Test]
    public void GetBasicXmlDocument ()
    {
      const string version = "1.2";
      var xmlCreator = new JiraIssueAggregator (Configuration.Current);

      var output = xmlWithoutHeader (xmlCreator.GetIssuesForVersion (version).ToString());
      var expectedOutput = xmlWithoutHeader (XDocument.Load (@"..\..\TestDomain\IssuesForVersion_1.2.xml").ToString());

      Assert.That (output, Is.EqualTo (expectedOutput));
    }

    [Test]
    public void GetIssuesForKeys ()
    {
      var xmlCreator = new JiraIssueAggregator (Configuration.Current);
      
      var output = xmlWithoutHeader (xmlCreator.GetIssuesForKeys (new[] {"UUU-111", "UUU-112"}).ToString ());
      var expectedOutput = xmlWithoutHeader (XDocument.Load (@"..\..\TestDomain\IssuesForKeys_111_112.xml").ToString ());

      Assert.That (output, Is.EqualTo (expectedOutput));
    }

    [Test]
    public void FindUnknownParentKeys ()
    {
      var xmlCreator = new JiraIssueAggregator (Configuration.Current);

      var output = xmlCreator.FindUnknownParentKeys (XDocument.Load (@"..\..\TestDomain\SearchRequest.xml"));
      var expectedOutput = new[] { "COMMONS-4" };

      Assert.That (output, Is.EquivalentTo (expectedOutput));
    }

    private string xmlWithoutHeader (string xml)
    {
      return xml.Substring (xml.IndexOf ("-->"));
    }
  }
}