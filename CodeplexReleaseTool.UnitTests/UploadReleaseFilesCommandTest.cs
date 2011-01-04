using System;
using System.IO;
using NUnit.Framework;
using Remotion.Text.CommandLine;
using Rhino.Mocks;

namespace CodeplexReleaseTool.UnitTests
{
  [TestFixture]
  public class UploadReleaseFilesCommandTest
  {
    private ICodeplexWebService _serviceMock;
    private UploadReleaseFilesCommand _command;

    [SetUp]
    public void SetUp ()
    {
      _serviceMock = MockRepository.GenerateMock<ICodeplexWebService> ();
      _command = new UploadReleaseFilesCommand (_serviceMock);
    }

    //TODO: Test for valid parameters

    [Test]
    [ExpectedException(typeof(FileNotFoundException))]
    public void InvalidFilePath ()
    {
      var args = "/projectName:ProjectName /releaseName:ReleaseName /fileDataPath:FileDataPath /fileDisplayName:FileDisplayName /mimeType:MimeType "
        +"/fileType:FileType /username:Username /password:Password";

      _command.Execute (args.Split (' '));
    }

    [Test]
    [ExpectedException (typeof (MissingRequiredCommandLineParameterException))]
    public void MissingParameters ()
    {
      _command.Execute (new string[0]);
    }

    [Test]
    [ExpectedException (typeof (InvalidCommandLineArgumentNameException))]
    public void InvalidParameters ()
    {
      _command.Execute (new[] { "/sdfsd" });
    }
  }
}