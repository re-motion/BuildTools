using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Remotion.BuildTools.MSBuildTasks.Jira.Semver
{
    public class SemVerParser
    {
        private string _versionPattern =
            @"^(?<major>\d+)\.(?<minor>\d+)\.(?<patch>\d+)(-(?<pre>alpha|beta|rc)\.(?<preversion>\d+))?$";

        public SemVer ParseVersion(string version)
        {
            if (!Regex.IsMatch(version, _versionPattern, RegexOptions.Multiline))
                throw new ArgumentException("Version has an invalid format. Expected equivalent to '1.2.3' or '1.2.3-alpha.4'");

            SemVer semVer = new SemVer();

            var match = Regex.Match(version, _versionPattern);
            semVer.Major = int.Parse(match.Groups["major"].ToString());
            semVer.Minor = int.Parse(match.Groups["minor"].ToString());
            semVer.Patch = int.Parse(match.Groups["patch"].ToString());

            if (!match.Groups["pre"].Success) return semVer;

            PreReleaseStage preReleaseStage;
            Enum.TryParse(match.Groups["pre"].ToString(), out preReleaseStage);
            semVer.Pre = preReleaseStage;

            semVer.PreReleaseCounter = int.Parse(match.Groups["preversion"].ToString());

            return semVer;
        }
    }
}