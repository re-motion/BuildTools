using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Remotion.BuildTools.MSBuildTasks.Jira
{
  public interface IJiraProjectVersionService
  {
    IEnumerable<JiraProjectVersion> GetUnreleasedVersions (string projectName, string versionNamePattern);

    void CreateVersion (string projectName, string versionName);

    void ReleaseVersion (string versionId);

    void DeleteVersion (string projectName, string versionName);
  }
}
