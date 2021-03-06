﻿using System;
using System.Linq;
using System.Net;
using NUnit.Framework;
using Remotion.BuildTools.MSBuildTasks.Jira;
using Remotion.BuildTools.MSBuildTasks.Jira.ServiceFacadeImplementations;
using RestSharp;

namespace BuildTools.MSBuildTasks.UnitTests.Jira
{
  [TestFixture]
  public class JiraProjectVersionServiceTest
  {
    private const string c_jiraUrl = "https://www.re-motion.org/jira/rest/api/2/";
    private const string c_jiraProjectKey = "SRCBLDTEST";
    private const string c_jiraUsername = "floriandeckerrubicon";
    private const string c_jiraPassword = "rubicon01";

    private JiraProjectVersionService _service;
    private JiraProjectVersionRepairer _repairer;
    private JiraProjectVersionFinder _versionFinder;
    private JiraIssueService _issueService;
    private JiraRestClient _restClient;

    [SetUp]
    public void SetUp ()
    {
      
      IAuthenticator authenticator = new HttpBasicAuthenticator(c_jiraUsername, c_jiraPassword);
      _restClient = new JiraRestClient (c_jiraUrl, authenticator);
      _service = new JiraProjectVersionService (_restClient);
      _versionFinder = new JiraProjectVersionFinder (_restClient);
      _issueService = new JiraIssueService (_restClient);
      _repairer = new JiraProjectVersionRepairer (_service, _versionFinder);
    }

    [Test]
    public void Call_JiraCheckAuthentication()
    {
      var jiraCheckAuthenticationTask = new JiraCheckAuthentication
      {
        JiraProject = c_jiraProjectKey,
        JiraUrl = c_jiraUrl,
        JiraUsername = c_jiraUsername,
        JiraPassword = c_jiraPassword

      };
      jiraCheckAuthenticationTask.Execute();
    }

    [Test]
    public void IntegrationTest ()
    {
      DeleteVersionsIfExistent (c_jiraProjectKey, "4.1.0", "4.1.1", "4.1.2", "4.2.0");

      // Create versions
      _service.CreateVersion (c_jiraProjectKey, "4.1.0", DateTime.Today.AddDays(1));
      _service.CreateSubsequentVersion(c_jiraProjectKey, "4\\.1\\..*", 3, DayOfWeek.Monday);
      _service.CreateSubsequentVersion(c_jiraProjectKey, "4\\.1\\..*", 3, DayOfWeek.Tuesday);
      _service.CreateVersion (c_jiraProjectKey, "4.2.0", DateTime.Today.AddDays(7));

      // Get latest unreleased version
      var versions = _versionFinder.FindUnreleasedVersions (c_jiraProjectKey, "4.1.").ToList();
      Assert.That (versions.Count(), Is.EqualTo (3));

      var versionToRelease = versions.First();
      Assert.That (versionToRelease.name, Is.EqualTo ("4.1.0"));

      var versionToFollow = versions.Skip (1).First();
      Assert.That (versionToFollow.name, Is.EqualTo ("4.1.1"));

      var versions2 = _versionFinder.FindUnreleasedVersions (c_jiraProjectKey, "4.2.");
      Assert.That (versions2.Count(), Is.EqualTo (1));

      var additionalVersion = versions2.First();
      Assert.That (additionalVersion.name, Is.EqualTo ("4.2.0"));

      // Add issues to versionToRelease
      AddTestIssueToVersion ("My Test", false, versionToRelease);      
      AddTestIssueToVersion ("My closed Test", true, versionToRelease);
      AddTestIssueToVersion ("My multiple fixVersion Test", false, versionToRelease, additionalVersion);

      // Release version
      _service.ReleaseVersion (versionToRelease.id, versionToFollow.id);

      // Get latest unreleased version again
      versions = _versionFinder.FindUnreleasedVersions (c_jiraProjectKey, "4.1.").ToList ();
      Assert.That (versions.Count(), Is.EqualTo (2));

      var versionThatFollowed = versions.First();
      Assert.That (versionThatFollowed.name, Is.EqualTo ("4.1.1"));

      // Check whether versionThatFollowed has all the non-closed issues from versionToRelease
      var issues = _issueService.FindAllNonClosedIssues (versionThatFollowed.id);
      Assert.That (issues.Count(), Is.EqualTo (2));
      
      // Check whether the additionalVersion still has its issue
      additionalVersion = _versionFinder.FindUnreleasedVersions (c_jiraProjectKey, "4.2.").First();
      var additionalVersionIssues = _issueService.FindAllNonClosedIssues (additionalVersion.id);
      Assert.That (additionalVersionIssues.Count(), Is.EqualTo (1));

      DeleteVersionsIfExistent (c_jiraProjectKey, "4.1.0", "4.1.1", "4.1.2", "4.2.0");
    }

