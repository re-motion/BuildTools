using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using RestSharp;

namespace Remotion.BuildTools.MSBuildTasks.Jira
{
  public class JiraProjectVersionService : IJiraProjectVersionService
  {
    private string JiraUrl { get; set; }
    private string JiraUsername { get; set; }
    private string JiraPassword { get; set; }

    private RestClient _client;

    public JiraProjectVersionService(string jiraUrl, string jiraUsername, string jiraPassword)
    {
      JiraUrl = jiraUrl;
      JiraUsername = jiraUsername;
      JiraPassword = jiraPassword;

      _client = new RestClient (JiraUrl)
                {
                  Authenticator = new HttpBasicAuthenticator (JiraUsername, JiraPassword)
                };
    }

    public IEnumerable<JiraProjectVersion> GetUnreleasedVersions (string projectName, string versionNamePattern)
    {
      var versions = GetVersions (projectName);
      return versions.Where (v => v.released == false && v.name.StartsWith(versionNamePattern)).OrderBy(v => v.name).ToList();
    }

    public void CreateVersion(string projectName, string versionName)
    {
      var request = new RestRequest () { 
        Method = Method.POST,
        RequestFormat = DataFormat.Json, 
        Resource = "version"
      };

      var projectVersion = new JiraProjectVersion { name = versionName, project = projectName, releaseDate = DateTime.Today.AddDays(7) };
      request.AddBody (projectVersion);

      var response = _client.Execute (request);
      if(response.StatusCode != HttpStatusCode.Created)
        throw new JiraException ("Error calling REST service, HTTP response is: " + response.StatusCode);
    }

    public void ReleaseVersion (string versionId)
    {
       var request = new RestRequest () { 
        Method = Method.PUT,
        RequestFormat = DataFormat.Json, 
        Resource = "version/" + versionId
      };

      var projectVersion = new JiraProjectVersion { id = versionId, released = true, releaseDate = DateTime.Today };
      request.AddBody (projectVersion);

      var response = _client.Execute (request);
      if(response.StatusCode != HttpStatusCode.OK)
        throw new JiraException ("Error calling REST service, HTTP response is: " + response.StatusCode);
    }

    public void DeleteVersion (string projectName, string versionName)
    {
      var versions = GetVersions (projectName);
      var versionToDelete = versions.FirstOrDefault (v => v.name == versionName);

      if(versionToDelete == null)
        throw new JiraException (string.Format("Version name '{0}' does not exist in project '{1}'.", versionName, projectName));

       var request = new RestRequest () { 
        Method = Method.DELETE,
        RequestFormat = DataFormat.Json, 
        Resource = "version/" + versionToDelete.id
      };

      var response = _client.Execute (request);
      if(response.StatusCode != HttpStatusCode.NoContent)
        throw new JiraException ("Error calling REST service, HTTP response is: " + response.StatusCode);
    }

    private IEnumerable<JiraProjectVersion> GetVersions (string projectName)
    {
      var request = new RestRequest () { 
        Method = Method.GET,
        RequestFormat = DataFormat.Json, 
        Resource = "project/" + projectName + "/versions" 
      };

      var response = _client.Execute<List<JiraProjectVersion>> (request);
      if(response.StatusCode != HttpStatusCode.OK)
        throw new JiraException ("Error calling REST service, HTTP response is: " + response.StatusCode);
      
      var versions = response.Data;
      return versions;
    }
  }
}
