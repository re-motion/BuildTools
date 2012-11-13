using System;
using System.Collections.Generic;

namespace Remotion.BuildTools.MSBuildTasks.Jira.ServiceFacade
{
  public interface IJiraProjectVersionService
  {
    /// <summary>
    /// Creates a project version.
    /// </summary>
    /// <returns>New project version ID</returns>
    string CreateVersion (string projectKey, string versionName, DateTime releaseDate);

    /// <summary>
    /// Creates a subsequent project version.
    /// </summary>
    /// <returns>New project version ID</returns>
    string CreateSubsequentVersion (string projectKey, string versionPattern, int versionComponentToIncrement, DayOfWeek versionReleaseWeekday);

    /// <summary>
    /// Releases a project version and moves all open issues to another project version.
    /// </summary>
    void ReleaseVersion (string versionID, string nextVersionID);

    /// <summary>
    /// Deletes a project version.
    /// </summary>
    /// <exception cref="JiraException">Thrown if version does not exist.</exception>
    void DeleteVersion (string projectKey, string versionName);

    /// <summary>
    /// Returns all unreleased versions of the project.
    /// Filters by Regex.IsMatch(name, versionPattern) if versionPattern is not null.
    /// </summary
    /// <returns>List of project versions or empty sequence</returns>
    IEnumerable<JiraProjectVersion> FindUnreleasedVersions (string projectKey, string versionPattern);
  }
}
