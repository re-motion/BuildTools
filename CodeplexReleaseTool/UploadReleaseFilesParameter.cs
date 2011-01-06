using System;
using Remotion.Text.CommandLine;

namespace CodeplexReleaseTool
{
  public class UploadReleaseFilesParameter
  {
    [CommandLineStringArgument ("projectName", false,
        Description = "The Codeplex project name.",
        Placeholder = "project")]
    public string ProjectName = string.Empty;

    [CommandLineStringArgument ("releaseName", false,
        Description = "The name of the release.",
        Placeholder = "release")]
    public string ReleaseName = string.Empty;

    [CommandLineStringArgument ("fileDataPath", false,
        Description = "The path to the file.",
        Placeholder = "path")]
    public string FileDataPath = string.Empty;

    [CommandLineStringArgument ("fileDisplayName", true,
       Description = "The display name for the file. If this is not specified, the FileName will be displayed.",
       Placeholder = "displayName")]
    public string FileDisplayName = string.Empty;

    [CommandLineStringArgument ("mimeType", true,
       Description = "The MIME type for the file. The default value is application/octet-stream.",
       Placeholder = "mimeType")]
    public string MimeType = "application/octet-stream";

    [CommandLineStringArgument ("fileType", false,
       Description = "The type of file in the release. Valid values are RuntimeBinary, SourceCode, Documentation, Example.",
       Placeholder = "fileType")]
    public string FileType = string.Empty;

    [CommandLineStringArgument ("username", false,
        Description = "The codeplex user name.",
        Placeholder = "user")]
    public string Username = string.Empty;

    [CommandLineStringArgument ("password", false,
        Description = "The codeplex password.",
        Placeholder = "password")]
    public string Password = string.Empty;
  }
}