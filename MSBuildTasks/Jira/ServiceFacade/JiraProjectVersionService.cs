// Copyright (c) rubicon IT GmbH, www.rubicon.eu
//
// See the NOTICE file distributed with this work for additional information
// regarding copyright ownership.  rubicon licenses this file to you under 
// the Apache License, Version 2.0 (the "License"); you may not use this 
// file except in compliance with the License.  You may obtain a copy of the 
// License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT 
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.  See the 
// License for the specific language governing permissions and limitations
// under the License.
// 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using RestSharp;

namespace Remotion.BuildTools.MSBuildTasks.Jira.ServiceFacade
{
  public class JiraProjectVersionService : IJiraProjectVersionService
  {
    private readonly RestClient _client;

    public RestClient RestClient
    {
      get { return _client; }
    }

    public JiraProjectVersionService(string jiraUrl, string jiraUsername, string jiraPassword)
    {
      _client = new RestClient (jiraUrl) { Authenticator = new HttpBasicAuthenticator (jiraUsername, jiraPassword) };
    }

    public string CreateVersion (string projectKey, string versionName, DateTime releaseDate)
    {
      var request = CreateRestRequest ("version", Method.POST);

      var adjustedReleaseDate = AdjustReleaseDateForJira(releaseDate);
      var projectVersion = new JiraProjectVersion { name = versionName, project = projectKey, releaseDate = adjustedReleaseDate };
      request.AddBody (projectVersion);

      var newProjectVersion = DoRequest<JiraProjectVersion> (request, HttpStatusCode.Created);
      return newProjectVersion.Data.id;
    }

    public string CreateSubsequentVersion (string projectKey, string versionPattern, int versionComponentToIncrement, DayOfWeek versionReleaseWeekday)
    {
      // Determine next version name
      var lastUnreleasedVersion = FindUnreleasedVersions (projectKey, versionPattern).Last();
      var nextVersionName = IncrementVersion (lastUnreleasedVersion.name, versionComponentToIncrement);

      // Determine next release day
      if (!lastUnreleasedVersion.releaseDate.HasValue)
        throw new JiraException ("releaseDate of lastUnreleasedVersion must have a value but is null");

      var nextReleaseDay = lastUnreleasedVersion.releaseDate.Value.AddDays(1);
      while(nextReleaseDay.DayOfWeek != versionReleaseWeekday)
        nextReleaseDay = nextReleaseDay.AddDays (1);

      var newVersionId = CreateVersion (projectKey, nextVersionName, nextReleaseDay);
      MoveVersion (newVersionId, lastUnreleasedVersion.self);

      return newVersionId;
    }

    private void MoveVersion(string versionId, string afterVersionUrl)
    {
      var request = CreateRestRequest ("version/" + versionId + "/move", Method.POST);

      request.AddBody (new { after = afterVersionUrl });

      DoRequest (request, HttpStatusCode.OK);
    }

    private string IncrementVersion (string version, int componentToIncrement)
    {
      if(componentToIncrement < 1 || componentToIncrement > 4)
        throw new ArgumentException ("componentToIncrement must be between 1 and 4");

      var versionParts = version.Split ('.').Select(int.Parse).ToArray();
      if(versionParts.Length < componentToIncrement)
        throw new ArgumentException(string.Format ("version must have at least {0} components", componentToIncrement));

      ++versionParts[componentToIncrement - 1];
      return versionParts.Select (p => p.ToString()).Aggregate ((l, r) => (l + "." + r));
    }

    public void ReleaseVersion (string versionID, string nextVersionID)
    {
      if(versionID != nextVersionID)
      {
        var nonClosedIssues = FindAllNonClosedIssues (versionID);
        MoveIssuesToVersion (nonClosedIssues, versionID, nextVersionID);
      }

      ReleaseVersion (versionID);
    }