    private JiraIssue AddTestIssueToVersion (string summaryOfIssue, bool closed, params JiraProjectVersion[] toRelease)
    {
      // Create new issue
      var resource = "issue";
      var request = new RestRequest { Method = Method.POST, RequestFormat = DataFormat.Json, Resource = resource};

      var body = new { fields = new { project = new { key = c_jiraProjectKey }, issuetype = new { name = "Task" }, summary = summaryOfIssue, description = "testDescription", fixVersions = toRelease.Select(v=>new{v.id}) } };
      request.AddBody (body);
      
      var response = _restClient.DoRequest<JiraIssue> (request, HttpStatusCode.Created);

      // Close issue if necessary
      if(closed)
      {
        var issue = response.Data;
        CloseIssue (issue.id);
      }

      return response.Data;
    }

    private void CloseIssue(string issueID)
    {
      var resource = "issue/" + issueID + "/transitions";
      var request = new RestRequest { Method = Method.POST, RequestFormat = DataFormat.Json, Resource = resource };

      var body = new { transition = new { id = 2} };
      request.AddBody (body);

      _restClient.DoRequest(request, HttpStatusCode.NoContent);
    }

    [Test]
    public void TestGetUnreleasedVersionsWithNonExistentPattern ()
    {
      DeleteVersionsIfExistent (c_jiraProjectKey, "a.b.c.d");

      // Try to get an unreleased version with a non-existent pattern
      var versions = _versionFinder.FindUnreleasedVersions (c_jiraProjectKey, "a.b.c.d");
      Assert.That (versions.Count(), Is.EqualTo (0));
    }

    [Test]
    public void TestCannotCreateVersionTwice ()
    {
      DeleteVersionsIfExistent (c_jiraProjectKey, "5.0.0");

      // Create version
      _service.CreateVersion (c_jiraProjectKey, "5.0.0", DateTime.Today.AddDays(14));

      // Try to create same version again, should throw
      Assert.Throws (typeof (JiraException), () => _service.CreateVersion (c_jiraProjectKey, "5.0.0", DateTime.Today.AddDays(14+1)));

      DeleteVersionsIfExistent (c_jiraProjectKey, "5.0.0");
    }

    [Test]
    public void TestDeleteVersion ()
    {
      DeleteVersionsIfExistent (c_jiraProjectKey, "6.0.0.0");

      _service.CreateVersion (c_jiraProjectKey, "6.0.0.0", DateTime.Today.AddDays(21));
      _service.DeleteVersion (c_jiraProjectKey, "6.0.0.0");
    }

    [Test]
    public void TestDeleteNonExistentVersion ()
    {
      DeleteVersionsIfExistent (c_jiraProjectKey, "6.0.0.0");

      Assert.Throws (typeof (JiraException), () => _service.DeleteVersion (c_jiraProjectKey, "6.0.0.0"));
    }

    [Test]
    public void TestReleaseVersionAndSquashUnreleased_ShouldThrowOnReleasedVersionsToBeSquashed ()
    {
      DeleteVersionsIfExistent (c_jiraProjectKey, "6.0.1-alpha.1", "6.0.1-alpha.2", "6.0.1-beta.1");

      //Create versions mangled to verify they are ordered before squashed
      _service.CreateVersion (c_jiraProjectKey, "6.0.1-alpha.2", null);
      _service.CreateVersion (c_jiraProjectKey, "6.0.1-beta.1", null);
      _service.CreateVersion (c_jiraProjectKey, "6.0.1-alpha.1", null);

      var alpha1Version = _versionFinder.FindVersions (c_jiraProjectKey, "6.0.1-alpha.1").Single (x => x.name == "6.0.1-alpha.1");
      var alpha2Version = _versionFinder.FindVersions (c_jiraProjectKey, "6.0.1.alpha.2").Single (x => x.name == "6.0.1-alpha.2");
      var beta1Version = _versionFinder.FindVersions (c_jiraProjectKey, "6.0.1-beta.1").Single (x => x.name == "6.0.1-beta.1");

      _service.ReleaseVersion (alpha2Version.id, beta1Version.id);

      Assert.That (
          () => { _service.ReleaseVersionAndSquashUnreleased (alpha1Version.id, beta1Version.id, c_jiraProjectKey); },
          Throws.Exception.TypeOf<JiraException>().With.Message.EqualTo (
              "Version '" + alpha1Version.name + "' cannot be released, as there is already one or multiple released version(s) (" + alpha2Version.name
              + ") before the next version '" + beta1Version.name + "'."));

      Assert.That (_versionFinder.FindVersions (c_jiraProjectKey, "6.0.1-alpha.2").SingleOrDefault (x => x.name == "6.0.1-alpha.2"), Is.Not.Null);

      DeleteVersionsIfExistent (c_jiraProjectKey, "6.0.1-alpha.1", "6.0.1-alpha.2", "6.0.1-beta.1");
    }

