using System;
using System.Text.RegularExpressions;

namespace Remotion.BuildTools.MSBuildTasks.Jira.SemanticVersioning
{
    public class SemanticVersionParser
    {
        private string _versionPattern =
            @"^(?<major>\d+)\.(?<minor>\d+)\.(?<patch>\d+)(-(?<pre>alpha|beta|rc)\.(?<preversion>\d+))?$";

        public SemanticVersion ParseVersion(string version)
        {
            if (!Regex.IsMatch(version, _versionPattern, RegexOptions.Multiline))
                throw new ArgumentException("Version has an invalid format. Expected equivalent to '1.2.3' or '1.2.3-alpha.4'");

            SemanticVersion semanticVersion = new SemanticVersion();

            var match = Regex.Match(version, _versionPattern);
            semanticVersion.Major = int.Parse(match.Groups["major"].ToString());
            semanticVersion.Minor = int.Parse(match.Groups["minor"].ToString());
            semanticVersion.Patch = int.Parse(match.Groups["patch"].ToString());

            if (!match.Groups["pre"].Success) return semanticVersion;

            PreReleaseStage preReleaseStage;
            Enum.TryParse(match.Groups["pre"].ToString(), out preReleaseStage);
            semanticVersion.Pre = preReleaseStage;

            semanticVersion.PreReleaseCounter = int.Parse(match.Groups["preversion"].ToString());

            return semanticVersion;
        }
    }
}