    private void ReleaseVersion(string versionID)
    {
      var resource = "version/" + versionID;
      var request = CreateRestRequest (resource, Method.PUT);

      var adjustedReleaseDate = AdjustReleaseDateForJira(DateTime.Today);
      var projectVersion = new JiraProjectVersion { id = versionID, released = true, releaseDate = adjustedReleaseDate };
      request.AddBody (projectVersion);

      DoRequest (request, HttpStatusCode.OK);
    }

    private void MoveIssuesToVersion (IEnumerable<JiraNonClosedIssue> issues, string oldVersionId, string newVersionId)
    {
      foreach(var issue in issues)
      {
        var resource = "issue/" + issue.id;
        var request = CreateRestRequest (resource, Method.PUT);

        var newFixVersions = issue.fields.fixVersions;
        newFixVersions.RemoveAll(v => v.id == oldVersionId);
        newFixVersions.Add(new JiraVersion{id = newVersionId});
        
        var body = new { fields = new { fixVersions = newFixVersions.Select(v => new{v.id}) } };
        request.AddBody (body);

        DoRequest<JiraIssue> (request, HttpStatusCode.NoContent);
      }
    }

    public IEnumerable<JiraNonClosedIssue> FindAllNonClosedIssues(string versionId)
    {
      var jql = "fixVersion=" + versionId + " and status != \"closed\"";
      var resource = "search?jql=" + jql + "&fields=id,fixVersions";
      var request = CreateRestRequest (resource, Method.GET);

      var response = DoRequest<JiraNonClosedIssues> (request, HttpStatusCode.OK);
      return response.Data.issues;
    }

    public void DeleteVersion (string projectKey, string versionName)
    {
      var versions = GetVersions (projectKey);
      var versionToDelete = versions.SingleOrDefault (v => v.name == versionName);
      if(versionToDelete == null)
        throw new JiraException (string.Format("Error, version with name '{0}' does not exist in project '{1}'.", versionName, projectKey));

      var resource = "version/" + versionToDelete.id;
      var request = CreateRestRequest (resource, Method.DELETE);
      DoRequest (request, HttpStatusCode.NoContent);
    }

    public IEnumerable<JiraProjectVersion> FindVersions (string projectKey, string versionPattern)
    {
      var versions = GetVersions (projectKey);
      return versions.Where (v => Regex.IsMatch (v.name, versionPattern));
    }

    public IEnumerable<JiraProjectVersion> FindUnreleasedVersions (string projectKey, string versionPattern)
    {
      return FindVersions (projectKey, versionPattern).Where (v => v.released != true);
    }

    private IEnumerable<JiraProjectVersion> GetVersions (string projectKey)
    {
      var resource = "project/" + projectKey + "/versions";
      var request = CreateRestRequest (resource, Method.GET);

      var response = DoRequest<List<JiraProjectVersion>> (request, HttpStatusCode.OK);
      return response.Data;
    }

    private static DateTime AdjustReleaseDateForJira (DateTime releaseDate)
    {
      var releaseDateAsUtcTime = releaseDate.ToUniversalTime();
      var difference = releaseDate - releaseDateAsUtcTime;
      var adjustedReleaseDate = releaseDate + difference;
      return adjustedReleaseDate;
    }

    private IRestRequest CreateRestRequest (string resource, Method method)
    {
      var request = new RestRequest() { Method = method, RequestFormat = DataFormat.Json, Resource = resource };
      return request;
    }

    private void DoRequest(IRestRequest request, HttpStatusCode successCode)
    {
      DoRequest<object> (request, successCode);
    }

    private IRestResponse<T> DoRequest<T>(IRestRequest request, HttpStatusCode successCode) where T : new()
    {
      var response = _client.Execute<T> (request);
      if(response.StatusCode != successCode)
        throw new JiraException (string.Format("Error calling REST service, HTTP resonse is: {0}\nReturned content: {1}", response.StatusCode, response.Content));

      return response;
    }
  }
}