    [Test]
    public void TestReleaseVersionAndSquashUnreleased_ShouldThrowOnSquashedVersionsContainingClosedIssues ()
    {
      DeleteVersionsIfExistent (c_jiraProjectKey, "6.0.1-alpha.1", "6.0.1-alpha.2", "6.0.1-beta.1");

      //Create versions mangled to verify they are ordered before squashed
      _service.CreateVersion (c_jiraProjectKey, "6.0.1-alpha.2", null);
      _service.CreateVersion (c_jiraProjectKey, "6.0.1-beta.1", null);
      _service.CreateVersion (c_jiraProjectKey, "6.0.1-alpha.1", null);

      var alpha1Version = _versionFinder.FindVersions (c_jiraProjectKey, "6.0.1-alpha.1").Single (x => x.name == "6.0.1-alpha.1");
      var alpha2Version = _versionFinder.FindVersions (c_jiraProjectKey, "6.0.1.alpha.2").Single (x => x.name == "6.0.1-alpha.2");
      var beta1Version = _versionFinder.FindVersions (c_jiraProjectKey, "6.0.1-beta.1").Single (x => x.name == "6.0.1-beta.1");

      var jiraIssue = AddTestIssueToVersion ("Closed issues", true, alpha2Version);

      Assert.That (
          () => { _service.ReleaseVersionAndSquashUnreleased (alpha1Version.id, beta1Version.id, c_jiraProjectKey); },
          Throws.Exception.TypeOf<JiraException>().With.Message.EqualTo (
              "Version '" + alpha1Version.name + "' cannot be released, as one  or multiple versions contain closed issues (" + jiraIssue.key + ")"));

      Assert.That (_versionFinder.FindVersions(c_jiraProjectKey, "6.0.1-alpha.2").SingleOrDefault(x => x.name == "6.0.1-alpha.2"), Is.Not.Null);

      DeleteVersionsIfExistent (c_jiraProjectKey, "6.0.1-alpha.1", "6.0.1-alpha.2", "6.0.1-beta.1");
    }


    [Test]
    public void TestReleaseVersionAndSquashUnreleased_ShouldSquashUnreleasedAndMoveIssues ()
    {
      DeleteVersionsIfExistent (c_jiraProjectKey, "6.0.1-alpha.1", "6.0.1-alpha.2", "6.0.1-beta.1");
    
      //Create versions mangled to verify they are ordered before squashed
      _service.CreateVersion (c_jiraProjectKey, "6.0.1-alpha.2", null);
      _service.CreateVersion (c_jiraProjectKey, "6.0.1-beta.1", null);
      _service.CreateVersion (c_jiraProjectKey, "6.0.1-alpha.1", null);

      var alpha1Version = _versionFinder.FindVersions (c_jiraProjectKey, "6.0.1-alpha.1").Single (x => x.name == "6.0.1-alpha.1");
      var alpha2Version = _versionFinder.FindVersions (c_jiraProjectKey, "6.0.1.alpha.2").Single (x => x.name == "6.0.1-alpha.2");
      var beta1Version = _versionFinder.FindVersions (c_jiraProjectKey, "6.0.1-beta.1").Single (x => x.name == "6.0.1-beta.1");

      AddTestIssueToVersion ("Open issues", false, alpha2Version);

      _service.ReleaseVersionAndSquashUnreleased (alpha1Version.id, beta1Version.id, c_jiraProjectKey);

      Assert.That (_versionFinder.FindVersions (c_jiraProjectKey, "6.0.1-alpha.2").SingleOrDefault (x => x.name == "6.0.1-alpha.2"), Is.Null);

      //Assert that the Open Issues of deleted alpha2Version got moved to beta1Version
      Assert.That (_issueService.FindAllNonClosedIssues (beta1Version.id).Count(), Is.EqualTo(1));

      DeleteVersionsIfExistent (c_jiraProjectKey, "6.0.1-alpha.1", "6.0.1-alpha.2", "6.0.1-beta.1");
    }

