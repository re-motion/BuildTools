using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Remotion.BuildTools.MSBuildTasks.Jira;

namespace BuildTools.MSBuildTasks.UnitTests
{
    [TestFixture]
    public class JiraProjectVersionServiceTest
    {
      private IJiraProjectVersionService _service;

      [SetUp]
      public void setUp()
      {
        _service = new JiraProjectVersionService("http://s0316:8080/jira/rest/api/2/", "dominik.rauch", "Rubicon01");
      }

      [Test]
      public void integrationTest()
      {
        DeleteVersionsIfExistent ("RM", "4.1.0", "4.1.1", "4.1.2", "4.2.0");

        // Create versions
        _service.CreateVersion ("RM", "4.1.0");
        _service.CreateVersion ("RM", "4.1.1");
        _service.CreateVersion ("RM", "4.1.2");
        _service.CreateVersion ("RM", "4.2.0");

        // Get latest unreleased version
        var versions = _service.GetUnreleasedVersions ("RM", "4.1.");
        Assert.That (versions.Count(), Is.EqualTo (3));

        var toRelease = versions.First();
        Assert.That (toRelease.name, Is.EqualTo ("4.1.0"));

        var versions2 = _service.GetUnreleasedVersions ("RM", "4.2.");
        Assert.That (versions2.Count(), Is.EqualTo (1));

        // Release version
        _service.ReleaseVersion (toRelease.id);

        // Get latest unreleased version again
        versions = _service.GetUnreleasedVersions ("RM", "4.1.");
        Assert.That (versions.Count(), Is.EqualTo (2));

        toRelease = versions.First();
        Assert.That (toRelease.name, Is.EqualTo ("4.1.1"));

        DeleteVersionsIfExistent ("RM", "4.1.0", "4.1.1", "4.1.2", "4.2.0");
      }

      [Test]
      public void testGetUnreleasedVersionsWithNonExistentPattern()
      {
        DeleteVersionsIfExistent ("RM", "a.b.c.d");

        // Try to get an unreleased version with a non-existent pattern
        var versions = _service.GetUnreleasedVersions ("RM", "a.b.c.d");
        Assert.That (versions.Count(), Is.EqualTo(0));
      }

      [Test]
      public void testCannotCreateVersionTwice()
      {
        DeleteVersionsIfExistent ("RM", "5.0.0");

        // Create version
        _service.CreateVersion ("RM", "5.0.0");

        // Try to create same version again, should throw
        Assert.Throws (typeof (JiraException), () => _service.CreateVersion ("RM", "5.0.0"));

        DeleteVersionsIfExistent ("RM", "5.0.0");
      }

      [Test]
      public void testDeleteVersion()
      {
        DeleteVersionsIfExistent ("RM", "6.0.0.0");

        _service.CreateVersion ("RM", "6.0.0.0");
        _service.DeleteVersion ("RM", "6.0.0.0");
      }

      [Test]
      public void testDeleteNonExistentVersion()
      {
        DeleteVersionsIfExistent ("RM", "6.0.0.0");

        Assert.Throws(typeof(JiraException), () => _service.DeleteVersion ("RM", "6.0.0.0"));
      }

      private void DeleteVersionsIfExistent(string projectName, params string[] versionNames)
      {
        foreach(var versionName in versionNames)
        {
          try
          {
            _service.DeleteVersion (projectName, versionName);
          }
          catch
          {
            // ignore
          }
        }
      }
    }
}
