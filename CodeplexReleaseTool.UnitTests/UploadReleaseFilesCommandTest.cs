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
      _serviceMock = MockRepository.GenerateMock<ICodeplexWebService>();
      _command = new UploadReleaseFilesCommand (_serviceMock);
    }

    [Test]
    public void ValidParameters ()
    {
      var fileDataPath = Path.Combine (AppDomain.CurrentDomain.BaseDirectory, "Resources\\TestFile.txt");
      var args = "/projectName:ProjectName /releaseName:ReleaseName /fileDataPath:" + fileDataPath
                 + " /fileDisplayName:FileDisplayName /mimeType:MimeType "
                 + "/fileType:FileType /username:Username /password:Password";

      _serviceMock
        .Expect (
          mock =>
          mock.UploadTheReleaseFiles (
              Arg.Is ("ProjectName"),
              Arg.Is ("ReleaseName"),
              Arg<ReleaseFile[]>.Is.Anything,
              Arg<string>.Is.Null,
              Arg.Is ("Username"),
              Arg.Is ("Password")))
              .WhenCalled (mi =>
              {
                var releaseFiles = (ReleaseFile[]) mi.Arguments[2];
                Assert.That (releaseFiles.Length, Is.EqualTo (1));
                Assert.That (releaseFiles[0].FileName, Is.EqualTo ("TestFile.txt"));
                Assert.That (releaseFiles[0].Name, Is.EqualTo ("FileDisplayName"));
                Assert.That (releaseFiles[0].FileType, Is.EqualTo ("FileType"));
                Assert.That (releaseFiles[0].MimeType, Is.EqualTo ("MimeType"));
                Assert.That (releaseFiles[0].FileData.Length, Is.GreaterThan(0));
              });
      _serviceMock.Replay();

      _command.Execute (args.Split (' '));

      _serviceMock.VerifyAllExpectations();
    }

    [Test]
    [ExpectedException (typeof (FileNotFoundException))]
    public void InvalidFilePath ()
    {
      var args = "/projectName:ProjectName /releaseName:ReleaseName /fileDataPath:FileDataPath /fileDisplayName:FileDisplayName /mimeType:MimeType "
                 + "/fileType:FileType /username:Username /password:Password";

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