    [Test]
    public void TestReleaseVersionAndSquashUnreleased_ShouldSquashMultipleUnreleasedAndMoveIssues()
    {
      DeleteVersionsIfExistent (c_jiraProjectKey, "6.0.1-alpha.1", "6.0.1-alpha.2", "6.0.1-alpha.3", "6.0.1-beta.1");

      //Create versions mangled to verify they are ordered before squashed
      _service.CreateVersion (c_jiraProjectKey, "6.0.1-beta.1", null);
      _service.CreateVersion (c_jiraProjectKey, "6.0.1-alpha.3", null);
      _service.CreateVersion (c_jiraProjectKey, "6.0.1-alpha.1", null);
      _service.CreateVersion (c_jiraProjectKey, "6.0.1-alpha.2", null);

      var alpha1Version = _versionFinder.FindVersions (c_jiraProjectKey, "6.0.1-alpha.1").Single (x => x.name == "6.0.1-alpha.1");
      var alpha2Version = _versionFinder.FindVersions (c_jiraProjectKey, "6.0.1.alpha.2").Single (x => x.name == "6.0.1-alpha.2");
      var alpha3Version = _versionFinder.FindVersions (c_jiraProjectKey, "6.0.1.alpha.3").Single (x => x.name == "6.0.1-alpha.3");
      var beta1Version = _versionFinder.FindVersions (c_jiraProjectKey, "6.0.1-beta.1").Single (x => x.name == "6.0.1-beta.1");

      AddTestIssueToVersion ("Open issues", false, alpha2Version);
      AddTestIssueToVersion ("Open issues", false, alpha3Version);

      _service.ReleaseVersionAndSquashUnreleased (alpha1Version.id, beta1Version.id, c_jiraProjectKey);

      Assert.That(_versionFinder.FindVersions (c_jiraProjectKey, "6.0.1-alpha.2").SingleOrDefault (x => x.name == "6.0.1-alpha.2"), Is.Null);
      Assert.That(_versionFinder.FindVersions (c_jiraProjectKey, "6.0.1-alpha.3").SingleOrDefault (x => x.name == "6.0.1-alpha.3"), Is.Null);

      //Assert that the Open Issues of deleted alpha2Version got moved to beta1Version
      Assert.That(_issueService.FindAllNonClosedIssues(beta1Version.id).Count(), Is.EqualTo(2));

      DeleteVersionsIfExistent(c_jiraProjectKey, "6.0.1-alpha.1", "6.0.1-alpha.2", "6.0.1-alpha.3", "6.0.1-beta.1");
    }

    [Test]
    public void TestReleaseVersionAndSquashUnreleased_ShouldNotSquashUnrelatedVersions()
    {
      DeleteVersionsIfExistent (c_jiraProjectKey, "2.2.0", "3.0.0-alpha.1", "3.0.0-alpha.2", "3.0.0");

      //Create versions mangled to verify they are ordered before squashed
      _service.CreateVersion (c_jiraProjectKey, "3.0.0-alpha.1", null);
      _service.CreateVersion (c_jiraProjectKey, "2.2.0", null);
      _service.CreateVersion (c_jiraProjectKey, "3.0.0-alpha.2", null);
      _service.CreateVersion (c_jiraProjectKey, "3.0.0", null);

      var version3alpha1 = _versionFinder.FindVersions (c_jiraProjectKey, "3.0.0-alpha.1").Single (x => x.name == "3.0.0-alpha.1");
      var version3alpha2 = _versionFinder.FindVersions (c_jiraProjectKey, "3.0.0-alpha.2").Single (x => x.name == "3.0.0-alpha.2");

      _service.ReleaseVersionAndSquashUnreleased (version3alpha1.id, version3alpha2.id, c_jiraProjectKey);

      Assert.That (_versionFinder.FindVersions (c_jiraProjectKey, "2.2.0").SingleOrDefault (x => x.name == "2.2.0"), Is.Not.Null);
      Assert.That (_versionFinder.FindVersions (c_jiraProjectKey, "3.0.0").SingleOrDefault (x => x.name == "3.0.0"), Is.Not.Null);

      DeleteVersionsIfExistent (c_jiraProjectKey, "2.2.0", "3.0.0", "3.0.0-alpha.1", "3.0.0-alpha.2");
    }

    [Test]
    public void TestSortingNetVersion ()
    {
      const string firstVersion = "1.16.32.0";
      const string secondVersion = "1.16.32.1";
      const string thirdVersion = "1.16.32.2";

      DeleteVersionsIfExistent (c_jiraProjectKey, firstVersion, secondVersion, thirdVersion);

      _service.CreateVersion (c_jiraProjectKey, firstVersion, null);
      _service.CreateVersion (c_jiraProjectKey, thirdVersion, null);
      var toBeRepairedVersionId = _service.CreateVersion (c_jiraProjectKey, secondVersion, null);
      _repairer.RepairVersionPosition (toBeRepairedVersionId);

      var versions = _versionFinder.FindVersions (c_jiraProjectKey, "(?s).*").ToList();

      var positionFirstVersion = versions.IndexOf (versions.Single (x => x.name == firstVersion));
      var positionSecondVersion = versions.IndexOf (versions.Single (x => x.name == secondVersion));
      var positionThirdVersion = versions.IndexOf (versions.Single (x => x.name == thirdVersion));


      Assert.That (positionFirstVersion < positionSecondVersion, Is.True);
      Assert.That (positionSecondVersion < positionThirdVersion, Is.True);
    }

    [Test]
    public void TestSortingSemanticVersion ()
    {
      const string firstVersion = "2.1.3";
      const string secondVersion = "2.2.0-alpha.5";
      const string thirdVersion = "2.2.0";

      DeleteVersionsIfExistent (c_jiraProjectKey, firstVersion, secondVersion, thirdVersion);

      _service.CreateVersion (c_jiraProjectKey, firstVersion, null);
      _service.CreateVersion (c_jiraProjectKey, thirdVersion, null);
      var toBeRepairedVersionId = _service.CreateVersion (c_jiraProjectKey, secondVersion, null);
      _repairer.RepairVersionPosition (toBeRepairedVersionId);

      var versions = _versionFinder.FindVersions (c_jiraProjectKey, "(?s).*").ToList();

      var positionFirstVersion = versions.IndexOf (versions.Single (x => x.name == firstVersion));
      var positionSecondVersion = versions.IndexOf (versions.Single (x => x.name == secondVersion));
      var positionThirdVersion = versions.IndexOf (versions.Single (x => x.name == thirdVersion));


      Assert.That (positionFirstVersion < positionSecondVersion, Is.True);
      Assert.That (positionSecondVersion < positionThirdVersion, Is.True);
    }

    [Test]
    public void TestSortingWithInvalidVersions ()
    {
      const string firstVersion = "1.17.21.0";
      const string secondVersion = "NotValidVersion";
      const string thirdVersion = "1.16.31.0";
      const string betweenFirstAndSecondVersion = "1.17.22.0";

      DeleteVersionsIfExistent (c_jiraProjectKey, firstVersion, secondVersion, thirdVersion, betweenFirstAndSecondVersion);

      _service.CreateVersion (c_jiraProjectKey, firstVersion, null);
      _service.CreateVersion (c_jiraProjectKey, secondVersion, null);
      _service.CreateVersion (c_jiraProjectKey, thirdVersion, null);
      var toBeRepairedVersionId = _service.CreateVersion (c_jiraProjectKey, betweenFirstAndSecondVersion, null);
      _repairer.RepairVersionPosition (toBeRepairedVersionId);

      var versions = _versionFinder.FindVersions (c_jiraProjectKey, "(?s).*").ToList();

      var positionFirstVersion = versions.IndexOf (versions.Single (x => x.name == firstVersion));
      var positionbetweenFirstAndSecondVersion = versions.IndexOf (versions.Single (x => x.name == betweenFirstAndSecondVersion));
      var positionSecondVersion = versions.IndexOf (versions.Single (x => x.name == secondVersion));
      var positionThirdVersion = versions.IndexOf (versions.Single (x => x.name == thirdVersion));


      Assert.That (positionFirstVersion < positionbetweenFirstAndSecondVersion, Is.True);
      Assert.That (positionbetweenFirstAndSecondVersion < positionSecondVersion, Is.True);
      Assert.That (positionSecondVersion < positionThirdVersion, Is.True);
    }

    private void DeleteVersionsIfExistent (string projectName, params string[] versionNames)
    {
      foreach (var versionName in versionNames)
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